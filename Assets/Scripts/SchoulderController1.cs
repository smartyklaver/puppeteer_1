using UnityEngine;

public class ShoulderController1 : MonoBehaviour
{
    [Header("Shoulder Setup")]
    public Transform leftShoulder;
    public Transform rightShoulder;
   // public float rotationSpeed = 100f;

    // [Header("Input Keys")]
    // public KeyCode leftClockwiseKey = KeyCode.Z;   // links arm draait mee met klok
    // public KeyCode leftCounterKey = KeyCode.S;     // links arm draait tegen klok in
    // public KeyCode rightClockwiseKey = KeyCode.E;
    // public KeyCode rightCounterKey = KeyCode.D;

    private float leftYAngle;
    private float rightYAngle;
    
    private UdpReceiver udp;

    void Start()
    {
        if (leftShoulder != null)
            leftYAngle = leftShoulder.localEulerAngles.z;

        if (rightShoulder != null)
            rightYAngle = rightShoulder.localEulerAngles.z;

        udp = FindObjectOfType<UdpReceiver>();
    }

    void Update()
    {
        // === Left Arm ===
        if (leftShoulder != null)
        {
            // if (Input.GetKey(leftClockwiseKey))
            //     leftYAngle += rotationSpeed * Time.deltaTime;
            // if (Input.GetKey(leftCounterKey))
            //     leftYAngle -= rotationSpeed * Time.deltaTime;
             leftYAngle =  udp.LatestData.leftShoulderValue + 90f ;

            // 360° rotatie behouden
            if (leftYAngle > 360f) leftYAngle -= 360f;
            if (leftYAngle < 0f) leftYAngle += 360f;

            Vector3 euler = leftShoulder.localEulerAngles;
            euler.z = leftYAngle;
            leftShoulder.localEulerAngles = euler;
        }

        // === Right Arm (Mirrored) ===
        if (rightShoulder != null)
        {
            // if (Input.GetKey(rightClockwiseKey))
            //     rightYAngle += rotationSpeed * Time.deltaTime;
            // if (Input.GetKey(rightCounterKey))
            //     rightYAngle -= rotationSpeed * Time.deltaTime;

            rightYAngle =   90f + udp.LatestData.rightShoulderValue;

            if (rightYAngle > 360f) rightYAngle -= 360f;
            if (rightYAngle < 0f) rightYAngle += 360f;

            Vector3 euler = rightShoulder.localEulerAngles;
            euler.z = rightYAngle;
            rightShoulder.localEulerAngles = euler;
        }
    }
}
