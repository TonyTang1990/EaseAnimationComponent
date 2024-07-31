/*
* Description:             TAnimationGroupInspector.cs
* Author:                  TONYTANG
* Create Date:             2024/07/31
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// TBaseAnimationEditor.cs
    /// TBaseAnimation的Inspector扩展
    /// </summary>
    [CustomEditor(typeof(TBaseAnimation), true)]
    public class TBaseAnimationEditor : Editor
    {
        /// <summary>
        /// 目标组件
        /// </summary>
        private TBaseAnimation mTarget;

        void OnEnable()
        {
            UnRegisterEditorUpdate();
            RegisterEditorUpadate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            UnRegisterEditorUpdate();
        }

        /// <summary>
        /// 初始化目标组件
        /// </summary>
        private void InitTarget()
        {
            mTarget ??= (mTarget as TBaseAnimation);
        }

        public override void OnInspectorGUI()
        {
            InitTarget();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("播放", GUILayout.ExpandWidth(true)))
            {
                mTarget?.StartAnim();
            }
            if (GUILayout.Button("暂停", GUILayout.ExpandWidth(true)))
            {
                mTarget?.PauseAnim();
            }
            if (GUILayout.Button("继续", GUILayout.ExpandWidth(true)))
            {
                mTarget?.ResumeAnim();
            }
            if (GUILayout.Button("停止", GUILayout.ExpandWidth(true)))
            {
                mTarget?.StopAnim();
            }
            EditorGUILayout.EndHorizontal();
            base.OnInspectorGUI();
            // 确保对SerializedObject和SerializedProperty的数据修改每帧同步
            serializedObject.Update();

            // 确保对SeralizedObject和SerializedProperty的数据修改写入生效
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 注入Editor Update
        /// </summary>
        private void RegisterEditorUpdate()
        {
            EditorApplication.update += EditorUpdate;
        }

        /// <summary>
        /// 取消Editor Update注入
        /// </summary>
        private void UnregisterEditorUpdate()
        {
            EditorApplication.update -= EditorUpdate;
        }

        /// <summary>
        /// Editor更新
        /// </summary>
        private void EditorUpdate()
        {
            mTarget?.Update();
        }
    }
}
