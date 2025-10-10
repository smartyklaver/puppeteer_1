using UnityEngine;

public class ShoulderController : MonoBehaviour
{
    [Header("Shoulder Setup")]
    public Transform shoulder;    
    public float rotationSpeed = 50f;
    public float minAngle = -30f;
    public float maxAngle = 60f;

    private float currentAngle; 

    // NIEUWE FUNCTIE: Start
    void Start()
    {
        if (shoulder != null)
        {
            
            // Pas de X aan naar Y of Z als u een van die assen gebruikt.
            currentAngle = shoulder.localEulerAngles.z;

           
            if (currentAngle > 180f)
            {
                currentAngle -= 360f;
            }
        }
    }

    void Update()
    {
        if (shoulder == null) return;

        
        if (Input.GetKey(KeyCode.W))
            currentAngle += rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S))
            currentAngle -= rotationSpeed * Time.deltaTime;

        // Clamp to avoid extreme rotations
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        // Apply rotation around local X-axis (pitch)
        shoulder.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}