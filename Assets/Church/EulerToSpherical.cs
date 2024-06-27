using UnityEngine;

public class EulerToSpherical : MonoBehaviour
{
    public Vector3 eulerAngles; // Euler angles (in degrees)
    public float phi; // Phi (angle from the positive z-axis)
    public float theta; // Theta (angle from the positive x-axis in the x-y plane)

    // Convert Euler angles to spherical coordinates
    public void UpdateSphericalCoordinates()
    {
        // Convert Euler angles to radians
        float xRad = Mathf.Deg2Rad * transform.eulerAngles.x;
        float yRad = Mathf.Deg2Rad * transform.eulerAngles.y;
        float zRad = Mathf.Deg2Rad * transform.eulerAngles.z;

        // Calculate spherical coordinates
        phi = Mathf.Acos(Mathf.Cos(yRad) * Mathf.Cos(xRad));
        theta = Mathf.Atan2(Mathf.Sin(yRad) * Mathf.Cos(xRad), Mathf.Sin(xRad));
    }
}
