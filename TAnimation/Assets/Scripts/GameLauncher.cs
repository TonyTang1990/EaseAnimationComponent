/*
 * Description:             游戏入口
 * Author:                  tanghuan
 * Create Date:             2018/03/12
 */

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏入口
/// </summary>
public class GameLauncher : MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(this);

        VisibleLogUtility visiblelog = gameObject.AddComponent<VisibleLogUtility>();
        visiblelog.setInstance(visiblelog);
        VisibleLogUtility.getInstance().mVisibleLogSwitch = FastUIEntry.LogSwitch;
        Application.logMessageReceived += VisibleLogUtility.getInstance().HandleLog;
        DOTween.Init();
    }

    private void Start () {
        Debug.Log("GameLauncher:Start()");
        //MonoMemoryProfiler.Singleton.setMemoryProfilerType(MonoMemoryProfiler.MemoryProfilerType.CSharp_GC);
        //MonoMemoryProfiler.Singleton.beginMemorySample("int[1000]");
        //int[] intarray = new int[1000];
        //MonoMemoryProfiler.Singleton.endMemorySample();
        //TimeCounter.Singleton.Restart("for(10000000--)");
        //int i = 10000000;
        //while (i > 0)
        //{
        //    i--;
        //}
        //TimeCounter.Singleton.End();
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= VisibleLogUtility.getInstance().HandleLog;
    }
}
