using UnityEngine;

public class AddImpulseToRigidbody : MonoBehaviour
{
    public Vector3 Impulse = new Vector3(0f, 1f, 0f);

    protected Rigidbody _rigidbody;
    public Rigidbody Rigidbody
    {
        get
        {
            if (_rigidbody == null)
            {
                _rigidbody = this.GetComponent<Rigidbody>();
            }
            return _rigidbody;
        }
    }

	public void AddImpulse()
	{
        Rigidbody.AddForce(Impulse, ForceMode.Impulse);

    }
}
