using UnityEngine;
using System.Collections;

public class GameObjectUtility {

	public static GameObject FindChild(string name, GameObject parent){
		return parent.transform.FindChild(name).transform.gameObject;
	}

	public static GameObject FindParent(GameObject go){
		return go.transform.parent.gameObject;
	}

    public static GameObject Clone(string name, GameObject prefab){
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        go.name = name;
        return go;
    }

    public static GameObject Clone(string resourcePath){
        GameObject go = GameObject.Instantiate(Resources.Load(resourcePath, typeof(GameObject))) as GameObject;
        return go;
    }

    public static GameObject Clone(string name, string resourcePath){
        GameObject go = Clone(resourcePath);
        go.name = name;
        return go;
    }

    public static GameObject AddChild(GameObject parent){
        GameObject go = new GameObject();
        if(parent != null){
            go.transform.parent = parent.transform;
            Reset(go);
            go.layer = parent.layer;
        }
        return go;
    }

    public static GameObject AddChild(string name, GameObject parent){
        GameObject child = AddChild(parent);
        child.name = name;
        return child;
    }

	public static GameObject AddChild(string name ,GameObject parent, string resourcePath){
		GameObject child = AddChild(parent, resourcePath);
		child.name = name;
		return child;
    }
    
    public static GameObject AddChild(GameObject parent, string resourcePath){
		Transform pt = parent.transform;
        GameObject child = GameObject.Instantiate(Resources.Load(resourcePath), Vector3.zero, pt.rotation) as GameObject;
		return ChangeChildLocalToParentLocal(parent, child);
	}

	public static GameObject AddChild(string name, GameObject parent, Object prefab){
		GameObject child = AddChild(parent, prefab);
		child.name = name;
		return child;
	}

    public static GameObject AddChild(GameObject parent, Object prefab){
        Transform pt = parent.transform;
		GameObject go = GameObject.Instantiate(prefab, Vector3.zero, pt.rotation) as GameObject;
		return ChangeChildLocalToParentLocal(parent, go);
    }

	public static GameObject ChangeChildLocalToParentLocal(GameObject parent, GameObject child){
		Transform pt = parent.transform;
		child.transform.localPosition = Vector3.zero;
		Transform t = child.transform;
		t.parent = pt;
		t.localScale = Vector3.one;
		child.layer = parent.layer;
		return child;
	}

	public static GameObject AddChild(string name, GameObject parent, string resourcePath, int layer){
		GameObject child = AddChild(parent, resourcePath, layer);
		child.name = name;
		return child;
	}

	public static GameObject AddChild(GameObject parent, string resourcePath, int layer){
		GameObject child = AddChild(parent, resourcePath);
		child.layer = layer;
		return child;
	}

	public static GameObject AddChild(string name, GameObject parent, string resourcePath, int layer, string tag){
		GameObject child = AddChild(parent, resourcePath, layer, tag);
		child.name = name;
		return child;
	}

	public static GameObject AddChild(GameObject parent, string resourcePath, int layer, string tag){
		GameObject child = AddChild(parent, resourcePath);
		child.layer = layer;
		child.tag = tag;
		return child;
	}
	
	public static GameObject AddChild(GameObject parent, Object prefab, int layer){
		GameObject child = AddChild(parent, prefab);
		child.layer = layer;
		return child;
	}


    public static GameObject AddChild(string name, GameObject parent, GameObject prefab){
        GameObject child = AddChild(parent, prefab);
        child.name = name;
        return child;
    }

    public static GameObject AddChild(GameObject parent, GameObject prefab, Transform transform){
        Transform pt = parent.transform;
        GameObject child = GameObject.Instantiate(prefab, transform.position, transform.rotation) as GameObject;
        
        if(child != null && parent != null){
            Transform t = child.transform;
            t.parent = pt;
            t.localScale = transform.localScale;
            child.layer = parent.layer;
        }
        
        return child;
    }
    
    public static GameObject AddChild(string name, GameObject parent, GameObject prefab, Transform transform){
        GameObject child = AddChild(parent, prefab, transform);
        child.name = name;
        return child;
    }

    public static T AddChild<T>(GameObject parent) where T : Component {
        GameObject child = AddChild(parent);
        return child.AddComponent<T>();
    }

    public static T AddChild<T>(string name, GameObject parent) where T : Component {
        GameObject child = AddChild(name, parent);
        return child.AddComponent<T>();
    }

    public static void Reset(GameObject gameObject){
        Transform t = gameObject.transform;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    public static void CopyLocalTransform(GameObject src, GameObject dst){
        Transform srcTransform = src.transform;
        Transform dstTransform = dst.transform;
        dstTransform.localPosition = srcTransform.localPosition;
        dstTransform.localRotation = srcTransform.localRotation;
        dstTransform.localScale = srcTransform.localScale;
    }

    public static void ClearChildren(GameObject go){
        foreach (Transform t in go.transform){
            Object.Destroy(t.gameObject);
        }
        go.transform.DetachChildren();
    }

    static public void Destroy (Object obj){
        if(obj == null) return;
#if UNITY_EDITOR
        if(!Application.isPlaying){
            Object.DestroyImmediate(obj);
            return;
        }
#endif
        if (obj is GameObject){
            GameObject go = obj as GameObject;
            go.transform.parent = null;
        }
        
        Object.Destroy(obj);
    }
}