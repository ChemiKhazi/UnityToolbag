using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbag
{
    [CustomPropertyDrawer(typeof(AnimParamsAttribute))]
    public class AnimParamsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Reject drawing this custom attribute if property wasn't a string type
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position,property,label);
                return;
            }

            AnimParamsAttribute animParams = (AnimParamsAttribute) attribute;

            SerializedProperty animProp = property.serializedObject.FindProperty(animParams.animatorProp);
            if (animProp == null)
            {
                base.OnGUI(position, property, label);
                return;
            }

            Animator targetAnim = animProp.objectReferenceValue as Animator;
            if (targetAnim == null)
            {
                base.OnGUI(position, property, label);
                return;
            }

            string propVal = property.stringValue;
            int selectedIndex;
            GUIContent[] popup = BuildParamList(targetAnim, propVal, out selectedIndex);

            selectedIndex = EditorGUI.Popup(position, label, selectedIndex, popup);
            if (selectedIndex < popup.Length && selectedIndex > -1)
                propVal = popup[selectedIndex].text;
            else
                propVal = "";

            if (propVal.Equals(property.stringValue) == false)
            {
                property.stringValue = propVal;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }

        private GUIContent[] BuildParamList(Animator targetAnimator, string propVal, out int selectedIndex)
        {
            selectedIndex = -1;
            GUIContent[] paramList = new GUIContent[targetAnimator.parameterCount+1];
            for (int i = 0; i < targetAnimator.parameterCount; i++)
            {
                AnimatorControllerParameter param = targetAnimator.parameters[i];
                paramList[i] = new GUIContent(param.name);
                if (param.name.Equals(propVal))
                    selectedIndex = i;
            }
            paramList[targetAnimator.parameterCount] = new GUIContent("<None>");
            return paramList;
        } 


    }
}