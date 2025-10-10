using UnityEngine;

public class ShoulderController : MonoBehaviour
{
    [Header("Shoulder Setup")]
    public Transform shoulder;    // Assign in Inspector
    public float rotationSpeed = 50f;
    public float minAngle = -30f;
    public float maxAngle = 60f;

    private float currentAngle; // Verwijder de initialisatie (= 0f) hier

    // NIEUWE FUNCTIE: Start
    void Start()
    {
        if (shoulder != null)
        {
            // Haal de huidige lokale hoek op de X-as op bij de start van het spel.
            // Pas de X aan naar Y of Z als u een van die assen gebruikt.
            currentAngle = shoulder.localEulerAngles.x;

            // Zorg dat de hoek correct wordt verwerkt (vooral belangrijk als de hoek negatief is)
            if (currentAngle > 180f)
            {
                currentAngle -= 360f;
            }
        }
    }

    void Update()
    {
        if (shoulder == null) return;

        // ... rest van de input code blijft hetzelfde ...

        // Control input
        if (Input.GetKey(KeyCode.W))
            currentAngle += rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S))
            currentAngle -= rotationSpeed * Time.deltaTime;

        // Clamp to avoid extreme rotations
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        // Apply rotation around local X-axis (pitch)
        // Pas dit aan naar de Y of Z-as die u heeft gevonden
        shoulder.localRotation = Quaternion.Euler(currentAngle, 0f, 0f);
    }
}