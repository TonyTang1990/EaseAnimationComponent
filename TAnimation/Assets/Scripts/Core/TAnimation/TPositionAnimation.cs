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
        /// 动画类型
        /// </summary>
        [Header("动画类型")]
        public AnimType AnimType = AnimType.Absolute;

        /// <summary>
        /// 起始位置
        /// </summary>
        [Header("起始位置")]
        public Vector3 StartPos = Vector3.zero;

        /// <summary>
        /// 结束位置
        /// </summary>
        [Header("结束位置")]
        public Vector3 EndPos = Vector3.zero;

        /// <summary>
        /// 缓动开始局部坐标位置
        /// </summary>
        protected Vector3 mLerpStartLocalPos;

        /// <summary>
        /// 响应插值开始(执行插值动画准备工作)
        /// </summary>
        protected override void OnLerpAnimStart()
        {
            base.OnLerpAnimStart();
            mLerpStartLocalPos = gameObject.transform.localPosition;
        }

        /// <summary>
        /// 执行真实插值动画效果
        /// </summary>
        /// <param name="t">缓动进度(0-1)</param>
        protected override void DoLerpAnim(float t)
        {
            if(AnimType == AnimType.Relative)
            {
                var startLocalPosition = mLerpStartLocalPos + StartPos;
                var endLocalPosition = mLerpStartLocalPos + EndPos;
                transform.localPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, t);
            }
            else if(AnimType == AnimType.Absolute)
            {
                transform.position = Vector3.Lerp(StartPos, EndPos, t);
            }
        }

        /// <summary>
        /// 差值动画结束前
        /// </summary>
        protected override void OnBeforeLerpAnimEnd()
        {
            base.OnBeforeLerpAnimEnd();
            mLerpStartLocalPos = Vector3.zero;
        }
    }
}