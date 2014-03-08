using UnityEngine;
using System.Collections;

public static class FingerGesturesExtensions
{
	public static bool RayCastExclusive(this ScreenRaycaster caster, Vector2 screenPos, LayerMask mask, out RaycastHit hit)
	{
		hit = new RaycastHit();
		foreach (Camera cam in caster.Cameras)
		{
			Ray ray = cam.ScreenPointToRay(screenPos);
			bool didHit = false;

			if (caster.RayThickness > 0)
				didHit = Physics.SphereCast(ray, 0.5f * caster.RayThickness, out hit, Mathf.Infinity, mask);
			else
				didHit = Physics.Raycast(ray, out hit, Mathf.Infinity, mask);

			if (didHit)
				return true;
		}
		return false;
	}
}
