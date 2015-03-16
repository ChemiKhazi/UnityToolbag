using UnityEngine;

namespace kontrabida.utils.extensions
{
	public static class BitmaskHelper
	{
		public static int AddFlag(int original, int add)
		{
			return original | add;
		}
		
		public static int ClearFlag(int original, int clear)
		{
			return original & ~clear;
		}
		
		public static bool HasFlag(int original, int test)
		{
			return (original & test) != 0;
		}
	}
	
	public static class LayerMaskExtension
	{
		public static bool Contains(this LayerMask layer, LayerMask check)
		{
			return (layer & check) != 0;
		}

		public static LayerMask AddLayer(this LayerMask layer, LayerMask addLayer)
		{
			return AddValue(layer, addLayer.value);
		}

		public static LayerMask RemoveLayer(this LayerMask layer, LayerMask removeLayer)
		{
			return RemoveValue(layer, removeLayer.value);
		}

		public static bool ContainsLayerValue(this LayerMask layer, int layerValue)
		{
			return ((1 << layerValue) & layer.value) != 0;
		}

		public static LayerMask AddValue(this LayerMask layer, int mask)
		{
			layer.value |= mask;
			return layer;
		}

		public static LayerMask RemoveValue(this LayerMask layer, int mask)
		{
			layer.value = layer.value & ~mask;
			return layer;
		}

		/// <summary>
		/// Returns the first active layer in the LayerMask, for use with GameObject.layer
		/// </summary>
		/// <param name="layer"></param>
		/// <returns></returns>
		public static int ToLayerNumber(this LayerMask layer)
		{
			for (int i = 0; i < 32; i++)
			{
				if (((1 << i) & layer.value) > 0)
					return i;
			}
			return 0;
		}

		public static LayerMask AddLayerValue(this LayerMask layer, int layerNumber)
		{
			layer.value |= (1 << layerNumber);
			return layer;
		}
	}
}
