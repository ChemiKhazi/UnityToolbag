using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlasher : MonoBehaviour {

	public SpriteRenderer sprite;
	public Color defaultFlashColor = Color.red;

	float lastFlashTime, flashLength;
	int flashCounter = 0;
	Color originalColor, flashColor;

	void Reset()
	{
		sprite = GetComponent<SpriteRenderer>();
	}

	public bool IsFlashing
	{
		get { return flashCounter != 0; }
	}

	void Update()
	{
		if (!IsFlashing)
			return;

		float currentT = Time.time - lastFlashTime / (flashLength / 2f);
		bool overPeak = currentT > 1f;
		if (overPeak)
			currentT = 2f - currentT;

		if (overPeak && currentT <= float.Epsilon)
		{
			if (flashCounter > 0)
				flashCounter--;
			lastFlashTime = Time.time;
		}

		Color currentColor = Color.Lerp(originalColor, flashColor, currentT);
		sprite.color = currentColor;
	}

	public void Flash(float flashLength)
	{
		Flash(defaultFlashColor, flashLength, 1);
	}

	public void Flash(float flashLength, int loops)
	{
		Flash(defaultFlashColor, flashLength, loops);
	}

	public void Flash(Color flashColor, float flashLength)
	{
		Flash(flashColor, flashLength, 1);
	}

	public void Flash(Color flashColor, float flashLength, int loops)
	{
		if (!IsFlashing)
			originalColor = sprite.color;

		lastFlashTime = Time.time;
		this.flashCounter = loops;
		this.flashColor = flashColor;
		this.flashLength = flashLength;
	}

	public void StopFlash()
	{
		if (IsFlashing)
		{
			sprite.color = originalColor;
		}
		flashCounter = 0;
	}
}
