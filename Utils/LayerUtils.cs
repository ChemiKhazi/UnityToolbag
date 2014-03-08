using UnityEngine;
using System.Collections;

public static class LayerMaskExtensions
{
	public static bool ContainsLayer(this LayerMask mask, int layerNumber)
	{
		return ((1 << layerNumber) & mask.value) != 0;
	}
}