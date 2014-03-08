using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
	public static T CreateAssetFullPath<T>(string path) where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();
		if (string.IsNullOrEmpty(path))
			return null;
		if (path.EndsWith(".asset") == false)
			return null;

		AssetDatabase.CreateAsset(asset, path);
		AssetDatabase.SaveAssets();
		EditorGUIUtility.PingObject(asset);
		return asset;
	}

	public static void CreateAsset<T>(string path) where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();
		if (path == "")
		{
			path = "Assets";
		}
		else if (Path.GetExtension(path) != "")
		{
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		}

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

		AssetDatabase.CreateAsset(asset, assetPathAndName);

		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

    public static void CreateAsset<T>() where T : ScriptableObject
	{
		CreateAsset<T>(Path.GetExtension("Assets"));
    }
}
