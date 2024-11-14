using UnityEngine;
using Unity.Netcode;

public class HeadBobbing : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float walkBobSpeed = 10f;
    public float walkBobAmount = 0.05f;
    public float sprintBobSpeed = 14f;
    public float sprintBobAmount = 0.1f;

    private float defaultYPos;
    private float timer;

    void Start()
    {
        // Save the default Y position of the camera
        defaultYPos = transform.localPosition.y;
    }

    

    public void Bob(float speed, bool isSprinting)
    {
        // Set the bobbing speed and amount based on sprinting or walking
        float bobSpeed = isSprinting ? sprintBobSpeed : walkBobSpeed;
        float bobAmount = isSprinting ? sprintBobAmount : walkBobAmount;

        // Calculate bobbing using a sinusoidal wave
        timer += Time.deltaTime * bobSpeed;
        float newY = defaultYPos + Mathf.Sin(timer) * bobAmount;
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }

    public void ResetPosition()
    {
        // Reset to default position when not moving
        timer = 0;
        transform.localPosition = new Vector3(transform.localPosition.x, defaultYPos, transform.localPosition.z);
    }
}

