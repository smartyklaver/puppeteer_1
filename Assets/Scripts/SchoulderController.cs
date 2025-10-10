using UnityEngine;

public class ShoulderController : MonoBehaviour
{
    // === Instellingen in Inspector ===
    [Header("Shoulder Setup")]
    public Transform shoulder;       // Sleep Arm.L hierin
    public float rotationSpeed = 50f;
    public float minAngle = -30f;
    public float maxAngle = 60f;

    [Header("Start Hoek Correctie")]
    public float initialStartAngle = 0f;    // De hoek (tussen min/max) waar de arm begint te bewegen

    private float currentAngle;
    private float initialXAngle; // De startrotatie van de X-as (de bewegingsas)
    private float fixedYAngle;   // De startrotatie van de Y-as (blijft vast)
    private float fixedZAngle;   // De startrotatie van de Z-as (blijft vast)

    void Start()
    {
        if (shoulder != null)
        {
            // Lees de huidige EULER hoeken in (de starthouding van het model)
            Vector3 startRotation = shoulder.localEulerAngles;

            // Sla alle startrotaties op:
            initialXAngle = startRotation.x; // De basis X-rotatie
            fixedYAngle = startRotation.y;   // Vaste Y-rotatie
            fixedZAngle = startRotation.z;   // Vaste Z-rotatie

            // Stel de variabele in op de gewenste startwaarde (bv. 0).
            currentAngle = initialStartAngle;
        }
    }

    void Update()
    {
        if (shoulder == null) return;

        // Input verwerking (W en S)
        if (Input.GetKey(KeyCode.W))
            currentAngle += rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S))
            currentAngle -= rotationSpeed * Time.deltaTime;

        // Hoeken beperken
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        // Rotatie toepassen op de X-as:
        // De 'currentAngle' (de beweging) wordt opgeteld bij de 'initialXAngle' (de startpositie).
        // Y en Z blijven vast op hun startpositie.
        shoulder.localRotation = Quaternion.Euler(
            currentAngle + initialXAngle,
            fixedYAngle,
            fixedZAngle
        );
    }
}