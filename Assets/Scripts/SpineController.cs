using UnityEngine;

public class SpineController : MonoBehaviour
{
    [Header("Spine Setup")]
    public Transform spine;          // Sleep hier het Spine object in (Armature/Hips/Spine)
    public float rotationSpeed = 50f;
    public float initialStartAngle = 0f;

    private float currentAngle;
    private float initialZAngle;
    private float fixedXAngle;
    private float fixedYAngle;

    [Header("Input Toetsen")]
    public KeyCode bendForwardKey = KeyCode.A;
    public KeyCode bendBackwardKey = KeyCode.Q;

    void Start()
    {
        if (spine != null)
        {
            // lees huidige rotatie
            Vector3 startRotation = spine.localEulerAngles;
            fixedXAngle = startRotation.x;
            fixedYAngle = startRotation.y;
            initialZAngle = startRotation.z;

            currentAngle = initialStartAngle;
        }
    }

    void Update()
    {
        if (spine == null) return;

        // Draai vooruit/achteruit
        if (Input.GetKey(bendForwardKey))
            currentAngle += rotationSpeed * Time.deltaTime;

        if (Input.GetKey(bendBackwardKey))
            currentAngle -= rotationSpeed * Time.deltaTime;

        // Pas de rotatie toe alleen in Z
        spine.localRotation = Quaternion.Euler(
            fixedXAngle,
            fixedYAngle,
            initialZAngle + currentAngle
        );
    }
}
