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
        /// 动画结束时是否恢复起始Alpha值
        /// </summary>
        [Tooltip("结束时恢复起始Alpha值")]
        public bool RecoverBeginValueAtEnd = false;

        /// <summary>
        /// 开始透明度
        /// </summary>
        [Tooltip("开始透明度")]
        [Range(0.0f, 1.0f)]
        public float StartAlpha = 1.0f;

        /// <summary>
        /// 结束透明度
        /// </summary>
        [Tooltip("结束透明度")]
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
        /// 插值动画结束
        /// </summary>
        protected override void OnLerpAnimEnd()
        {
            base.OnLerpAnimEnd();
            if(RecoverBeginValueAtEnd)
            {
                mCanvasGroupComponent.alpha = StartAlpha;
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
