/*
 * Description:             AnimState.cs
 * Author:                  TONYTANG
 * Create Date:             2020/05/31
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// AnimState.cs
    /// 插值动画状态枚举
    /// </summary>
    public enum AnimState
    {
        WaitStart = 1,          // 等待开始
        Executing,              // 执行中
        Paused,                 // 暂停中
        Ended,                  // 已结束
    }
}