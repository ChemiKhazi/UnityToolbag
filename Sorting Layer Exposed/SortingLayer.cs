using UnityEngine;
using System.Collections;

[System.Serializable]
public class SortingLayer {

	public int sortLayer = -1;
	public string layerName = "";
	public int layerOrder = 0;

	public void ApplyToRenderer(Renderer targetRenderer)
	{
		targetRenderer.sortingLayerID = sortLayer;
		targetRenderer.sortingOrder = layerOrder;
		targetRenderer.sortingLayerName = layerName;
	}
}
