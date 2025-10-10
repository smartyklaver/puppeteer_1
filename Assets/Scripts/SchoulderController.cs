using UnityEngine;

public class ShoulderController : MonoBehaviour
{
    [Header("Shoulder Setup")]
    public Transform shoulder;   // Assign in Inspector
    public float rotationSpeed = 50f;  // Degrees per second
    public float minAngle = -30f; // How far down the arm can go
    public float maxAngle = 60f;  // How far up the arm can go

    private float currentAngle = 0f;

    void Update()
    {
        if (shoulder == null) return;

        // Control input
        if (Input.GetKey(KeyCode.W))
            currentAngle += rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S))
            currentAngle -= rotationSpeed * Time.deltaTime;

        // Clamp to avoid extreme rotations
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        // Apply rotation around local X-axis (pitch)
        shoulder.localRotation = Quaternion.Euler(currentAngle, 0f, 0f);
    }
}
