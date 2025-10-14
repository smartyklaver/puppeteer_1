using UnityEngine;

public class SpineController : MonoBehaviour
{
    [Header("Spine Setup")]
    public Transform spine;          // Sleep hier het Spine object in (Armature/Hips/Spine)
    public float rotationSpeed = 50f;
    public float minAngle = -30f;    // Hoe ver hij achterover mag buigen
    public float maxAngle = 60f;     // Hoe ver hij voorover mag buigen
    public float initialStartAngle = 0f;

    private float currentAngle;
    private float initialXAngle;
    private float fixedYAngle;
    private float fixedZAngle;

    [Header("Input Toetsen")]
    public KeyCode bendForwardKey = KeyCode.A;
    public KeyCode bendBackwardKey = KeyCode.Q;

    void Start()
    {
        if (spine != null)
        {
            Vector3 startRotation = spine.localEulerAngles;
            initialXAngle = startRotation.x;
            fixedYAngle = startRotation.y;
            fixedZAngle = startRotation.z;
            currentAngle = initialStartAngle;
        }
    }

    void Update()
    {
        if (spine == null) return;

        if (Input.GetKey(bendForwardKey))
            currentAngle += rotationSpeed * Time.deltaTime;

        if (Input.GetKey(bendBackwardKey))
            currentAngle -= rotationSpeed * Time.deltaTime;

        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        spine.localRotation = Quaternion.Euler(
            currentAngle + initialXAngle,
            fixedYAngle,
            fixedZAngle
        );
    }
}
