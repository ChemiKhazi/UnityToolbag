using UnityEngine;
using System.Collections;

public static class RectExtension
{
	public static Rect ScreenAlign(this Rect input, SpriteAlignment align)
	{
		Vector2 size = new Vector2(input.width, input.height);

		// X align
		if (align == SpriteAlignment.TopLeft ||
			align == SpriteAlignment.LeftCenter ||
			align == SpriteAlignment.BottomLeft)
		{
			input.xMin = input.xMin;
		}

		if (align == SpriteAlignment.TopCenter ||
			align == SpriteAlignment.Center ||
			align == SpriteAlignment.BottomCenter)
		{
			input.xMin = (Screen.width / 2) + input.xMin;
		}

		if (align == SpriteAlignment.TopRight ||
			align == SpriteAlignment.RightCenter ||
			align == SpriteAlignment.BottomRight)
		{
			input.xMin = (Screen.width) - input.xMin;
		}

		// Y align
		if (align == SpriteAlignment.TopLeft ||
			align == SpriteAlignment.TopLeft ||
			align == SpriteAlignment.TopRight)
		{
			input.yMin = input.yMin;
		}
		if (align == SpriteAlignment.LeftCenter ||
			align == SpriteAlignment.Center ||
			align == SpriteAlignment.RightCenter)
		{
			input.yMin = (Screen.height / 2) + input.yMin;
		}
		if (align == SpriteAlignment.BottomLeft ||
			align == SpriteAlignment.BottomCenter ||
			align == SpriteAlignment.BottomRight)
		{
			input.yMin = (Screen.height) - input.yMin;
		}

		input.width = size.x;
		input.height = size.y;
		return input;
	}

	public static Rect GetScreenRect(this RectTransform target, Camera cam = null)
	{
		// 0 lower left, 1 upper left, 2 upper right, 3 lower right
		Vector3[] corners = new Vector3[4];
		target.GetWorldCorners(corners);

		for (int i = 0; i < corners.Length; i++)
		{
			corners[i] = RectTransformUtility.WorldToScreenPoint(cam, corners[i]);
		}

		Rect screenRect = new Rect
		{
			min = corners[0],
			max = corners[2]
		};
		return screenRect;
	}

	/// <summary>
	/// Get other RectTransform as a Rect in local space of this RectTransform
	/// </summary>
	/// <param name="target"></param>
	/// <param name="other"></param>
	/// <param name="cam"></param>
	/// <returns></returns>
	public static Rect GetRectAsLocal(this RectTransform target, RectTransform other, Camera cam = null)
	{
		// First, get other as screen point rect
		Rect otherScreenRect = other.GetScreenRect();
		// Convert min max to local points
		Vector2 localMin;
		Vector2 localMax;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(target, otherScreenRect.min, cam, out localMin);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(target, otherScreenRect.max, cam, out localMax);

		return new Rect()
		{
			min = localMin,
			max = localMax
		};
	}
}
