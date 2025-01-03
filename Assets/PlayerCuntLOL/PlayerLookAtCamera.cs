using UnityEngine;

public class PlayerLookWithCamera : MonoBehaviour
{
    public Transform cameraTransform; // Assign the Main Camera
    public Transform upperBodyTarget; // Assign the upper body bone (e.g., Spine_02 or Chest)
    public float cameraPitchLimit = 30f; // Limit vertical rotation (adjust as needed)
    public float rotationSpeed = 5f; // Smoothing speed for rotation

    void Update()
    {
        if (cameraTransform == null || upperBodyTarget == null)
        {
            Debug.LogWarning("CameraTransform or UpperBodyTarget is not assigned!");
            return;
        }

        // Get the camera's local rotation (x-axis for pitch)
        Vector3 cameraEuler = cameraTransform.localEulerAngles;

        // Convert the pitch to a range of -180 to 180
        if (cameraEuler.x > 180f) cameraEuler.x -= 360f;

        // Clamp the camera's pitch rotation
        float clampedPitch = Mathf.Clamp(cameraEuler.x, -cameraPitchLimit, cameraPitchLimit);

        /* Smoothly rotate the upper body only on the x-axis
        Quaternion targetRotation = Quaternion.Euler(clampedPitch, upperBodyTarget.localEulerAngles.y, upperBodyTarget.localEulerAngles.z);
        upperBodyTarget.localRotation = Quaternion.Slerp(upperBodyTarget.localRotation, targetRotation, rotationSpeed * Time.deltaTime);*/
    }
}
