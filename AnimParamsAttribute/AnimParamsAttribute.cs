using System;
using UnityEngine;

namespace UnityToolbag
{
    public class AnimParamsAttribute : PropertyAttribute
    {
        public string animPath;
        public bool isConfig;

		/// <summary>
		/// Show the parameters of an Animator as a drop down in the inspector
		/// </summary>
		/// <param name="animatorPropName">The field name of an Animator component in the same MonoBehaviour</param>
        public AnimParamsAttribute(string animatorPropName)
        {
            animPath = animatorPropName;
	        isConfig = false;
        }

		/// <summary>
		/// Show the parameters of an Animator as a drop down in the inspector
		/// </summary>
		/// <param name="configPath">The field name of the Animator or AnimParamsConfig in this MonoBehaviour</param>
		/// <param name="isConfig">If true, configPath points to an AnimParamsConfig</param>
        public AnimParamsAttribute(string configPath, bool isConfig)
        {
			animPath = configPath;
			this.isConfig = isConfig;
        }
    }

	/// <summary>
	/// Inspector configurator for AnimParamsAttribute
	/// </summary>
    [Serializable]
    public class AnimParamsConfig
    {
		[SerializeField]
        protected string instanceId;
    }
}