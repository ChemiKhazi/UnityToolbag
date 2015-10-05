using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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
	            EditorGUI.PropertyField(position, property, label);
                return;
            }

            AnimParamsAttribute animParams = (AnimParamsAttribute) attribute;

	        int selectedIndex = -1;
	        GUIContent[] popupGUI = null;

	        if (animParams.isConfig)
	        {
		        popupGUI = GetFromInstanceId(property, animParams, out selectedIndex);
	        }
	        else
	        {
		        popupGUI = GetFromComponent(property, animParams, out selectedIndex);
	        }

	        if (popupGUI == null)
			{
				EditorGUI.PropertyField(position, property, label);
		        return;
	        }

	        string propVal;
            selectedIndex = EditorGUI.Popup(position, label, selectedIndex, popupGUI);
            if (selectedIndex < popupGUI.Length && selectedIndex > -1)
                propVal = popupGUI[selectedIndex].text;
            else
                propVal = "";

            if (propVal.Equals(property.stringValue) == false)
            {
                property.stringValue = propVal;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }

	    private GUIContent[] GetFromInstanceId(SerializedProperty property,
											AnimParamsAttribute animParams,
											out int selectedIndex)
	    {
		    selectedIndex = -1;

		    SerializedProperty animParamCfgProp = property.serializedObject.FindProperty(animParams.animPath);
		    if (animParamCfgProp == null)
			    return null;

		    SerializedProperty refType = animParamCfgProp.FindPropertyRelative("referenceType");
		    SerializedProperty refPath = animParamCfgProp.FindPropertyRelative("instanceId");

		    if (refType == null || refPath == null)
			    return null;

		    AnimatorControllerParameter[] paramList = null;
		    int targetId;

		    if (refType.enumValueIndex == (int) AnimParamsConfig.RefType.Animator)
		    {
			    if (int.TryParse(refPath.stringValue, out targetId))
			    {
				    Animator targetAnimator = EditorUtility.InstanceIDToObject(targetId) as Animator;
					if (targetAnimator != null)
						paramList = targetAnimator.parameters;
			    }
		    }
		    else
			{
				if (int.TryParse(refPath.stringValue, out targetId))
				{
					AnimatorController targetController = EditorUtility.InstanceIDToObject(targetId) as AnimatorController;
					if (targetController != null)
						paramList = targetController.parameters;
				}
			    
		    }

		    if (paramList != null)
			    return BuildParamList(paramList, property.stringValue, out selectedIndex);

		    return null;
	    }

	    private GUIContent[] GetFromComponent(SerializedProperty property, AnimParamsAttribute animParams, out int selectedIndex)
	    {
		    selectedIndex = -1;
			SerializedProperty animProp = property.serializedObject.FindProperty(animParams.animPath);
			if (animProp == null)
				return null;

			Animator targetAnim = animProp.objectReferenceValue as Animator;
		    if (targetAnim == null)
			    return null;

			string propVal = property.stringValue;
			return BuildParamList(targetAnim.parameters, propVal, out selectedIndex);
	    }

        private GUIContent[] BuildParamList(AnimatorControllerParameter[] paramList, string propVal, out int selectedIndex)
        {
            selectedIndex = -1;
            GUIContent[] paramsGUI = new GUIContent[paramList.Length+1];
            for (int i = 0; i < paramList.Length; i++)
            {
                AnimatorControllerParameter param = paramList[i];
                paramsGUI[i] = new GUIContent(param.name);
                if (param.name.Equals(propVal))
                    selectedIndex = i;
            }
            paramsGUI[paramList.Length] = new GUIContent("<None>");
            return paramsGUI;
        }
    }
}