﻿/*
 * Description:             TAnimationGroupInspector.cs
 * Author:                  TONYTANG
 * Create Date:             2020/05/31
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAnimation
{
    /// <summary>
    /// TAnimationGroupInspector.cs
    /// 序列动画编辑器扩展
    /// </summary>
    [CustomEditor(typeof(TSequenceAnimation))]
    public class TSequenceAnimationInspector : Editor
    {
        /// <summary>
        /// 目标组件
        /// </summary>
        private TSequenceAnimation mTarget;

        /// <summary>
        /// PlayAllOnStart成员属性
        /// </summary>
        private SerializedProperty mPlayAllOnAwakeProperty;

        /// <summary>
        /// SequenceType成员属性
        /// </summary>
        private SerializedProperty mSequenceTypeProperty;

        /// <summary>
        /// AnimationInfoList成员属性
        /// </summary>
        private SerializedProperty mAnimationInfoListProperty;

        /// <summary>
        /// 当前目标对象组件
        /// </summary>
        private TSequenceAnimation mTargetTSequenceAnimation;

        /// <summary>
        /// 当前选中对象的符合条件组件数量映射Map，
        /// Key为当前对象所在索引值
        /// Value为当前对象TBaseAnimation组件挂载数量(用于判定挂载的TBaseAnimation组件数量是否变化)
        /// </summary>
        private Dictionary<int, int> mValideComponentsNumberMap = new Dictionary<int, int>();

        /// <summary>
        /// 当前选中对象符合条件的TBaseAnimation动画选择数组映射Map
        /// Key为当前动画对象索引值
        /// Value为该对象的有效插值动画组件选项
        /// </summary>
        private Dictionary<int, string[]> mValideComponentOptionMap = new Dictionary<int, string[]>();

        void OnEnable()
        {
            mPlayAllOnAwakeProperty = serializedObject.FindProperty("PlayAllOnAwake");
            mSequenceTypeProperty = serializedObject.FindProperty("SequenceType");
            mAnimationInfoListProperty = serializedObject.FindProperty("AnimationInfoList");
            mTargetTSequenceAnimation = serializedObject.targetObject as TSequenceAnimation;
            mValideComponentsNumberMap.Clear();
            mValideComponentOptionMap.Clear();
            UpdateValideComponentsInfo();
            UnregisterEditorUpdate();
            RegisterEditorUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            UnregisterEditorUpdate();
        }

        /// <summary>
        /// 初始化目标组件
        /// </summary>
        private void InitTarget()
        {
            mTarget ??= (mTarget as TSequenceAnimation);
        }

        /// <summary>
        /// 初始化有效组件信息
        /// </summary>
        private void InitValideComponentsInfo()
        {

        }

        /// <summary>
        /// 获取指定GameObject身上的TBaseAnimation组件数量
        /// </summary>
        /// <param name="go"></param>
        private int GetGoAnimationComponentsNumber(GameObject go)
        {
            return go != null ? go.GetComponents<TBaseAnimation>().Length : 0;
        }

        public override void OnInspectorGUI()
        {
            InitTarget();
            serializedObject.Update();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("播放", GUILayout.ExpandWidth(true)))
            {
                mTarget?.StartAllAnim();
            }
            if (GUILayout.Button("暂停", GUILayout.ExpandWidth(true)))
            {
                mTarget?.PauseAllAnim();
            }
            if (GUILayout.Button("继续", GUILayout.ExpandWidth(true)))
            {
                mTarget?.ResumeAllAnim();
            }
            if (GUILayout.Button("停止", GUILayout.ExpandWidth(true)))
            {
                mTarget?.StopAllAnim();
            }
            EditorGUILayout.EndHorizontal();
            UpdateValideComponentsInfo();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(mPlayAllOnAwakeProperty);
            EditorGUILayout.PropertyField(mSequenceTypeProperty);
            for (int i = 0; i < mAnimationInfoListProperty.arraySize; i++)
            {
                DisplayOneAnimationInspector(i);
            }
            if (GUILayout.Button("+", GUILayout.ExpandWidth(true), GUILayout.Height(20.0f)))
            {
                mAnimationInfoListProperty.InsertArrayElementAtIndex(mAnimationInfoListProperty.arraySize);
                var animationinfoproperty = mAnimationInfoListProperty.GetArrayElementAtIndex(mAnimationInfoListProperty.arraySize - 1);
                // 默认GameObject以自己为单位(大部分情况都是控制自身的TBaseAnimation)
                animationinfoproperty.FindPropertyRelative("TargetGo").objectReferenceValue = mTargetTSequenceAnimation.gameObject;
                animationinfoproperty.FindPropertyRelative("ControlAnimation").objectReferenceValue = null;
                animationinfoproperty.FindPropertyRelative("ControlAnimationIndex").intValue = -1;
                animationinfoproperty.serializedObject.ApplyModifiedProperties();
                PrintAllAnimationInfo();
            }
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 更新有效组件选择信息
        /// </summary>
        private void UpdateValideComponentsInfo()
        {
            for (int i = 0, length = mAnimationInfoListProperty.arraySize; i < length; i++)
            {
                var animationinfoproperty = mAnimationInfoListProperty.GetArrayElementAtIndex(i);
                var targetgo = animationinfoproperty.FindPropertyRelative("TargetGo").objectReferenceValue as GameObject;
                var newanimcomponentnumber = GetGoAnimationComponentsNumber(targetgo);
                bool needupdatecomponentoption = false;
                if (mValideComponentsNumberMap.ContainsKey(i))
                {
                    if (newanimcomponentnumber != mValideComponentsNumberMap[i])
                    {
                        mValideComponentsNumberMap[i] = newanimcomponentnumber;
                        needupdatecomponentoption = true;
                    }
                }
                else
                {
                    mValideComponentsNumberMap.Add(i, newanimcomponentnumber);
                    needupdatecomponentoption = true;
                }
                if (needupdatecomponentoption)
                {
                    //Debug.Log($"更新对象:{targetgo?.name}的有效动画组件数量:{newanimcomponentnumber}");
                    TBaseAnimation[] valideanimationcomponentarray = targetgo != null ? targetgo.GetComponents<TBaseAnimation>() : null;
                    var newvalideanimationnumber = valideanimationcomponentarray != null ? valideanimationcomponentarray.Length : 0;
                    string[] newvalidecomponentsoption = new string[] { };
                    Array.Resize(ref newvalidecomponentsoption, newvalideanimationnumber);
                    if (!mValideComponentOptionMap.ContainsKey(i))
                    {
                        mValideComponentOptionMap.Add(i, newvalidecomponentsoption);
                    }
                    else
                    {
                        mValideComponentOptionMap[i] = newvalidecomponentsoption;
                    }
                    for (int j = 0, length2 = newvalideanimationnumber; j < length2; j++)
                    {
                        mValideComponentOptionMap[i][j] = $"{j}:{valideanimationcomponentarray[j].GetType().Name}";
                    }
                }
            }
        }

        /// <summary>
        /// 显示一个动画组件观察面板
        /// </summary>
        /// <param name="index"></param>
        private void DisplayOneAnimationInspector(int index)
        {
            var amimnationinfoproperty = mAnimationInfoListProperty.GetArrayElementAtIndex(index);
            var targetgoproperty = amimnationinfoproperty.FindPropertyRelative("TargetGo");
            var targetgo = targetgoproperty.objectReferenceValue as GameObject;
            var controlanimationproperty = amimnationinfoproperty.FindPropertyRelative("ControlAnimation");
            var controlanimation = controlanimationproperty.objectReferenceValue as TBaseAnimation;
            var controlanimationindexproperty = amimnationinfoproperty.FindPropertyRelative("ControlAnimationIndex");
            var controlanimationindex = controlanimationindexproperty.intValue;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(20.0f), GUILayout.Height(20.0f)))
            {
                Debug.Log($"总数量:{mAnimationInfoListProperty.arraySize}删除索引:{index}");
                mAnimationInfoListProperty.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
                PrintAllAnimationInfo();
            }
            GUILayout.Space(5.0f);
            EditorGUILayout.LabelField($"{index}", GUILayout.Width(20.0f), GUILayout.Height(20.0f));
            EditorGUI.BeginChangeCheck();
            targetgo = EditorGUILayout.ObjectField(targetgo, typeof(GameObject), true, GUILayout.MinWidth(150.0f), GUILayout.Height(20)) as GameObject;
            if (GUI.changed)
            {
                Debug.Log("物体选择改变!");
                targetgoproperty.objectReferenceValue = targetgo;
                controlanimationproperty.objectReferenceValue = controlanimation;
                amimnationinfoproperty.serializedObject.ApplyModifiedProperties();
                PrintAllAnimationInfo();
            }
            EditorGUI.EndChangeCheck();
            EditorGUI.BeginChangeCheck();
            controlanimationindex = EditorGUILayout.Popup(controlanimationindex, mValideComponentOptionMap[index], GUILayout.Width(150.0f), GUILayout.Height(20.0f));
            if (GUI.changed)
            {
                Debug.Log($"组件选择索引:{controlanimationindex}");
                controlanimationproperty.objectReferenceValue = targetgo.GetComponents<TBaseAnimation>()[controlanimationindex];
                controlanimationindexproperty.intValue = controlanimationindex;
                amimnationinfoproperty.serializedObject.ApplyModifiedProperties();
                PrintAllAnimationInfo();
            }
            EditorGUI.EndChangeCheck();
            EditorGUILayout.EndHorizontal();
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
            if(mTarget == null)
            {
                return;
            }
            for(int i = 0; i < mTarget.AnimationInfoList.Count; i++)
            {
                var animationInfo = mTarget.AnimationInfoList[i];
                if(animationInfo == null || animationInfo.ControlAnimation == null)
                {
                    continue;
                }
                animationInfo.ControlAnimation.Update();
            }
        }

        /// <summary>
        /// 打印所有挂载的Animation信息
        /// </summary>
        private void PrintAllAnimationInfo()
        {
            for (int i = 0, length = mAnimationInfoListProperty.arraySize; i < length; i++)
            {
                var amimnationinfoproperty = mAnimationInfoListProperty.GetArrayElementAtIndex(i);
                var targetgoproperty = amimnationinfoproperty.FindPropertyRelative("TargetGo");
                var targetgo = targetgoproperty.objectReferenceValue as GameObject;
                var controlanimationproperty = amimnationinfoproperty.FindPropertyRelative("ControlAnimation");
                var controlanimation = controlanimationproperty.objectReferenceValue as TBaseAnimation;
                var controlanimationindexproperty = amimnationinfoproperty.FindPropertyRelative("ControlAnimationIndex");
                var controlanimationindex = controlanimationindexproperty.intValue;
                Debug.Log($"索引值:{i}目标对象名:{targetgo?.name}目标动画组件:{controlanimation?.GetType().Name}目标动画索引值:{controlanimationindex}");
            }
        }
    }
}