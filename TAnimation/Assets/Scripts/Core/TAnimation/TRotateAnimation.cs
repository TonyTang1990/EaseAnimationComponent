/*
 * Description:             TRotateAnimation.cs
 * Author:                  TONYTANG
 * Create Date:             2020/05/31
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// TRotateAnimation.cs
    /// 旋转插值动画
    /// </summary>
    public class TRotateAnimation : TBaseAnimation
    {
        /// <summary>
        /// 动画类型
        /// </summary>
        [Header("动画类型")]
        public AnimType AnimType = AnimType.Absolute;

        /// <summary>
        /// 起始欧拉角
        /// </summary>
        [Header("起始欧拉角")]
        public Vector3 StartEulerAngles = Vector3.zero;

        /// <summary>
        /// 结束欧拉角
        /// </summary>
        [Header("结束欧拉角")]
        public Vector3 EndEulerAngles = Vector3.zero;

        /// <summary>
        /// 缓动开始局部欧拉角
        /// </summary>
        protected Vector3 mLerpStartLocalEulerAngels;

        /// <summary>
        /// 响应插值开始(执行插值动画准备工作)
        /// </summary>
        protected override void OnLerpAnimStart()
        {
            base.OnLerpAnimStart();
            mLerpStartLocalEulerAngels = transform.localEulerAngles;
        }

        /// <summary>
        /// 执行真实插值动画效果
        /// </summary>
        /// <param name="t">缓动进度(0-1)</param>
        protected override void DoLerpAnim(float t)
        {
            if(AnimType == AnimType.Relative)
            {
                var startLocalEulerAngels = mLerpStartLocalEulerAngels + StartEulerAngles;
                var endLocalEulerAngels = mLerpStartLocalEulerAngels + EndEulerAngles;
                transform.localEulerAngles = Vector3.Lerp(startLocalEulerAngels, endLocalEulerAngels, t);
            }
            else if(AnimType == AnimType.Absolute)
            {
                transform.eulerAngles = Vector3.Lerp(StartEulerAngles, EndEulerAngles, t);
            }
        }

        /// <summary>
        /// 差值动画结束前
        /// </summary>
        protected override void OnBeforeLerpAnimEnd()
        {
            base.OnBeforeLerpAnimEnd();
            mLerpStartLocalEulerAngels = Vector3.zero;
        }
    }
}