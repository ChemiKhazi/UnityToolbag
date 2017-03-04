using UnityEngine;
using System.Collections;

namespace UnityToolbag
{
	/// <summary>
	/// Display a List/Array as a sortable list in the inspector
	/// </summary>
	public class SortableArrayAttribute : PropertyAttribute
	{
		public string ElementHeader { get; protected set; }
		public bool HeaderZeroIndex { get; protected set; }

		public SortableArrayAttribute()
		{
			ElementHeader = string.Empty;
			HeaderZeroIndex = false;
		}

		public SortableArrayAttribute(string headerString, bool isZeroIndex = true)
		{
			ElementHeader = headerString;
			HeaderZeroIndex = isZeroIndex;
		}
	}
}
