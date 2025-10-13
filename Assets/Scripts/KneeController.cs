using UnityEngine;

public class KneeController : MonoBehaviour
{
    [Header("Left Knee Setup")]
    public Transform leftKnee;
    public float leftRotationSpeed = 50f;
    public float leftMinAngle = 0f;
    public float leftMaxAngle = 90f;
    public KeyCode leftBendKey = KeyCode.X;       // buigen
    public KeyCode leftStraightenKey = KeyCode.C; // strekken

    [Header("Right Knee Setup")]
    public Transform rightKnee;
    public float rightRotationSpeed = 50f;
    public float rightMinAngle = 0f;
    public float rightMaxAngle = 90f;
    public KeyCode rightBendKey = KeyCode.B;
    public KeyCode rightStraightenKey = KeyCode.N;

    private float leftCurrentAngle;
    private float rightCurrentAngle;

    // Startrotaties (voor behoud van oorspronkelijke houding)
    private float leftInitialX;
    private float leftY;
    private float leftZ;
    private float rightInitialX;
    private float rightY;
    private float rightZ;

    void Start()
    {
        if (leftKnee != null)
        {
            Vector3 startRotation = leftKnee.localEulerAngles;
            leftInitialX = startRotation.x;
            leftY = startRotation.y;
            leftZ = startRotation.z;
        }

        if (rightKnee != null)
        {
            Vector3 startRotation = rightKnee.localEulerAngles;
            rightInitialX = startRotation.x;
            rightY = startRotation.y;
            rightZ = startRotation.z;
        }
    }

    void Update()
    {
        if (leftKnee != null)
        {
            // Links buigen/strekken
            if (Input.GetKey(leftBendKey))
                leftCurrentAngle += leftRotationSpeed * Time.deltaTime;

            if (Input.GetKey(leftStraightenKey))
                leftCurrentAngle -= leftRotationSpeed * Time.deltaTime;

            leftCurrentAngle = Mathf.Clamp(leftCurrentAngle, leftMinAngle, leftMaxAngle);

            leftKnee.localRotation = Quaternion.Euler(
                leftInitialX + leftCurrentAngle,
                leftY,
                leftZ
            );
        }

        if (rightKnee != null)
        {
            // Rechts buigen/strekken
            if (Input.GetKey(rightBendKey))
                rightCurrentAngle += rightRotationSpeed * Time.deltaTime;

            if (Input.GetKey(rightStraightenKey))
                rightCurrentAngle -= rightRotationSpeed * Time.deltaTime;

            rightCurrentAngle = Mathf.Clamp(rightCurrentAngle, rightMinAngle, rightMaxAngle);

            rightKnee.localRotation = Quaternion.Euler(
                rightInitialX + rightCurrentAngle,
                rightY,
                rightZ
            );
        }
    }
}
