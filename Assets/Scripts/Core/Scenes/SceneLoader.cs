using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using SceneLoaded = System.Action<string, bool>;

public class SceneLoader : Singleton<SceneLoader> {

        private void Awake() {
            if (sInstance == null)
                sInstance = this;
        }

        private void OnDestory() {
            if (sInstance == this)
                sInstance = null;
            this.OnSceneLoaded = null;
        }

        public void Load(string nextSceneName, string firstPageName, params object[] parameters) {
            SceneController currentScene  = FindObjectOfType(typeof(SceneController)) as SceneController;
            string currentSceneName = currentScene.gameObject.name;
            if(!caches.ContainsKey(currentSceneName))
                caches.Add(currentSceneName, currentScene);
            
            SceneController nextScene = null;
            caches.TryGetValue(nextSceneName, out nextScene);

            bool loadedNextScene = false;

            NotifyClose(currentScene.gameObject);

            if(nextScene != currentScene) {
                if(nextScene != null) {
                    nextScene.gameObject.SetActive(true);
                }else{
                    Application.LoadLevelAdditive(nextSceneName);
                    loadedNextScene = true;
                }
                currentScene.gameObject.SetActive(false);
            }
            if(loadedNextScene){
                StartCoroutine(WaitOneFrame(() => {
                    InvokeOnSceneLoaded(nextSceneName, false);
                    NotifyOpen(GameObject.Find(nextSceneName), firstPageName, parameters);
                }));
            }else{
                InvokeOnSceneLoaded(nextSceneName, true);
                NotifyOpen(nextScene.gameObject, firstPageName, parameters);
            }
        }
	
        public void CleanLoad(string nextSceneName, string firstPageName, params object[] parameters) {
            SceneController currentScene = FindObjectOfType(typeof(SceneController)) as SceneController;
            NotifyClose(currentScene.gameObject);

            caches.Clear();
            StartCoroutine(DestoryAndLoad(currentScene, nextSceneName, firstPageName, parameters));
        }
        public void CleanLoad(string nextSceneName, GameObject firstPagePrefab, params object[] parameters) {
            SceneController currentScene = FindObjectOfType(typeof(SceneController)) as SceneController;
            NotifyClose(currentScene.gameObject);

            caches.Clear();
            StartCoroutine(DestoryAndLoad(currentScene, nextSceneName, firstPagePrefab, parameters));
        }

        private IEnumerator DestoryAndLoad(SceneController currentScene, string nextSceneName, string firstPageName, params object[] parameters) {
            GameObject.Destroy(currentScene.gameObject);
            yield return null;
            yield return Resources.UnloadUnusedAssets();
            yield return Application.LoadLevelAsync(nextSceneName);

            InvokeOnSceneLoaded(nextSceneName, false);
            NotifyOpen(GameObject.Find(nextSceneName), firstPageName, parameters);
        }

        private IEnumerator DestoryAndLoad(SceneController currentScene, string nextSceneName, GameObject firstPagePrefab, params object[] parameters) {
            GameObject.Destroy(currentScene.gameObject);
            yield return null;
            yield return Resources.UnloadUnusedAssets();
            yield return Application.LoadLevelAsync(nextSceneName);

            InvokeOnSceneLoaded(nextSceneName, false);
            NotifyOpen(GameObject.Find(nextSceneName), firstPagePrefab, parameters);
        }

        private void InvokeOnSceneLoaded(string sceneName, bool loadedFromCache) {
            if(this.OnSceneLoaded != null)
                this.OnSceneLoaded(sceneName, loadedFromCache);
        }

        private static void NotifyOpen(GameObject target, string firstPageName, params object[] parameters) {
            ViewLoader.ViewLoadType type = ViewLoader.ViewLoadType.ByName;
            target.SendMessage("Open", new object[] { type , firstPageName, parameters });
        }

        private static void NotifyOpen(GameObject target, GameObject firstPagePrefab, params object[] parameters) {
            ViewLoader.ViewLoadType type = ViewLoader.ViewLoadType.ByPrefab;
            target.SendMessage("Open", new object[] { type , firstPagePrefab, parameters });
        }

        private static void NotifyClose(GameObject target) {
            target.SendMessage("Close");
        }
	
        private IEnumerator WaitOneFrame(Action callback) {
            yield return null;
            if(callback != null)
                callback();
        }

        public void ClearCaches(params string[] sceneNames) {
            if (sceneNames == null) {
                foreach (SceneController scene in caches.Values) {
                    if(scene.gameObject.activeSelf)
                        continue;
                    Destroy(scene.gameObject);
                }
                caches.Clear();
                return;
            }

            foreach (string sceneName in sceneNames) {
                SceneController scene = null;
                caches.TryGetValue(sceneName, out scene);
                if (scene == null || scene.gameObject.activeSelf)
                    continue;

                Destroy(scene.gameObject);
                caches.Remove(sceneName);
            }
        }

        private Dictionary<string, SceneController> caches = new Dictionary<string, SceneController>();
        public event SceneLoaded OnSceneLoaded;
    }
