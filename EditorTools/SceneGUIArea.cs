using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityToolbag
{
	public class SceneGUIArea : IDisposable
	{
		public SceneGUIArea(Rect area)
		{
			GUILayout.BeginArea(area);
			Handles.BeginGUI();
		}

		public void Dispose()
		{
			Handles.EndGUI();
			GUILayout.EndArea();
		}
	}
}
