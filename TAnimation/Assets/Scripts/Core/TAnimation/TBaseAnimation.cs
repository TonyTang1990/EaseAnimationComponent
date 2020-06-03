/*
 * Description:             TBaseAnimation.cs
 * Author:                  TONYTANG
 * Create Date:             2020/05/31
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// TBaseAnimation.cs
    /// 插值动画基类
    /// </summary>
    public class TBaseAnimation : MonoBehaviour
    {
        /// <summary>
        /// 是否Awake时就触发播放
        /// </summary>
        [Tooltip("是否Awake时就触发播放")]
        public bool PlayOnAwake = true;

        /// <summary>
        /// 缓动类型
        /// </summary>
        [Tooltip("缓动类型")]
        public EasingFunction.Ease EaseType = EasingFunction.Ease.Linear;

        /// <summary>
        /// 开始播放延迟时长
        /// </summary>
        [Tooltip("播放延迟时长")]
        [Range(0.0f, 10.0f)]
        public float StartDelayTime = 0.0f;

        /// <summary>
        /// Lerp时长
        /// </summary>
        [Tooltip("动画时长")]
        [Range(0.0f, 20.0f)]
        public float LerpDurationTime = 2.0f;

        /// <summary>
        /// 是否反向插值动画
        /// </summary>
        [Tooltip("是否反向插值动画")]
        public bool IsReverse = false;

        /// <summary>
        /// 当前动画状态
        /// </summary>
        protected EAnimState mAnimState = EAnimState.WaitStart;

        /// <summary>
        /// 延时播放携程
        /// </summary>
        protected Coroutine mDelayStartCoroutine;

        /// <summary>
        /// 插值动画携程
        /// </summary>
        protected Coroutine mAnimCoroutine;

        /// <summary>
        /// 插值动画开始回调
        /// </summary>
        protected event Action<TBaseAnimation> mOnLerpAnimBeginEvent;

        /// <summary>
        /// 插值动画结束回调
        /// </summary>
        protected event Action<TBaseAnimation> mOnLerpAnimEndEvent;

        protected void Awake()
        {
            if (PlayOnAwake)
            {
                StartAnim();
            }
        }

        protected void OnDestroy()
        {

        }

        /// <summary>
        /// 开始插值动画
        /// <param name="onlerpanimendcb"/>插值动画结束回调</param>
        /// <param name="onlerpanimbegincb"/>插值动画开始回调</param>
        /// </summary>
        public void StartAnim(Action<TBaseAnimation> onlerpanimendcb = null, Action<TBaseAnimation> onlerpanimbegincb = null)
        {
            if (gameObject.activeInHierarchy && enabled)
            {
                if (mAnimState == EAnimState.WaitStart || mAnimState == EAnimState.Ended)
                {
                    mOnLerpAnimBeginEvent = onlerpanimbegincb;
                    mOnLerpAnimEndEvent = onlerpanimendcb;
                    mAnimState = EAnimState.Executing;
                    if(Mathf.Approximately(StartDelayTime, float.Epsilon))
                    {
                        mAnimCoroutine = StartCoroutine(LerpCoroutine());
                    }
                    else
                    {
                        mDelayStartCoroutine = StartCoroutine(DelayStartCoroutine());
                    }
                }
                else
                {
                    Debug.LogError("插值动画正在进行中，请等待执行结束后再重新开始!");
                }
            }
            else
            {
                Debug.LogError($"当前脚本:{GetType().Name}或所在对象:{gameObject.name}处于未激活状态，播放失败!");
            }
        }

        /// <summary>
        /// 是否可播放
        /// </summary>
        /// <returns></returns>
        public bool IsAvalibleToPlay()
        {
            if(gameObject.activeInHierarchy && enabled && (mAnimState == EAnimState.WaitStart || mAnimState == EAnimState.Ended))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 响应插值动画开始
        /// </summary>
        protected virtual void OnLerpAnimStart()
        {

        }

        /// <summary>
        /// 插值动画结束
        /// </summary>
        protected virtual void OnLerpAnimEnd()
        {

        }

        /// <summary>
        /// 响应插值动画开始
        /// </summary>
        /// <returns></returns>
        protected IEnumerator DelayStartCoroutine()
        {
            yield return new WaitForSeconds(StartDelayTime);
            mDelayStartCoroutine = null;
            mAnimCoroutine = StartCoroutine(LerpCoroutine());
        }

        /// <summary>
        /// 插值携程
        /// </summary>
        /// <returns></returns>
        protected IEnumerator LerpCoroutine()
        {
            OnLerpAnimStart();
            mOnLerpAnimBeginEvent?.Invoke(this);
            mOnLerpAnimBeginEvent = null;
            var starttime = Time.time;
            var endtime = starttime + LerpDurationTime;
            while (Time.time < endtime)
            {
                DoLerp(starttime, endtime, Time.time);
                yield return null;
            }
            DoLerp(starttime, endtime, Time.time);
            mAnimState = EAnimState.Ended;
            OnLerpAnimEnd();
            mOnLerpAnimEndEvent?.Invoke(this);
            mOnLerpAnimEndEvent = null;
        }

        /// <summary>
        /// 执行插值
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="currenttime"></param>
        protected void DoLerp(float starttime, float endtime, float currenttime)
        {
            var t = Mathf.InverseLerp(starttime, endtime, currenttime);
            var easefunc = EasingFunction.GetEasingFunction(EaseType);
            if(IsReverse == false)
            {
                t = easefunc(0, 1, t);
            }
            else
            {
                t = easefunc(1, 0, t);
            }
            //Debug.Log($"缓动类型:{this.EaseType}开始时间:{starttime}结束时间:{endtime}当前时间:{currenttime}当前缓动值:{t}");
            DoLerpAnim(t);
        }

        /// <summary>
        /// 执行真实插值动画效果
        /// </summary>
        /// <param name="t">缓动进度(0-1)</param>
        protected virtual void DoLerpAnim(float t)
        {

        }
    }
}