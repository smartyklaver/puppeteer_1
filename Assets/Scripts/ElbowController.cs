using UnityEngine;

public class ElbowController : MonoBehaviour
{
    [Header("Left Arm Setup")]
    public Transform leftForearm;
    public Transform leftHand;

    [Header("Right Arm Setup")]
    public Transform rightForearm;
    public Transform rightHand;

    [Header("Settings")]
    public float bendSpeed = 50f;
    public float rotateSpeed = 80f;

    public float minBendAngle = 0f;     // volledig gestrekt
    public float maxBendAngle = 120f;   // maximaal gebogen

    public float palmUpAngle = 0f;      // 0° = handpalm volledig naar boven
    public float palmDownAngle = 180f;  // 180° = handpalm naar beneden

    [Header("Left Arm Controls")]
    public KeyCode bendUpLeft = KeyCode.Q;
    public KeyCode bendDownLeft = KeyCode.A;
    public KeyCode rotatePalmLeftUp = KeyCode.E;
    public KeyCode rotatePalmLeftDown = KeyCode.D;

    [Header("Right Arm Controls")]
    public KeyCode bendUpRight = KeyCode.O;
    public KeyCode bendDownRight = KeyCode.L;
    public KeyCode rotatePalmRightUp = KeyCode.P;
    public KeyCode rotatePalmRightDown = KeyCode.Semicolon;

    // interne status
    private float leftBend, rightBend;
    private float leftPalmRotation, rightPalmRotation;

    private float leftInitialForearmX, leftInitialForearmY, leftInitialForearmZ;
    private float rightInitialForearmX, rightInitialForearmY, rightInitialForearmZ;

    private float leftInitialHandX, leftInitialHandY, leftInitialHandZ;
    private float rightInitialHandX, rightInitialHandY, rightInitialHandZ;

    void Start()
    {
        if (leftForearm != null)
        {
            Vector3 rot = leftForearm.localEulerAngles;
            leftInitialForearmX = rot.x;
            leftInitialForearmY = rot.y;
            leftInitialForearmZ = rot.z;
        }

        if (rightForearm != null)
        {
            Vector3 rot = rightForearm.localEulerAngles;
            rightInitialForearmX = rot.x;
            rightInitialForearmY = rot.y;
            rightInitialForearmZ = rot.z;
        }

        if (leftHand != null)
        {
            Vector3 rot = leftHand.localEulerAngles;
            leftInitialHandX = rot.x;
            leftInitialHandY = rot.y;
            leftInitialHandZ = rot.z;
        }

        if (rightHand != null)
        {
            Vector3 rot = rightHand.localEulerAngles;
            rightInitialHandX = rot.x;
            rightInitialHandY = rot.y;
            rightInitialHandZ = rot.z;
        }
    }

    void Update()
    {
        HandleArm(
            leftForearm, leftHand,
            bendUpLeft, bendDownLeft, rotatePalmLeftUp, rotatePalmLeftDown,
            ref leftBend, ref leftPalmRotation,
            leftInitialForearmX, leftInitialForearmY, leftInitialForearmZ,
            leftInitialHandX, leftInitialHandY, leftInitialHandZ
        );

        HandleArm(
            rightForearm, rightHand,
            bendUpRight, bendDownRight, rotatePalmRightUp, rotatePalmRightDown,
            ref rightBend, ref rightPalmRotation,
            rightInitialForearmX, rightInitialForearmY, rightInitialForearmZ,
            rightInitialHandX, rightInitialHandY, rightInitialHandZ
        );
    }

    void HandleArm(
        Transform forearm, Transform hand,
        KeyCode bendUp, KeyCode bendDown, KeyCode palmUp, KeyCode palmDown,
        ref float bendAngle, ref float palmRot,
        float foreX, float foreY, float foreZ,
        float handX, float handY, float handZ
    )
    {
        if (forearm == null || hand == null) return;

        // ---- HAND PALM ROTATION ----
        if (Input.GetKey(palmUp))
            palmRot -= rotateSpeed * Time.deltaTime;  // supinatie (naar boven)
        if (Input.GetKey(palmDown))
            palmRot += rotateSpeed * Time.deltaTime;  // pronatie (naar beneden)
        palmRot = Mathf.Clamp(palmRot, palmUpAngle, palmDownAngle);

        // ---- ELBOW BENDING ----
        // hoe meer handpalm naar boven, hoe groter buigbereik
        float palmFactor = Mathf.InverseLerp(palmDownAngle, palmUpAngle, palmRot); // 0 = palm down, 1 = palm up
        float effectiveMaxBend = Mathf.Lerp(30f, maxBendAngle, palmFactor); // beperkt bij palm down

        if (Input.GetKey(bendUp))
            bendAngle += bendSpeed * Time.deltaTime;
        if (Input.GetKey(bendDown))
            bendAngle -= bendSpeed * Time.deltaTime;

        bendAngle = Mathf.Clamp(bendAngle, minBendAngle, effectiveMaxBend);

        // ---- APPLY ROTATIONS ----
        // Elleboog buigt op X-as
        forearm.localRotation = Quaternion.Euler(foreX + bendAngle, foreY, foreZ);
        // Hand draait rond Z-as (of Y afhankelijk van je rig)
        hand.localRotation = Quaternion.Euler(handX, handY, handZ + palmRot);
    }
}
