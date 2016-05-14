using System;
using UnityEngine;

namespace UnityToolbag
{
	public class ResourcePathAttribute : PropertyAttribute
	{
		public Type allowedType;

		/// <summary>
		/// Show string as an object box that points to a resource asset.
		/// </summary>
		public ResourcePathAttribute()
		{
			allowedType = null;
		}

		/// <summary>
		/// Show string as an object box that points to a resource asset, restricted by a type
		/// </summary>
		/// <param name="restrictType">Type of asset that can be placed into object box.</param>
		public ResourcePathAttribute(Type restrictType)
		{
			allowedType = restrictType;
		}
	}
}