using UnityEngine;
using System.Collections;

namespace SecondStar.Effects
{
	public class ScreenRefractionEffect : ImageEffectBase
	{
		static Color cameraColor = new Color(0.5f, 0.5f, 1f, 0);

		[Range(0f, 1f)]
		public float refractStrength = 0.1f;
		public LayerMask refractMask;

		Camera effectsCamera;

		void Reset()
		{
			shader = Shader.Find("KBD/Refract Shader");
		}

		protected override void OnPreRender()
		{
			if (effectsCamera == null)
			{
				GameObject effectCamObject = new GameObject();
				effectCamObject.hideFlags = HideFlags.HideAndDontSave;
				effectsCamera = effectCamObject.AddComponent<Camera>();
				effectsCamera.enabled = false;
			}
		}

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			RenderTexture distortBuffer = RenderTexture.GetTemporary(source.width, source.height);
			effectsCamera.CopyFrom(camera);
			effectsCamera.cullingMask = refractMask;
			effectsCamera.backgroundColor = cameraColor;
			effectsCamera.transparencySortMode = TransparencySortMode.Orthographic;
			effectsCamera.targetTexture = distortBuffer;
			effectsCamera.Render();

			material.SetFloat("_Strength", refractStrength);
			material.SetTexture("_Refract", distortBuffer);
			Graphics.Blit(source, destination, material);

			RenderTexture.ReleaseTemporary(distortBuffer);
		}
	}
}