using UnityEngine;

namespace SecondStar.Effects
{
	[RequireComponent (typeof(Camera))]
	[ExecuteInEditMode]
	public class ImageEffectBase : MonoBehaviour {
		/// Provides a shader property that is set in the inspector
		/// and a material instantiated from the shader
		public Shader shader;

		private Material m_Material;
		protected Material material {
			get {
				if (m_Material == null) {
					m_Material = new Material (shader);
					m_Material.hideFlags = HideFlags.HideAndDontSave;
				}
				return m_Material;
			} 
		}

		public RenderTexture Output
		{
			get; protected set;
		}

		protected virtual void Start ()
		{
			// Disable if we don't support image effects
			if (!SystemInfo.supportsImageEffects) {
				enabled = false;
				return;
			}
			
			// Disable the image effect if the shader can't
			// run on the users graphics card
			if (!shader.isSupported)
				enabled = false;
		}
		
		protected virtual void OnDisable() {
			if( m_Material )
			{
				DestroyImmediate( m_Material );
			}
			if (Output != null)
			{
				ClearBuffer(Output);
			}
		}

		protected virtual void OnPreRender()
		{
		}

		protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
		}

		protected void ClearBuffer(RenderTexture buffer)
		{
			if (buffer != null)
			{
				RenderTexture.ReleaseTemporary(buffer);
				buffer = null;
			}
		}
	}
}