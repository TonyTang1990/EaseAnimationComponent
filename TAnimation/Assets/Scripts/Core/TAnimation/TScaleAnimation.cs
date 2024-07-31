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
        /// 动画类型
        /// </summary>
        [Header("动画类型")]
        public AnimType AnimType = AnimType.Absolute;

        /// <summary>
        /// 起始缩放值
        /// </summary>
        [Header("起始缩放值")]
        public Vector3 StartLocalScale;

        /// <summary>
        /// 结束缩放值
        /// </summary>
        [Header("结束缩放值")]
        public Vector3 EndLocalScale = Vector3.one;

        /// <summary>
        /// 缓动开始局部缩放值
        /// </summary>
        protected Vector3 mLerpStartLocalScale;

        /// <summary>
        /// 响应插值开始(执行插值动画准备工作)
        /// </summary>
        protected override void OnLerpAnimStart()
        {
            base.OnLerpAnimStart();
            mLerpStartLocalScale = transform.localScale;
        }

        /// <summary>
        /// 执行真实插值动画效果
        /// </summary>
        /// <param name="t">缓动进度(0-1)</param>
        protected override void DoLerpAnim(float t)
        {
            if(AnimType == AnimType.Relative)
            {
                var startLocalScale = mLerpStartLocalScale + StartLocalScale;
                var endLocalScale = mLerpStartLocalScale + EndLocalScale;
                transform.localScale = Vector3.Lerp(startLocalScale, endLocalScale, t);
            }
            else if(AnimType == AnimType.Absolute)
            {
                transform.localScale = Vector3.Lerp(StartLocalScale, EndLocalScale, t);
            }
        }

        /// <summary>
        /// 差值动画结束前
        /// </summary>
        protected override void OnBeforeLerpAnimEnd()
        {
            base.OnBeforeLerpAnimEnd();
            mLerpStartLocalScale = Vector3.zero;
        }
    }
}