using UnityEngine;

public class NeckController : MonoBehaviour
{
    [Header("Neck Setup")]
    public Transform neck;              // Sleep hier het nek- of hoofdobject in
    public float rotationSpeed = 50f;
    public float minAngle = -40f;       // Hoe ver naar beneden
    public float maxAngle = 40f;        // Hoe ver naar boven

    [Header("Start Hoek Correctie")]
    public float initialStartAngle = 0f; // Startpositie

    [Header("Controls")]
    public KeyCode lookUpKey = KeyCode.UpArrow;      // Omhoog kijken
    public KeyCode lookDownKey = KeyCode.DownArrow;  // Omlaag kijken

    public bool enableHorizontalRotation = false;    // aan/uit voor links-rechts

    private float verticalAngle;   // op/neer
    private float horizontalAngle; // links/rechts

    // Startrotatie bewaren
    private float initialX;
    private float initialY;
    private float initialZ;

    void Start()
    {
        if (neck != null)
        {
            Vector3 startRotation = neck.localEulerAngles;
            initialX = startRotation.x;
            initialY = startRotation.y;
            initialZ = startRotation.z;
            verticalAngle = initialStartAngle;
        }
    }

    void Update()
    {
        if (neck == null) return;

        // Op/neer rotatie (voor/achter buigen)
        if (Input.GetKey(lookDownKey))
            verticalAngle += rotationSpeed * Time.deltaTime;
        if (Input.GetKey(lookUpKey))
            verticalAngle -= rotationSpeed * Time.deltaTime;

        verticalAngle = Mathf.Clamp(verticalAngle, minAngle, maxAngle);

        

        // Toepassen van rotatie
        neck.localRotation = Quaternion.Euler(
            initialX + verticalAngle,
            initialY + (enableHorizontalRotation ? horizontalAngle : 0f),
            initialZ
        );
    }
}
