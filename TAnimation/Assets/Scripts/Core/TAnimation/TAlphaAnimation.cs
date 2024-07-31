/*
 * Description:             TAlphaAnimation.cs
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
    /// TAlphaAnimation.cs
    /// 透明度插值动画
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class TAlphaAnimation : TBaseAnimation
    {
        /// <summary>
        /// 开始透明度
        /// </summary>
        [Header("开始透明度")]
        [Range(0.0f, 1.0f)]
        public float StartAlpha = 1.0f;

        /// <summary>
        /// 结束透明度
        /// </summary>
        [Header("结束透明度")]
        [Range(0.0f, 1.0f)]
        public float EndAlpha = 0.0f;

        /// <summary>
        /// CanvasGroup组件
        /// </summary>
        protected CanvasGroup mCanvasGroupComponent;

        /// <summary>
        /// 响应插值动画开始
        /// </summary>
        protected override void OnLerpAnimStart()
        {
            base.OnLerpAnimStart();
            if(mCanvasGroupComponent == null)
            {
                mCanvasGroupComponent = GetComponent<CanvasGroup>();
            }
        }

        /// <summary>
        /// 执行真实插值动画效果
        /// </summary>
        /// <param name="t">缓动进度(0-1)</param>
        protected override void DoLerpAnim(float t)
        {
            var newalpha = Mathf.Lerp(StartAlpha, EndAlpha, t);
            mCanvasGroupComponent.alpha = newalpha;
        }
    }
}
