using UnityEngine;

public class ShoulderController : MonoBehaviour
{
    [Header("Shoulder Setup")]
    public Transform leftShoulder;
    public Transform rightShoulder;
    public float rotationSpeed = 50f;
    public float currentValue = 0.5f; // startpositie (tussen 0 en 1)

    [Header("Left Arm Rotation Range")]
    public Vector3 leftLowRotation = new Vector3(5.215f, -100.063f, -72.656f);
    public Vector3 leftHighRotation = new Vector3(-2.258f, 89.23f, -107.951f);

    [Header("Input Keys")]
    public KeyCode leftUpKey = KeyCode.Z;
    public KeyCode leftDownKey = KeyCode.S;
    public KeyCode rightUpKey = KeyCode.E;
    public KeyCode rightDownKey = KeyCode.D;

    private float leftValue;
    private float rightValue;

    void Start()
    {
        leftValue = currentValue;
        rightValue = currentValue;
    }

    void Update()
    {
        // === Left Arm ===
        if (leftShoulder != null)
        {
            if (Input.GetKey(leftUpKey))
                leftValue += rotationSpeed * Time.deltaTime * 0.01f;
            if (Input.GetKey(leftDownKey))
                leftValue -= rotationSpeed * Time.deltaTime * 0.01f;

            leftValue = Mathf.Clamp01(leftValue);

            Quaternion leftRot = Quaternion.Euler(Vector3.Lerp(leftLowRotation, leftHighRotation, leftValue));
            leftShoulder.localRotation = leftRot;
        }

        // === Right Arm (Mirrored) ===
        if (rightShoulder != null)
        {
            if (Input.GetKey(rightUpKey))
                rightValue += rotationSpeed * Time.deltaTime * 0.01f;
            if (Input.GetKey(rightDownKey))
                rightValue -= rotationSpeed * Time.deltaTime * 0.01f;

            rightValue = Mathf.Clamp01(rightValue);

            // Spiegelen van linkerarm rotatie (Y en Z negatief)
            Vector3 rightLow = new Vector3(leftLowRotation.x, -leftLowRotation.y, -leftLowRotation.z);
            Vector3 rightHigh = new Vector3(leftHighRotation.x, -leftHighRotation.y, -leftHighRotation.z);

            Quaternion rightRot = Quaternion.Euler(Vector3.Lerp(rightLow, rightHigh, rightValue));
            rightShoulder.localRotation = rightRot;
        }
    }
}
