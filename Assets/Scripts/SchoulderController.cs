using UnityEngine;

public class ShoulderController : MonoBehaviour
{
    [Header("Shoulder Setup")]
    public Transform leftShoulder;
    public Transform rightShoulder;
    public float rotationSpeed = 50f;
    public float minAngle = -30f;
    public float maxAngle = 60f;
    public float initialStartAngle = 0f;

    [Header("Offset Correction (use this to fix uneven poses)")]
    public float leftOffset = 0f;
    public float rightOffset = 0f;

    private float leftCurrentAngle;
    private float rightCurrentAngle;

    private float leftInitialX;
    private float leftY;
    private float leftZ;

    private float rightInitialX;
    private float rightY;
    private float rightZ;

    [Header("Input Keys")]
    public KeyCode leftUpKey = KeyCode.W;
    public KeyCode leftDownKey = KeyCode.S;
    public KeyCode rightUpKey = KeyCode.E;
    public KeyCode rightDownKey = KeyCode.D;

    void Start()
    {
        if (leftShoulder != null)
        {
            Vector3 rot = leftShoulder.localEulerAngles;
            leftInitialX = rot.x;
            leftY = rot.y;
            leftZ = rot.z;
            leftCurrentAngle = initialStartAngle;
        }

        if (rightShoulder != null)
        {
            Vector3 rot = rightShoulder.localEulerAngles;
            rightInitialX = rot.x;
            rightY = rot.y;
            rightZ = rot.z;
            rightCurrentAngle = initialStartAngle;
        }
    }

    void Update()
    {
        // === Left Arm ===
        if (leftShoulder != null)
        {
            if (Input.GetKey(leftUpKey))
                leftCurrentAngle += rotationSpeed * Time.deltaTime;
            if (Input.GetKey(leftDownKey))
                leftCurrentAngle -= rotationSpeed * Time.deltaTime;

            leftCurrentAngle = Mathf.Clamp(leftCurrentAngle, minAngle, maxAngle);
            leftShoulder.localRotation = Quaternion.Euler(
                leftCurrentAngle + leftInitialX + leftOffset,
                leftY,
                leftZ
            );
        }

        // === Right Arm ===
        if (rightShoulder != null)
        {
            if (Input.GetKey(rightUpKey))
                rightCurrentAngle += rotationSpeed * Time.deltaTime;
            if (Input.GetKey(rightDownKey))
                rightCurrentAngle -= rotationSpeed * Time.deltaTime;

            rightCurrentAngle = Mathf.Clamp(rightCurrentAngle, minAngle, maxAngle);
            rightShoulder.localRotation = Quaternion.Euler(
                rightCurrentAngle + rightInitialX + rightOffset,
                rightY,
                rightZ
            );
        }
    }
}
