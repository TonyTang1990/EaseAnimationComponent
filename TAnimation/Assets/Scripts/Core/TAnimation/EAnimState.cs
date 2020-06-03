/*
 * Description:             EAnimState.cs
 * Author:                  TONYTANG
 * Create Date:             2020/05/31
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// EAnimState.cs
    /// 插值动画状态枚举
    /// </summary>
    public enum EAnimState
    {
        WaitStart = 1,          // 等待开始
        Executing,              // 执行中
        Ended,                  // 已结束
    }
}