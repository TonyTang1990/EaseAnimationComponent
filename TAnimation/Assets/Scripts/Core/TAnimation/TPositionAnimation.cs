/*
 * Description:             TPositionAnimation.cs
 * Author:                  TONYTANG
 * Create Date:             2020/05/31
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// TPositionAnimation.cs
    /// 位移插值动画
    /// </summary>
    public class TPositionAnimation : TBaseAnimation
    {
        /// <summary>
        /// 起始本地位置
        /// </summary>
        public Vector3 StartLocalPos = Vector3.zero;

        /// <summary>
        /// 结束本地位置
        /// </summary>
        public Vector3 EndLocalPos = Vector3.zero;

        /// <summary>
        /// 响应插值开始(执行插值动画准备工作)
        /// </summary>
        protected override void OnLerpAnimStart()
        {
            base.OnLerpAnimStart();
        }

        /// <summary>
        /// 执行真实插值动画效果
        /// </summary>
        /// <param name="t">缓动进度(0-1)</param>
        protected override void DoLerpAnim(float t)
        {
            transform.localPosition = Vector3.LerpUnclamped(StartLocalPos, EndLocalPos, t);
        }
    }
}