using UnityEngine;

public class CameraSizeAdjuster : MonoBehaviour
{
    public float size16_9 = 24.13123f; // Camera size for 16:9 aspect ratio
    public float size16_10 = 24.13123f; // Camera size for 16:10 aspect ratio
    private float tolerance = 0.05f;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        float aspectRatio = (float)Screen.width / Screen.height;
        if (Mathf.Abs(aspectRatio - 16f / 9f) < tolerance)
        {
            cam.orthographicSize = size16_9;
        }
        else if (Mathf.Abs(aspectRatio - 16f / 10f) < tolerance)
        {
            cam.orthographicSize = size16_10;
        }
        else
        {
            // Handle other aspect ratios if needed
            cam.orthographicSize = size16_9; // Default to 16:9 size
        }
    }

    void Update()
    {

    }
}