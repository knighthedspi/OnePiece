using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GmcsEditor : EditorWindow
{
    //-----------------static-----------------------
    [MenuItem("Tools/GmcsEditor")]
    public static void Open()
    {
        EditorWindow.GetWindow<GmcsEditor>(false, "GmcsEditor");
    }

    //-----------------variable-----------------------
    private DefineData[] mDefineList;
    private Vector2      mScrollView = new Vector2();
    private bool         mIsComment = true;

    private const string RSP_PATH = "Assets/smcs.rsp";
    private const string TEMPLETE_PATH = "Assets/devconfig.template";


    //-----------------medhod-----------------------
    public void OnGUI()
    {
        //Init
        if(null == mDefineList) Init();

        //toggle
        mScrollView = EditorGUILayout.BeginScrollView(mScrollView);

        string tCategory = null;
        foreach(DefineData tDefineData in mDefineList) {
            //category
            if(tCategory != tDefineData.category) {
                if(null != tDefineData.category) {
                    GUILayout.Label("--" + tDefineData.category + "--");
                } else {
                    GUILayout.Label("---------------");
                }
                tCategory = tDefineData.category;
            }

            EditorGUILayout.BeginHorizontal(); {


                //select
                bool nowFlg = EditorGUILayout.Toggle(tDefineData.flg, GUILayout.MaxWidth(10));
                if(nowFlg != tDefineData.flg) {
                    tDefineData.flg = nowFlg;
                    save();
                }

                //define
                EditorGUILayout.SelectableLabel(tDefineData.define, GUILayout.MaxWidth(300));

                //comment
                if(mIsComment && null != tDefineData.comment && "" != tDefineData.comment) {
                    EditorGUILayout.TextField(tDefineData.comment);
                }
            } EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndScrollView();

        if(GUILayout.Button("apply")) {
            AssetDatabase.Refresh();
        }

        if(GUILayout.Button("open rsp")) {
            System.Diagnostics.Process.Start("open", RSP_PATH);
        }

        if(GUILayout.Button("edit template")) {
            System.Diagnostics.Process.Start("open", TEMPLETE_PATH);
        }

        if(GUILayout.Button("reload")) {
            mDefineList = null;
        }

        mIsComment = EditorGUILayout.Toggle("show comment", mIsComment);
    }

    void save()
    {
        string[] tTexts = new string[mDefineList.Length];
        for(int tIndex = 0; tIndex < mDefineList.Length; ++tIndex) {
            if(mDefineList[tIndex].flg) {
                tTexts[tIndex] = mDefineList[tIndex].define;
            } else {
                tTexts[tIndex] = "";
            }
        }

        if (!File.Exists(RSP_PATH)) {
            using (var stream = File.Create(RSP_PATH));
        }
        File.WriteAllLines(RSP_PATH, tTexts);

        string dummyCSname = "Assets/Plugins/" + Path.GetFileNameWithoutExtension(RSP_PATH) + ".cs";
        if (!File.Exists(dummyCSname)) {
            using (var stream = File.Create(dummyCSname));
        }
        File.WriteAllLines(dummyCSname, new string[]{""});
    }



    private void Init()
    {
        string[] tAllLines = File.ReadAllLines(TEMPLETE_PATH);

        mDefineList  = new DefineData[tAllLines.Length];

        int tIndex = 0;
        foreach(string tLine in tAllLines) {
            string[] tSplitLine = tLine.Split(new string[]{"//"}, StringSplitOptions.None);
            mDefineList[tIndex] = new DefineData();
            mDefineList[tIndex].define = tSplitLine[0];
            if(1 < tSplitLine.Length) {
                mDefineList[tIndex].comment = tSplitLine[1];
            }
            if(2 < tSplitLine.Length) {
                mDefineList[tIndex].category = tSplitLine[2];
            }
            tIndex++;
        }

        if (File.Exists(RSP_PATH)) {
            string[] tEnableDefine = File.ReadAllLines(RSP_PATH);
            for(int i = 0; i < mDefineList.Length; ++i) {
                mDefineList[i].flg = Array.Exists<string>(tEnableDefine, (tDef) => tDef == mDefineList[i].define);
            }
        }


        Array.Sort<DefineData>(mDefineList, (x, y) => {
            int tRet;
            if(null != x.category && null != y.category) {
                tRet = x.category.CompareTo(y.category);
                if(0 != tRet) return tRet;
            } else if(null != x.category && null == y.category) {
                return -1;
            } else if(null == x.category && null != y.category) {
                return 1;
            }

            return -x.define.CompareTo(y.define);
        });
    }


    class DefineData
    {
        public string define;
        public string comment;
        public string category;
        public bool   flg;
    }
}

