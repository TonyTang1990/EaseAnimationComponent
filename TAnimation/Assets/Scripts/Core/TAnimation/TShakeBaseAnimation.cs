/*
 * Description:             TShakeBaseAnimation.cs
 * Author:                  TANGHUAN
 * Create Date:             2020/06/03
 */

using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// ������ֵ��������
    /// </summary>
    public abstract class TShakeBaseAnimation : TBaseAnimation
    {
        /// <summary>
        /// ����ǿ��(x,y,z�ֱ�����������ǿ��)
        /// </summary>
        [Header("����ǿ��(x,y,z�ֱ�����������ǿ��)")]
        public Vector3 ShakeStrength = Vector3.zero;
        
        /// <summary>
        /// ƫ��
        /// </summary>
        private Vector3 mOffset = Vector3.zero;

        /// <summary>
        /// ��ֵ��������
        /// </summary>
        protected override void OnLerpAnimEnd()
        {
            base.OnLerpAnimEnd();
            Restore();
        }

        /// <summary>
        /// ִ����ʵ��ֵ����Ч��
        /// </summary>
        /// <param name="t">��������(0-1)</param>
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
        /// ����ʱ�ָ�
        /// </summary>
        protected abstract void Restore();
    }
}