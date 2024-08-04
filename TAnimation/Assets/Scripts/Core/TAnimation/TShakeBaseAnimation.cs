/*
 * Description:             TShakeBaseAnimation.cs
 * Author:                  TANGHUAN
 * Create Date:             2020/06/03
 */

using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// 抖动插值动画基类
    /// </summary>
    public abstract class TShakeBaseAnimation : TBaseAnimation
    {
        /// <summary>
        /// 抖动强度(x,y,z分别代表三个轴的强度)
        /// </summary>
        [Header("抖动强度(x,y,z分别代表三个轴的强度)")]
        public Vector3 ShakeStrength = Vector3.zero;
        
        /// <summary>
        /// 偏移
        /// </summary>
        private Vector3 mOffset = Vector3.zero;

        /// <summary>
        /// 插值动画结束
        /// </summary>
        protected override void OnLerpAnimEnd()
        {
            base.OnLerpAnimEnd();
            Restore();
        }

        /// <summary>
        /// 执行真实插值动画效果
        /// </summary>
        /// <param name="t">缓动进度(0-1)</param>
        protected override void DoLerpAnim(float t)
        {
            mOffset.x = Random.Range(-1.0f, 1.0f) * ShakeStrength.x * t;
            mOffset.y = Random.Range(-1.0f, 1.0f) * ShakeStrength.y * t;
            mOffset.z = Random.Range(-1.0f, 1.0f) * ShakeStrength.z * t;
            OnShake(mOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        protected abstract void OnShake(Vector3 offset);

        /// <summary>
        /// 结束时恢复
        /// </summary>
        protected abstract void Restore();
    }
}