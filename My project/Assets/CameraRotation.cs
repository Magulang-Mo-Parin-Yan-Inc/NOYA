using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;             // The player
    public Vector3 offset = new Vector3(0, 3, -6);
    public float rotationSpeed = 120f;
    public float pitchMin = -30f;
    public float pitchMax = 60f;

    private float yaw = 0f;
    private float pitch = 15f;

    void LateUpdate()
    {
        if (target == null) return;

        // Mouse input
        yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // Apply rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f); // Adjust vertical look if needed
    }
}