using UnityEngine;

public class SpriteRadialFill : MonoBehaviour
{
	public SpriteRenderer sprite;

	[SerializeField]
	protected float rotation = 0f;
	[SerializeField]
	protected bool clockwise = false;
	[Range(0f, 1f)]
	[SerializeField]
	protected float fillAmount = 1f;

	public float Rotation
	{
		get { return rotation + 90f; }
		set
		{
			rotation = value;
			SetMaterial();
		}
	}

	public bool Clockwise
	{
		get { return clockwise; }
		set
		{
			clockwise = value;
			SetMaterial();
		}
	}

	public float FillAmount
	{
		get { return fillAmount; }
		set
		{
			fillAmount = value;
			SetMaterial();
		}
	}

	void Reset()
	{
		sprite = GetComponent<SpriteRenderer>();
		SetMaterial();
	}

	void Start ()
	{
		SetMaterial();
	}

	void OnValidate()
	{
		SetMaterial();
	}

	void SetMaterial()
	{
		if (!sprite)
		{
			Debug.LogError("No sprite set for UI fill", gameObject);
			return;
		}

		MaterialPropertyBlock block = new MaterialPropertyBlock();
		sprite.GetPropertyBlock(block);

		Vector3 scale = new Vector3(clockwise ? -1:1, 1, 1);
		Quaternion rot = Quaternion.Euler(0, 0, rotation + 90f);
		block.SetMatrix("_Rotation", Matrix4x4.TRS(Vector3.zero, rot, scale));
		block.SetFloat("_Fill", fillAmount);
		sprite.SetPropertyBlock(block);
	}
}
