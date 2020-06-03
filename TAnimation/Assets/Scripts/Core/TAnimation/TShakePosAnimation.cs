/*
 * Description:             TShakePosAnimation.cs
 * Author:                  TANGHUAN
 * Create Date:             2020/06/03
 */

using System;
using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// 插值位置抖动动画
    /// </summary>
    public class TShakePosAnimation : TShakeBaseAnimation
    {
        /// <summary>
        /// 起始本地位置
        /// </summary>
        protected Vector3 mBeginLocalPos;

        /// <summary>
        /// 响应插值动画开始
        /// </summary>
        protected override void OnLerpAnimStart()
        {
            base.OnLerpAnimStart();
            mBeginLocalPos = transform.localPosition;
        }

        /// <summary>
        /// 晃动位置
        /// </summary>
        /// <param name="offset"></param>
        protected override void OnShake(Vector3 offset)
        {
            transform.localPosition = mBeginLocalPos + offset;
        }

        /// <summary>
        /// 结束时恢复
        /// </summary>
        protected override void Restore()
        {
            transform.localPosition = mBeginLocalPos;
        }
    }
}