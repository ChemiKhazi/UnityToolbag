using UnityEngine;
using System.Collections;

namespace SecondStar.Effects
{
	/// <summary>
	/// Port of standard blur effect
	/// </summary>
	public class BlurEffect : ImageEffectBase {
		
		public enum BlurType {
			StandardGauss = 0,
			SgxGauss = 1,
		}

		[Range(0,2)]
		public int downsample;
		[Range(0f, 10f)]
		public float blurSize = 3;
		[Range(1,4)]
		public int blurIterations = 2;
		public BlurType blurType = BlurType.StandardGauss;

		public LayerMask maskLayer;
		Camera effectsCamera;

		Material mixMaterial;

		void Reset()
		{
			shader = Shader.Find("KBD/Fast Blur");
		}

		private Material MixMaterial
		{
			get
			{
				if (mixMaterial == null)
				{
					mixMaterial = new Material(Shader.Find("KBD/Mix Shader"));
					mixMaterial.hideFlags = HideFlags.HideAndDontSave;
				}
				return mixMaterial;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (mixMaterial != null)
				DestroyImmediate(mixMaterial);
			if (effectsCamera != null)
				DestroyImmediate(effectsCamera);
		}

		protected override void OnPreRender()
		{
			if (maskLayer.value != 0 && effectsCamera == null)
			{
				GameObject effectCamObject = new GameObject();
				effectCamObject.hideFlags = HideFlags.HideAndDontSave;
				effectsCamera = effectCamObject.AddComponent<Camera>();
				effectsCamera.enabled = false;
			}
		}

		protected override void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
			float widthMod = 1.0f / (1.0f * (1<<downsample));
			Vector4 blurParams =  new Vector4 (blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f);
			material.SetVector("_Parameter", blurParams);
			source.filterMode = FilterMode.Bilinear;

			int renderWidth = source.width >> downsample;
			int renderHeight = source.height >> downsample;

			// downsample
			RenderTexture tempRender = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, source.format);
			tempRender.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, tempRender, material, 0);

			int passOffs = blurType == BlurType.StandardGauss ? 0 : 2;

			for (int i = 0; i < blurIterations; i++)
			{
				float iterationOffs = i;
				blurParams = new Vector4 (blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f);
				material.SetVector("_Parameter", blurParams);

				// vertical blur
				RenderTexture tempRender2 = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, source.format);
				tempRender2.filterMode = FilterMode.Bilinear;
				Graphics.Blit(tempRender, tempRender2, material, 1 + passOffs);
				RenderTexture.ReleaseTemporary(tempRender);
				tempRender = tempRender2;

				// horizontal blur
				tempRender2 = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, source.format);
				tempRender2.filterMode = FilterMode.Bilinear;
				Graphics.Blit(tempRender, tempRender2, material, 2 + passOffs);
				RenderTexture.ReleaseTemporary(tempRender);
				tempRender = tempRender2;
			}

			if (maskLayer.value == 0)
			{
				Graphics.Blit(tempRender, destination);
			}
			else
			{
				RenderTexture maskBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, source.format);

				effectsCamera.CopyFrom(GetComponent<Camera>());
				effectsCamera.cullingMask = maskLayer;
				effectsCamera.backgroundColor = Color.black;
				effectsCamera.transparencySortMode = GetComponent<Camera>().transparencySortMode;
				effectsCamera.targetTexture = maskBuffer;
				effectsCamera.Render();

				MixMaterial.SetTexture("_OverLayer", tempRender);
				MixMaterial.SetTexture("_Blend", maskBuffer);
				Graphics.Blit(source, destination, MixMaterial);

				RenderTexture.ReleaseTemporary(maskBuffer);
			}

			RenderTexture.ReleaseTemporary(tempRender);
		}

	}
}