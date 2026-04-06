using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

[InitializeOnLoad]
public class EditModeSceneAutoSave : MonoBehaviour
{
    private static double TickTimer;
    static EditModeSceneAutoSave()
    {
        Debug.Log("[EditModeSceneAutoSave]Script Loaded");

        TickTimer = EditorApplication.timeSinceStartup;
        EditorApplication.update += OnUpdate;

        //自動保存秒数(分)を設定ファイルから読み込み
        //例外：設定ファイル読み込みエラー, 設定値が半角数字ではない, 設定値が0以下
        var ConfigFile = "Packages/net.mt-sys.unityeditmodesceneautosave/Editor/Config.txt";
        try
        {
            using (var Reader = new StreamReader(ConfigFile))
            {
                var TimerConfigMin = int.Parse(Reader.ReadLine()) * 60;
                if (TimerConfigMin <= 0)
                {
                    throw new Exception("TimerConfigMin <= 0");
                }
                AutoSaveSec = TimerConfigMin;
            }
            Debug.Log("[EditModeSceneAutoSave]AutoSaveSec -> " + AutoSaveSec);
        }
        catch
        {
            Debug.Log("[EditModeSceneAutoSave]Timer ERROR: Default AutoSaveSec -> " + AutoSaveSec);
        }
    }

    private static int TimerCountSec = 0;
    private static int AutoSaveSec = 300; // デフォルト値(設定ファイル取得エラー時用)


    private static void OnUpdate()
    {
        if (EditorApplication.timeSinceStartup - TickTimer >= 1.0)
        {
            //Editモードの場合のみカウントアップ、他モードに入るとカウンターリセット
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPaused)
            {
                TimerCountSec++;
                if (TimerCountSec >= AutoSaveSec)
                {
                    Debug.Log("[EditModeSceneAutoSave]Execute AutoSave");
                    TimerCountSec = 0;

                    //現在のシーンを上書き保存
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }
            }
            else
            {
                TimerCountSec = 0;
            }
            //Debug.Log("[EditModeSceneAutoSave]TimerSec " + TimerCountSec);
            TickTimer = EditorApplication.timeSinceStartup;
        }
    }
}
