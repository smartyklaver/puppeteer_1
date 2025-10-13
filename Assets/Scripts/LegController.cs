using UnityEngine;

public class LegController : MonoBehaviour
{
    [Header("Leg Setup")]
    public Transform leftThigh;   // Sleep hier Thigh.L in
    public Transform rightThigh;  // Sleep hier Thigh.R in

    public float rotationSpeed = 50f;
    public float minAngle = -90f;  // Hoe ver het been naar achter mag
    public float maxAngle = 30f;   // Hoe ver het been naar voren mag
    public float initialStartAngle = 0f;

    private float leftCurrentAngle;
    private float rightCurrentAngle;

    private float leftInitialX;
    private float rightInitialX;
    private float leftY;
    private float rightY;
    private float leftZ;
    private float rightZ;

    [Header("Input Toetsen (Links Been)")]
    public KeyCode leftForwardKey = KeyCode.R;   // Voorbeeld: E = linkerbeen vooruit
    public KeyCode leftBackwardKey = KeyCode.F;  // D = linkerbeen achteruit

    [Header("Input Toetsen (Rechts Been)")]
    public KeyCode rightForwardKey = KeyCode.T;  // I = rechterbeen vooruit
    public KeyCode rightBackwardKey = KeyCode.G; // K = rechterbeen achteruit

    void Start()
    {
        if (leftThigh != null)
        {
            Vector3 leftStart = leftThigh.localEulerAngles;
            leftInitialX = leftStart.x;
            leftY = leftStart.y;
            leftZ = leftStart.z;
            leftCurrentAngle = initialStartAngle;
        }

        if (rightThigh != null)
        {
            Vector3 rightStart = rightThigh.localEulerAngles;
            rightInitialX = rightStart.x;
            rightY = rightStart.y;
            rightZ = rightStart.z;
            rightCurrentAngle = initialStartAngle;
        }
    }

    void Update()
    {
        // === Linkerbeen ===
        if (leftThigh != null)
        {
            if (Input.GetKey(leftForwardKey))
                leftCurrentAngle += rotationSpeed * Time.deltaTime;
            if (Input.GetKey(leftBackwardKey))
                leftCurrentAngle -= rotationSpeed * Time.deltaTime;

            leftCurrentAngle = Mathf.Clamp(leftCurrentAngle, minAngle, maxAngle);
            leftThigh.localRotation = Quaternion.Euler(
                leftCurrentAngle + leftInitialX,
                leftY,
                leftZ
            );
        }

        // === Rechterbeen ===
        if (rightThigh != null)
        {
            if (Input.GetKey(rightForwardKey))
                rightCurrentAngle += rotationSpeed * Time.deltaTime;
            if (Input.GetKey(rightBackwardKey))
                rightCurrentAngle -= rotationSpeed * Time.deltaTime;

            rightCurrentAngle = Mathf.Clamp(rightCurrentAngle, minAngle, maxAngle);
            rightThigh.localRotation = Quaternion.Euler(
                rightCurrentAngle + rightInitialX,
                rightY,
                rightZ
            );
        }
    }
}
