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
        [Header("是否Awake时就触发播放")]
        public bool PlayOnAwake = true;

        /// <summary>
        /// 缓动类型
        /// </summary>
        [Header("缓动类型")]
        public EasingFunction.Ease EaseType = EasingFunction.Ease.Linear;

        /// <summary>
        /// 循环类型
        /// </summary>
        [Header("循环类型")]
        public LoopType LoopType = LoopType.None;

        /// <summary>
        /// 循环次数(-1表示无限次)
        /// </summary>
        [Header("循环次数(-1表示无限次)")]
        public int LoopTimes;

        /// <summary>
        /// 开始播放延迟时长
        /// </summary>
        [Header("播放延迟时长")]
        [Range(0.0f, 10.0f)]
        public float StartDelayTime = 0.0f;

        /// <summary>
        /// Lerp时长
        /// </summary>
        [Header("动画时长")]
        [Range(0.0f, 20.0f)]
        public float LerpDurationTime = 2.0f;

        /// <summary>
        /// 是否等待执行
        /// </summary>
        public bool IsWaitStart
        {
            get
            {
                return mAnimState == AnimState.WaitStart;
            }
        }

        /// <summary>
        /// 是否执行中
        /// </summary>
        public bool IsExecuting
        {
            get
            {
                return mAnimState == AnimState.Executing;
            }
        }

        /// <summary>
        /// 是否暂停中
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return mAnimState == AnimState.Paused;
            }
        }

        /// <summary>
        /// 是否已结束
        /// </summary>
        public bool IsEnded
        {
            get
            {
                return mAnimState == AnimState.Ended;
            }
        }

        /// <summary>
        /// 是否是循环动画
        /// </summary>
        public bool IsLoop
        {
            get
            {
                return LoopType == LoopType.Restart || LoopType == LoopType.Yoyo;
            }
        }

        /// <summary>
        /// 是否已经开始
        /// </summary>
        protected bool mIsStart = false;

        /// <summary>
        /// 动画经历时长
        /// </summary>
        protected float mAnimTimePassed;

        /// <summary>
        /// 当前动画状态
        /// </summary>
        protected AnimState mAnimState = AnimState.WaitStart;

        /// <summary>
        /// 已经循环的次数
        /// </summary>
        protected int mLoopedTimes;

        /// <summary>
        /// 是否在反向插值动画
        /// </summary>
        protected bool mIsReversePlay = false;

        /// <summary>
        /// 插值动画开始回调
        /// Note:
        /// 循环动画仅第一次回调
        /// </summary>
        private event Action<TBaseAnimation> mOnLerpAnimBeginEvent;

        /// <summary>
        /// 插值动画结束回调
        /// Note:
        /// 循环动画需要等待所有动画次数播放完成后才回调
        /// </summary>
        private event Action<TBaseAnimation> mOnLerpAnimEndEvent;

        protected void Awake()
        {
            if (PlayOnAwake)
            {
                StartAnim();
            }
        }

        public void Update()
        {
            if(IsWaitStart || IsEnded)
            {
                return;
            }
            if(!IsAvalibleToPlay())
            {
                return;
            }
            if(IsExecuting)
            {
                mAnimTimePassed += Time.deltaTime;
                if(!mIsStart)
                {
                    if(mAnimTimePassed >= StartDelayTime)
                    {
                        DoStartAnim();
                    }
                }
                else
                {
                    DoLerp(0, LerpDurationTime, mAnimTimePassed);
                    if(mAnimTimePassed >= LerpDurationTime)
                    {
                        OnSingleLerpAnimEnd();
                    }
                }
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
            if (IsAvalibleToPlay())
            {
                if (IsExecuting || IsPaused)
                {
                    Debug.Log("插值动画正在进行中，强制打断重新开始!");
                    StopAnim();
                }
                mOnLerpAnimBeginEvent = onlerpanimbegincb;
                mOnLerpAnimEndEvent = onlerpanimendcb;
                mAnimState = AnimState.Executing;
                if(Mathf.Approximately(StartDelayTime, float.Epsilon))
                {
                    DoStartAnim();
                }
            }
            else
            {
                Debug.LogError($"实体对象:{gameObject.name}的动画组件:{GetType().Name}处于未激活状态，播放失败!");
            }
        }

        /// <summary>
        /// 暂停动画
        /// </summary>
        public void PauseAnim()
        {
            if(IsExecuting)
            {
                OnPauseAnim();
            }
        }

        /// <summary>
        /// 继续动画
        /// </summary>
        public void ResumeAnim()
        {
            if(IsPaused)
            {
                OnResumeAnim();
            }
        }

        /// <summary>
        /// 结束动画
        /// </summary>
        public void StopAnim()
        {
            if(IsExecuting || IsPaused)
            {
                // 强制结束也算结束，调用是为了确保外部逻辑正常进行
                OnLerpAnimEnd();
            }
        }

        /// <summary>
        /// 是否可播放
        /// </summary>
        /// <returns></returns>
        public bool IsAvalibleToPlay()
        {
            if (gameObject.activeInHierarchy && enabled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 循环动画是否结束
        /// </summary>
        /// <returns></returns>
        protected bool IsLoopEnd()
        {
            if(!IsLoop)
            {
                return true;
            }
            if(LoopTimes == -1)
            {
                return false;
            }
            if(LoopType == LoopType.Yoyo)
            {
                if(mIsReversePlay)
                {
                    return false;
                }
            }
            return mLoopedTimes >= LoopTimes;
        }

        /// <summary>
        /// 响应差值动画开始
        /// </summary>
        protected virtual void OnLerpAnimStart()
        {
            mIsStart = true;
            mIsReversePlay = false;
            mLoopedTimes = 0;
            mAnimTimePassed = 0f;
            mOnLerpAnimBeginEvent?.Invoke(this);
            mOnLerpAnimBeginEvent = null;
        }

        /// <summary>
        /// 响应动画暂停
        /// </summary>
        protected virtual void OnPauseAnim()
        {
            if(!IsExecuting)
            {
                Debug.LogError($"实体对象:{gameObject.name}的动画组件:{GetType().Name}处于运行中，不应该进入暂停动画流程，请检查代码！");
                return;
            }
            mAnimState = AnimState.Paused;
        }

        /// <summary>
        /// 响应动画继续
        /// </summary>
        protected virtual void OnResumeAnim()
        {
            if (!IsPaused)
            {
                Debug.LogError($"实体对象:{gameObject.name}的动画组件:{GetType().Name}处于暂停中，不应该进入继续动画流程，请检查代码！");
                return;
            }
            mAnimState = AnimState.Executing;
        }

        /// <summary>
        /// 差值动画结束前(方便做一些自定义清理)
        /// </summary>
        protected virtual void OnBeforeLerpAnimEnd()
        {

        }


        /// <summary>
        /// 单次插值动画结束
        /// </summary>
        protected virtual void OnSingleLerpAnimEnd()
        {
            mAnimTimePassed = 0f;
            if(LoopType == LoopType.Restart)
            {
                mLoopedTimes++;
                if(IsLoopEnd())
                {
                    OnLerpAnimEnd();
                }
            }
            else if(LoopType == LoopType.Yoyo)
            {
                if(mIsReversePlay)
                {
                    mLoopedTimes++;
                }
                mIsReversePlay = !mIsReversePlay;
                if(IsLoopEnd())
                {
                    OnLerpAnimEnd();
                }
            }
            else
            {
                OnLerpAnimEnd();
            }
        }

        /// <summary>
        /// 插值动画结束
        /// </summary>
        protected virtual void OnLerpAnimEnd()
        {
            OnBeforeLerpAnimEnd();
            mAnimState = AnimState.Ended;
            mIsStart = false;
            mIsReversePlay = false;
            mLoopedTimes = 0;
            mAnimTimePassed = 0f;
            mOnLerpAnimBeginEvent = null;
            mOnLerpAnimEndEvent?.Invoke(this);
            mOnLerpAnimEndEvent = null;
        }

        /// <summary>
        /// 执行动画开始
        /// </summary>
        /// <returns></returns>
        protected bool DoStartAnim()
        {
            if(mIsStart)
            {
                Debug.LogError($"实体对象:{gameObject.name}的动画组件:{GetType().Name}已经开始，不应该进入这里！");
                return false;
            }
            OnLerpAnimStart();
            return true;
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
            if(mIsReversePlay == false)
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