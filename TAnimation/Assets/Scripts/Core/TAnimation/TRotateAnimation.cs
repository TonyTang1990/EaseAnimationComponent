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
        /// 起始欧拉角
        /// </summary>
        [Tooltip("起始欧拉角")]
        public Vector3 StartEulerAngles = Vector3.zero;

        /// <summary>
        /// 结束欧拉角
        /// </summary>
        [Tooltip("结束欧拉角")]
        public Vector3 EndEulerAngles = Vector3.zero;

        /// <summary>
        /// 相对坐标系
        /// </summary>
        [Tooltip("相对坐标系")]
        public Space RelativeCoordinateSystem = Space.Self;

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
            if(RelativeCoordinateSystem == Space.Self)
            {
                transform.localEulerAngles = Vector3.LerpUnclamped(StartEulerAngles, EndEulerAngles, t);
            }
            else
            {
                transform.eulerAngles = Vector3.LerpUnclamped(StartEulerAngles, EndEulerAngles, t);
            }
        }
    }
}