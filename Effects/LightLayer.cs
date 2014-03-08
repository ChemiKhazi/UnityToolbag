using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SecondStar.LevelDesign
{
	public class LightLayer : MonoBehaviour
	{
		public enum OverlayMode
		{
			SoftLight,
			HardLight,
			Overlay
		}

		public OverlayMode blendMode;
		public float opacity = 1;
		public Color lightColor;
		public float lightVolume;

		public Shader LightShader
		{
			get
			{
				return Shader.Find("Kbd/Lighting/" + blendMode);
			}
		}

		public List<GameObject> attachedObjects = new List<GameObject>();

		public void AttachObjectLight(GameObject targetObject)
		{
			attachedObjects.Add(targetObject);
			targetObject.layer = gameObject.layer;
		}

		public void SetToActiveLightLayer()
		{
			SetToLayer(LayerMask.NameToLayer("active-light"));
		}

		public void SetToInactiveLightLayer()
		{
			SetToLayer(LayerMask.NameToLayer("light"));
		}

		void SetToLayer(int layer)
		{
			//Debug.Log("Set to layer " + layer);
			gameObject.layer = layer;
			foreach (GameObject attachedObject in attachedObjects)
			{
				attachedObject.layer = layer;
			}
		}
	}
}