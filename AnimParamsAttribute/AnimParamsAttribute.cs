using UnityEngine;

namespace UnityToolbag
{
	public class AnimParamsAttribute : PropertyAttribute
	{
		public string animatorProp;

		public AnimParamsAttribute(string animatorPropName)
		{
			animatorProp = animatorPropName;
		}
	}
}