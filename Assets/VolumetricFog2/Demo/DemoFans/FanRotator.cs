using UnityEngine;

public class FanRotator : MonoBehaviour {
    Transform thisTransform;

    public float speed = 90;

    // Start is called before the first frame update
    void Start() {
        thisTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update() {
        thisTransform.Rotate(0, speed * Time.deltaTime, 0, Space.Self);
    }
}
