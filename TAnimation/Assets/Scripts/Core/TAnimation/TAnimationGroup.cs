/*
 * Description:             TAnimationGroup.cs
 * Author:                  TONYTANG
 * Create Date:             2020/05/31
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// TAnimationGroup.cs
    /// 动画组
    /// </summary>
    public class TAnimationGroup : MonoBehaviour
    {
        /// <summary>
        /// 动画组列表
        /// </summary>
        public List<TBaseAnimation> AnimationList = new List<TBaseAnimation>();
        
        /// <summary>
        /// 开始指定动画索引(强制动画重新播放)
        /// </summary>
        /// <param name="index"></param>
        public void StartAnim(int index)
        {
            Debug.Assert(index >= AnimationList.Count, $"动画索引值:{index}超出最大动画数量:{AnimationList.Count},播放指定动画索引失败!");
        }

        /// <summary>
        /// 开始所有动画(强制所有动画重新播放)
        /// </summary>
        public void StartAllAnim()
        {
            //foreach
        }

        /// <summary>
        /// 停止指定动画索引
        /// </summary>
        /// <param name="index"></param>
        public void StopAnim(int index)
        {
            Debug.Assert(index >= AnimationList.Count, $"动画索引值:{index}超出最大动画数量:{AnimationList.Count},停止指定动画索引失败!");

        }

        /// <summary>
        /// 停止所有动画
        /// </summary>
        public void StopAllAnim()
        {

        }
    }
}