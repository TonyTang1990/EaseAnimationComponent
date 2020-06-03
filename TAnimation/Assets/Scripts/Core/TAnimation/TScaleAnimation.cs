/*
 * Description:             TScaleAnimation.cs
 * Author:                  TONYTANG
 * Create Date:             2020/05/31
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// TScaleAnimation.cs
    /// 缩放插值动画
    /// </summary>
    public class TScaleAnimation : TBaseAnimation
    {
        /// <summary>
        /// 起始缩放值
        /// </summary>
        [Tooltip("起始缩放值")]
        public Vector3 StartLocalScale;

        /// <summary>
        /// 结束缩放值
        /// </summary>
        [Tooltip("结束缩放值")]
        public Vector3 EndLocalScale = Vector3.one;

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
            transform.localScale = Vector3.LerpUnclamped(StartLocalScale, EndLocalScale, t);
        }
    }
}