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
        [Tooltip("是否Awake时触发播放")]
        public bool PlayAllOnAwake = true;

        /// <summary>
        /// 序列动画类型
        /// </summary>
        [Tooltip("序列类型")]
        public ESequenceType SequenceType = ESequenceType.Paral;

        /// <summary>
        /// 动画列表
        /// </summary>
        public List<TAnimationInfo> AnimationInfoList = new List<TAnimationInfo>();

        /// <summary>
        /// 当前序列动画状态
        /// </summary>
        protected EAnimState mAnimState = EAnimState.WaitStart;

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
        /// </summary>
        private event Action<TSequenceAnimation> mOnSequenceAnimBeginEvent;

        /// <summary>
        /// 插值序列动画结束回调
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
                if (mAnimState == EAnimState.Executing)
                {
                    Debug.Log("序列动画正在进行中，强制打断重新开始!");
                    ForceStopAnims();
                }
                mOnSequenceAnimBeginEvent = onsequenceanimbegincb;
                mOnSequenceAnimEndEvent = onsequenceanimendcb;
                if (SequenceType == ESequenceType.Paral)
                {
                    mAnimState = EAnimState.Executing;
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
                    mAnimState = EAnimState.Executing;
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
        /// 强制打断当前序列动画
        /// </summary>
        public void ForceStopAnims()
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
            mOnSequenceAnimBeginEvent = null;
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
        /// 序列动画播放完成
        /// </summary>
        private void OnPlaySequenceAnimationEnd()
        {
            Debug.Log($"序列动画对象:{gameObject.name}动画播放完成!");
            mAnimState = EAnimState.Ended;
            mOnSequenceAnimEndEvent?.Invoke(this);
            mOnSequenceAnimEndEvent = null;
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