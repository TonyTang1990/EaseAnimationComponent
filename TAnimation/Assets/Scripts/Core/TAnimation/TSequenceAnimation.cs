/*
 * Description:             TSequenceAnimation.cs
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
    /// 选中动画信息
    /// </summary>
    [Serializable]
    public class TAnimationInfo
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        public GameObject TargetGo;

        /// <summary>
        /// 当前控制的动画组件
        /// </summary>
        public TBaseAnimation ControlAnimation;

        /// <summary>
        /// 当前控制的动画组件索引值
        /// </summary>
        public int ControlAnimationIndex;

        TAnimationInfo()
        {
            TargetGo = null;
            ControlAnimation = null;
            ControlAnimationIndex = -1;
        }

        TAnimationInfo(GameObject targetgo, TBaseAnimation controlanimation = null)
        {
            TargetGo = targetgo;
            ControlAnimation = controlanimation;
            ControlAnimationIndex = -1;
        }
    }

    /// <summary>
    /// TAnimationGroup.cs
    /// 序列动画
    /// Note:
    /// 序列动画里的动画组件不能设置循环动画
    /// </summary>
    public class TSequenceAnimation : MonoBehaviour
    {
        /// <summary>
        /// 序列动画类型
        /// </summary>
        public enum ESequenceType
        {
            Paral = 1,          // 并发
            Linear,             // 线性
        }

        /// <summary>
        /// 是否Awake时触发播放
        /// </summary>
        [Header("是否Awake时触发播放")]
        public bool PlayAllOnAwake = true;

        /// <summary>
        /// 序列动画类型
        /// </summary>
        [Header("序列类型")]
        public ESequenceType SequenceType = ESequenceType.Paral;

        /// <summary>
        /// 是否是循环动画
        /// </summary>
        [Header("是否是循环动画")]
        public bool IsLoop;

        /// <summary>
        /// 动画列表
        /// </summary>
        public List<TAnimationInfo> AnimationInfoList = new List<TAnimationInfo>();

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
        /// 当前序列动画状态
        /// </summary>
        protected AnimState mAnimState = AnimState.WaitStart;

        /// <summary>
        /// 并发序列动画等待完成的动画列表
        /// </summary>
        private List<TBaseAnimation> mParalWailtedCompletedAnimationList = new List<TBaseAnimation>();

        /// <summary>
        /// 当前线性正在播放的动画
        /// </summary>
        private TBaseAnimation mCurrentLinearPlayAnimation;

        /// <summary>
        /// 线性序列动画下一个播放索引
        /// </summary>
        private int mLinearPlayAnimationNextIndex = 0;

        /// <summary>
        /// 插值序列动画开始回调
        /// Note:
        /// 循环动画仅第一次回调
        /// </summary>
        private event Action<TSequenceAnimation> mOnSequenceAnimBeginEvent;

        /// <summary>
        /// 插值序列动画结束回调
        /// Note:
        /// 循环动画仅第一次完成回调
        /// </summary>
        private event Action<TSequenceAnimation> mOnSequenceAnimEndEvent;

        protected void Awake()
        {
            if (PlayAllOnAwake)
            {
                StartAllAnim();
            }
        }

        /// <summary>
        /// 开始所有动画(强制所有动画重新播放)
        /// <param name="onsequenceanimendcb"/>插值序列动画结束回调</param>
        /// <param name="onsequenceanimbegincb"/>插值序列动画开始回调</param>
        /// </summary>
        public void StartAllAnim(Action<TSequenceAnimation> onsequenceanimendcb = null, Action<TSequenceAnimation> onsequenceanimbegincb = null)
        {
            if (IsAvalibleToPlay())
            {
                TurnOffAllLoopAnims();
                if (mAnimState == AnimState.Executing)
                {
                    Debug.Log("序列动画正在进行中，强制打断重新开始!");
                    ForceStopAnims();
                }
                mOnSequenceAnimBeginEvent = onsequenceanimbegincb;
                mOnSequenceAnimEndEvent = onsequenceanimendcb;
                if (SequenceType == ESequenceType.Paral)
                {
                    mAnimState = AnimState.Executing;
                    mParalWailtedCompletedAnimationList.Clear();
                    for (int i = 0, length = AnimationInfoList.Count; i < length; i++)
                    {
                        if (AnimationInfoList[i].ControlAnimation != null)
                        {
                            if (AnimationInfoList[i].ControlAnimation.IsAvalibleToPlay())
                            {
                                mParalWailtedCompletedAnimationList.Add(AnimationInfoList[i].ControlAnimation);
                                AnimationInfoList[i].ControlAnimation?.StartAnim(OnParalAnimationPlayCompleted);
                            }
                            else
                            {
                                Debug.LogWarning($"动画:{AnimationInfoList[i].ControlAnimation.GetType().Name}不符合播放条件，无法通过序列组动画对象:{gameObject.name}并发播放!");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"序列动画索引:{i}的动画组件为空，无法并发播放!");
                        }
                    }
                    //没有有效动画的话，直接触发完成
                    if(mParalWailtedCompletedAnimationList.Count == 0)
                    {
                        OnPlaySequenceAnimationEnd();
                    }
                }
                else if(SequenceType == ESequenceType.Linear)
                {
                    mAnimState = AnimState.Executing;
                    mLinearPlayAnimationNextIndex = 0;
                    PlayNextLinearAnimation();
                }
                else
                {
                    Debug.LogError($"不支持的序列动画类型:{SequenceType}");
                }
            }
            else
            {
                Debug.LogError($"当前脚本:{GetType().Name}或所在对象:{gameObject.name}处于未激活状态，播放所有失败!");
            }
        }

        /// <summary>
        /// 关闭所有循环子动画
        /// </summary>
        protected void TurnOffAllLoopAnims()
        {
            for(int i = 0, length = AnimationInfoList.Count; i < length; i++)
            {
                var animationInfo = AnimationInfoList[i];
                if(animationInfo != null && animationInfo.ControlAnimation != null &&
                    animationInfo.ControlAnimation.IsLoop)
                {
                    Debug.LogWarning($"TSequenceAnimation里包含循环类型自动组件：{animationInfo.ControlAnimation.GetType().Name},自动修改成非循环类型动画！");
                    animationInfo.ControlAnimation.LoopType = LoopType.None;
                }
            }
        }

        /// <summary>
        /// 暂停所有动画
        /// </summary>
        public void PauseAllAnim()
        {
            if (IsExecuting)
            {
                OnPauseAllAnim();
            }
        }

        /// <summary>
        /// 继续所有动画
        /// </summary>
        public void ResumeAllAnim()
        {
            if (IsPaused)
            {
                OnResumeAllAnim();
            }
        }

        /// <summary>
        /// 结束所有动画
        /// </summary>
        public void StopAllAnim()
        {
            if (IsExecuting || IsPaused)
            {
                // 强制线性播放到最后一个，强制结束
                mLinearPlayAnimationNextIndex = AnimationInfoList.Count;
                if (mCurrentLinearPlayAnimation != null)
                {
                    mCurrentLinearPlayAnimation.ForceStopAnim();
                }
                mCurrentLinearPlayAnimation = null;
                foreach (var waitedcompletedanimation in mParalWailtedCompletedAnimationList.ToArray())
                {
                    waitedcompletedanimation.ForceStopAnim();
                }
                mParalWailtedCompletedAnimationList.Clear();

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
        /// 响应所有动画暂停
        /// </summary>
        protected virtual void OnPauseAllAnim()
        {
            if (!IsExecuting)
            {
                Debug.LogError($"实体对象:{gameObject.name}的动画组件:{GetType().Name}处于运行中，不应该进入暂停动画流程，请检查代码！");
                return;
            }
            mAnimState = AnimState.Paused;
            if (SequenceType == ESequenceType.Paral)
            {
                foreach (var waitedCompletedAnimation in mParalWailtedCompletedAnimationList.ToArray())
                {
                    waitedCompletedAnimation?.PauseAnim();
                }
            }
            else if (SequenceType == ESequenceType.Linear)
            {
                mCurrentLinearPlayAnimation?.PauseAnim();
            }
        }

        /// <summary>
        /// 响应所有动画继续
        /// </summary>
        protected virtual void OnResumeAllAnim()
        {
            if (!IsPaused)
            {
                Debug.LogError($"实体对象:{gameObject.name}的动画组件:{GetType().Name}处于暂停中，不应该进入继续动画流程，请检查代码！");
                return;
            }
            mAnimState = AnimState.Executing;
            if (SequenceType == ESequenceType.Paral)
            {
                foreach (var waitedCompletedAnimation in mParalWailtedCompletedAnimationList.ToArray())
                {
                    waitedCompletedAnimation?.ResumeAnim();
                }
            }
            else if (SequenceType == ESequenceType.Linear)
            {
                mCurrentLinearPlayAnimation?.ResumeAnim();
            }
        }

        /// <summary>
        /// 序列动画播放完成
        /// </summary>
        private void OnPlaySequenceAnimationEnd()
        {
            Debug.Log($"序列动画对象:{gameObject.name}动画播放完成!");
            mAnimState = AnimState.Ended;
            mOnSequenceAnimEndEvent?.Invoke(this);
            mOnSequenceAnimEndEvent = null;
            if(IsLoop)
            {
                StartAllAnim();
            }
        }

        /// <summary>
        /// 响应并发序列动画单个动画播放完成
        /// </summary>
        /// <param name="tanim"></param>
        private void OnParalAnimationPlayCompleted(TBaseAnimation tanim)
        {
            mParalWailtedCompletedAnimationList.Remove(tanim);
            Debug.Log($"并发序列-动画:{tanim.GetType().Name}播放完成,剩余等待完成动画数量:{mParalWailtedCompletedAnimationList.Count}!");
            if(mParalWailtedCompletedAnimationList.Count == 0)
            {
                OnPlaySequenceAnimationEnd();
            }
        }

        /// <summary>
        /// 响应线性序列动画单个动画播放完成
        /// </summary>
        /// <param name="tanim"></param>
        private void OnLinearAnimationPlayCompleted(TBaseAnimation tanim)
        {
            Debug.Log($"线性序列-动画:{tanim.GetType().Name}播放完成,下一个等待播放动画索引:{mLinearPlayAnimationNextIndex}!");
            mCurrentLinearPlayAnimation = null;
            PlayNextLinearAnimation();
        }

        /// <summary>
        /// 播放下一个线性动画
        /// </summary>
        private void PlayNextLinearAnimation()
        {
            if(mLinearPlayAnimationNextIndex < AnimationInfoList.Count)
            {
                for(int i = mLinearPlayAnimationNextIndex, length = AnimationInfoList.Count; i < length; i++)
                {
                    if(AnimationInfoList[mLinearPlayAnimationNextIndex].ControlAnimation != null)
                    {
                        if (AnimationInfoList[mLinearPlayAnimationNextIndex].ControlAnimation.IsAvalibleToPlay())
                        {
                            mLinearPlayAnimationNextIndex = i + 1;
                            mCurrentLinearPlayAnimation = AnimationInfoList[i].ControlAnimation;
                            break;
                        }
                        else
                        {
                            Debug.LogWarning($"动画:{AnimationInfoList[i].ControlAnimation.GetType().Name}不符合播放条件，无法通过序列组动画对象:{gameObject.name}线性播放!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"序列动画索引:{i}的动画组件为空，无法线性播放!");
                    }
                }
                if(mCurrentLinearPlayAnimation != null)
                {
                    mCurrentLinearPlayAnimation.StartAnim(OnLinearAnimationPlayCompleted);
                }
                else
                {
                    OnPlaySequenceAnimationEnd();
                }
            }
            else
            {
                OnPlaySequenceAnimationEnd();
            }
        }
    }
}