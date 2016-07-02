using UnityEngine;

namespace UnityToolbag
{
	/// <summary>
	/// Display an integer property as a dropdown, referring to a property that is an array
	/// </summary>
	public class ArrayIndexAttribute : PropertyAttribute
	{
		public string	Path { get; protected set; }
		public int		DefaultIndex { get; protected set; }
		public string	DefaultDisplay { get; protected set; }

		/// <summary>
		/// Display an integer property as a dropdown
		/// </summary>
		/// <param name="propertyPath">The path to the array property</param>
		/// <param name="defaultIndex">The default integer to use</param>
		/// <param name="defaultDisplay">Default value to display in drop down</param>
		public ArrayIndexAttribute(string propertyPath, int defaultIndex = -1, string defaultDisplay = "None")
		{
			Path = propertyPath;
			DefaultIndex = defaultIndex;
			DefaultDisplay = defaultDisplay;
		}	 
	}
}