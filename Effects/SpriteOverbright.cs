using UnityEngine;
using System.Collections;

[ExecuteInEditMode, RequireComponent(typeof(SpriteRenderer))]
public class SpriteOverbright : MonoBehaviour {

	public SpriteRenderer sprite;

	public Color spriteColor = Color.white;
	[Range(1f, 4f)]
	public float intensity = 1f;

	public Color defaultFlashColor = Color.red;
	[Range(1f, 4f)]
	public float defaultFlashIntensity = 1f;

	public float defaultFlashTime = 1f;

	float lastFlashTime, flashLength;
	int flashCounter = 0;
	Color originalColor, flashColor;
	float originalIntensity, flashIntensity;

	void Reset()
	{
		sprite = GetComponent<SpriteRenderer>();
	}

	#region Flashing API
	public bool IsFlashing
	{
		get { return flashCounter != 0; }
	}

	public void FlashIntensity(float flashLength, float flashIntensity, int loops)
	{
		FlashColor(flashLength, spriteColor, flashIntensity, loops);
	}

	public void FlashColor(float flashLength, Color flashColor, float flashIntensity, int loops)
	{
		if (!IsFlashing)
		{
			originalColor = spriteColor;
			originalIntensity = intensity;
		}

		lastFlashTime = Time.time;
		this.flashCounter = loops;
		this.flashColor = flashColor;
		this.flashLength = flashLength;
		this.flashIntensity = Mathf.Clamp(flashIntensity, 1f, 4f);
	}

	public void StopFlash()
	{
		if (IsFlashing)
		{
			sprite.color = originalColor;
			intensity = originalIntensity;
		}
		flashCounter = 0;
	}
	#endregion

	void Update () {

		Color updateColor = spriteColor;
		float mulBy = intensity / 4f;

		if (IsFlashing)
		{
			float currentT = (Time.time - lastFlashTime) / (flashLength / 2f);
			bool overPeak = currentT > 1f;
			if (overPeak)
				currentT = 2f - currentT;

			if (overPeak && currentT <= float.Epsilon)
			{
				if (flashCounter > 0)
					flashCounter--;
				lastFlashTime = Time.time;
			}

			updateColor = Color.Lerp(originalColor, flashColor, currentT);
			mulBy = Mathf.Lerp(originalIntensity, flashIntensity, currentT) / 4f;
		}

		updateColor.r *= mulBy;
		updateColor.g *= mulBy;
		updateColor.b *= mulBy;
		sprite.color = updateColor;
	}

	//void OnGUI()
	//{
	//	if (GUILayout.Button("Once"))
	//		Flash(1f, Color.white, 4f, 1);
	//	if (GUILayout.Button("Thrice"))
	//		Flash(1f, Color.white, 4f, 3);
	//	if (GUILayout.Button("Loop"))
	//		Flash(1f, Color.white, 4f, -1);
	//	if (GUILayout.Button("Stop"))
	//		StopFlash();
	//}
}
