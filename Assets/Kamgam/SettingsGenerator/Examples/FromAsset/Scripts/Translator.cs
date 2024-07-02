using UnityEngine;

public class Translator : MonoBehaviour
{
	public float Speed = 1f;
	public float Amplitude = 1f;
    public Vector3 Direction = Vector3.up;
    public bool AlongOwnAxis = false;
    public float SinOffset = 0f;
    public bool ResetOnDisable = true;

    protected float _angleInRad;
	protected Vector3 _startPos;

	void Awake()
	{
        _startPos = transform.localPosition;
	}
	
    void Update()
    {
        _angleInRad += 0.03f * Speed * Time.deltaTime * 60f;
        var dir = Direction;
        if (AlongOwnAxis)
            dir = transform.localRotation * Direction;
        var pos = _startPos + dir * (Mathf.Sin(_angleInRad) + SinOffset) * Amplitude;
        transform.localPosition = pos;
    }

    void OnDisable()
    {
        if (ResetOnDisable)
        {
            transform.localPosition = _startPos;
            _angleInRad = 0f;
        }
    }

    public void Toggle()
    {
        enabled = !enabled;
    }
}
