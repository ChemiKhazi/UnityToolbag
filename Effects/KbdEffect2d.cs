using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SecondStar.LevelDesign
{
	[ExecuteInEditMode]
	public class KbdEffect2d : MonoBehaviour
	{
		public bool doFog, doLight;

		#region Shader materials
		public Shader positionShader;

		public Shader fogShader;
		protected Material fogMat;
		protected Material FogMaterial
		{
			get
			{
				if (fogMat == null)
				{
					fogMat = new Material(fogShader);
					fogMat.hideFlags = HideFlags.HideAndDontSave;
				}
				return fogMat;
			}
		}
		
		protected Material lightMat;
		protected void SetupLightMaterial(Shader lightShader)
		{
			DestroyImmediate(lightMat);
			lightMat = new Material(lightShader);
			lightMat.hideFlags = HideFlags.HideAndDontSave;
		}
		protected Material LightMaterial
		{
			get
			{
				return lightMat;
			}
		}
		#endregion

		public float effectCeiling = 300;
		public Texture2D fogTexture;
		public Color fogColor, fogColorFar;
		public float fogStart;
		public float fogRollOff;
		public float fogHeight;
		public float fogHeightFalloff;

		public LayerMask lightLayerMask;

		public bool resetLights = false;

		RenderTexture lightBuffer, accumulationBuffer, positionBuffer;
		Camera effectsCamera;
		List<LightLayer> lightLayers = new List<LightLayer>();

		protected void Start()
		{
			// Disable if we don't support image effects
			if (!SystemInfo.supportsImageEffects)
			{
				enabled = false;
				return;
			}

			GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
			if (doFog && (!fogShader || !fogShader.isSupported))
				enabled = false;
		}

		protected void OnDisable()
		{
			if (fogMat != null)
				DestroyImmediate(fogMat);
			if (lightMat != null)
				DestroyImmediate(lightMat);
			if (effectsCamera != null)
				DestroyImmediate(effectsCamera);
		}

		void CleanBuffers()
		{
			if (lightBuffer != null)
			{
				RenderTexture.ReleaseTemporary(lightBuffer);
				lightBuffer = null;
			}
			if (positionBuffer != null)
			{
				RenderTexture.ReleaseTemporary(positionBuffer);
				positionBuffer = null;
			}
			if (accumulationBuffer != null)
			{
				RenderTexture.ReleaseTemporary(accumulationBuffer);
				accumulationBuffer = null;
			}
		}

		void FindLightLayers()
		{
			LightLayer[] foundLayers = (LightLayer[]) FindObjectsOfType(typeof(LightLayer));
			lightLayers.AddRange(foundLayers);
		}

		void OnPreRender()
		{
			if (!enabled || !gameObject.activeInHierarchy || !doLight)
				return;

			// Rendering objects setup
			if (effectsCamera == null)
			{
				GameObject lightCamObject = new GameObject();
				lightCamObject.hideFlags = HideFlags.HideAndDontSave;
				effectsCamera = lightCamObject.AddComponent<Camera>();
				effectsCamera.enabled = false;
			}
			if (lightLayers.Count == 0 || resetLights)
			{
				lightLayers.Clear();
				FindLightLayers();
				resetLights = false;
			}

			// Pre render setup
			CleanBuffers();

			if (doLight || doFog)
			{
				Shader.SetGlobalFloat("_EffectCeiling", effectCeiling);

				// Render the position data out
				positionBuffer = RenderTexture.GetTemporary((int)(GetComponent<Camera>().pixelWidth),
															(int)(GetComponent<Camera>().pixelHeight),
															16, RenderTextureFormat.Default);

				effectsCamera.CopyFrom(GetComponent<Camera>());
				effectsCamera.cullingMask = GetComponent<Camera>().cullingMask;
				effectsCamera.backgroundColor = new Color(0, 0, 0, 0);
				effectsCamera.clearFlags = CameraClearFlags.SolidColor;
				effectsCamera.depthTextureMode = DepthTextureMode.DepthNormals;
				effectsCamera.transparencySortMode = TransparencySortMode.Orthographic;
				effectsCamera.targetTexture = positionBuffer;

				effectsCamera.RenderWithShader(positionShader, "");
			}

			// For lighting, setup the buffers
			if (doLight)
			{
				// Create a buffer to render the light layers into, half size for quicker render
				lightBuffer = RenderTexture.GetTemporary((int)(GetComponent<Camera>().pixelWidth),
														(int)(GetComponent<Camera>().pixelHeight),
														16, RenderTextureFormat.Default);
				// Create another buffer to accumulate the lights on
				accumulationBuffer = RenderTexture.GetTemporary((int)GetComponent<Camera>().pixelWidth,
																(int)GetComponent<Camera>().pixelHeight,
																(int)GetComponent<Camera>().depth,
																RenderTextureFormat.Default);
			}
			// Not lighting, reset stuff
			else
			{
				GetComponent<Camera>().targetTexture = null;
			}
		}

		void TestRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(positionBuffer, destination);
		}

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			//TestRenderImage(source, destination);
			bool hasBlit = false;

			// Used for precalculating values for the shader
			float cameraDepth = GetComponent<Camera>().farClipPlane - GetComponent<Camera>().nearClipPlane;
			
			if (doLight)
			{
				// Copy the main camera output into the accumulation buffer
				Graphics.Blit(source, accumulationBuffer);

				// Setup the effects camera
				effectsCamera.CopyFrom(GetComponent<Camera>());
				effectsCamera.backgroundColor = new Color(0, 0, 0, 0);
				effectsCamera.clearFlags = CameraClearFlags.SolidColor;
				effectsCamera.transparencySortMode = TransparencySortMode.Orthographic;
				effectsCamera.cullingMask = lightLayerMask;

				bool doResetLights = false;
				foreach (LightLayer light in lightLayers)
				{
					if (light == null)
						doResetLights = true;

					light.SetToActiveLightLayer();

					// Render out the light overlay
					effectsCamera.targetTexture = lightBuffer;
					effectsCamera.Render();

					// Calculate values for the light in camera depth space
					float lightVolume = (light.lightVolume / cameraDepth);
					Vector3 depthVector = GetComponent<Camera>().transform.position - light.transform.position;
					depthVector = Vector3.Project(depthVector, GetComponent<Camera>().transform.forward);
					float lightDepth = Vector3.Dot(depthVector, GetComponent<Camera>().transform.forward * -1) / cameraDepth;

					SetupLightMaterial(light.LightShader);
					LightMaterial.SetTexture("_PositionTex", positionBuffer);
					LightMaterial.SetTexture("_LightOverlay", lightBuffer);
					LightMaterial.SetColor("_LightColor", light.lightColor);
					LightMaterial.SetFloat("_LightDepth", lightDepth);
					LightMaterial.SetFloat("_LightVolume", lightVolume);
					LightMaterial.SetFloat("_MasterOpacity", light.opacity);

					// Render the light on the accumulation buffer
					Graphics.Blit(accumulationBuffer, accumulationBuffer, LightMaterial);

					light.SetToInactiveLightLayer();
				}

				if (doResetLights)
					resetLights = true;

				if (!doFog)
				{
					Graphics.Blit(accumulationBuffer, destination);
				}

				hasBlit = true;
			}

			if (doFog)
			{
				// Calculate fog parameters in camera depth space
				Vector3 fogVector = new Vector3(0, 0, fogStart - GetComponent<Camera>().transform.position.z);
				fogVector = Vector3.Project(fogVector, GetComponent<Camera>().transform.forward);
				float fogStartPos = Vector3.Dot(fogVector, GetComponent<Camera>().transform.forward) / cameraDepth;
				float fogFallOff = fogStartPos + (fogRollOff / cameraDepth);

				// Setup fog parameters
				FogMaterial.SetTexture("_PositionTex", positionBuffer);
				FogMaterial.SetColor("_FogColor", fogColor);
				FogMaterial.SetColor("_FogColorFar", fogColorFar);
				FogMaterial.SetTexture("_FogTex", fogTexture);
				FogMaterial.SetVector("_FogParams", new Vector4(fogStartPos, fogFallOff, fogHeight, fogHeightFalloff));

				hasBlit = true;

				// If not lit, take from source
				if (!doLight)
					Graphics.Blit(source, accumulationBuffer);
				
				Graphics.Blit(accumulationBuffer, destination, FogMaterial);
			}

			if (!hasBlit)
				Graphics.Blit(source, destination);
		}
	}
}