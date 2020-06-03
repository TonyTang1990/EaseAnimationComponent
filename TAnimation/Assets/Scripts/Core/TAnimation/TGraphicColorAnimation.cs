/*
 * Description:             TGraphicColorAnimation.cs
 * Author:                  TONYTANG
 * Create Date:             2020/05/31
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TAnimation
{
    /// <summary>
    /// TImageColorAnimation.cs
    /// 图像颜色插值动画
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    public class TGraphicColorAnimation : TBaseAnimation
    {
        /// <summary>
        /// 起始颜色值
        /// </summary>
        [Tooltip("起始颜色值")]
        public Color StartColor = Color.white;

        /// <summary>
        /// 结束颜色值
        /// </summary>
        [Tooltip("结束颜色值")]
        public Color EndColor = Color.white;

        /// <summary>
        /// Image组件
        /// </summary>
        protected Image mImageComponent;

        /// <summary>
        /// 响应插值开始(执行插值动画准备工作)
        /// </summary>
        protected override void OnLerpAnimStart()
        {
            base.OnLerpAnimStart();
            if(mImageComponent == null)
            {
                mImageComponent = GetComponent<Image>();
            }
        }

        /// <summary>
        /// 执行真实插值动画效果
        /// </summary>
        /// <param name="t">缓动进度(0-1)</param>
        protected override void DoLerpAnim(float t)
        {
            var newcolor = Color.Lerp(StartColor, EndColor, t);
            mImageComponent.color = newcolor;
        }
    }
}