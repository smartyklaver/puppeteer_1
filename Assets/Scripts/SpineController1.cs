using UnityEngine;

public class SpineController1 : MonoBehaviour
{
    [Header("Spine Setup")]
    public Transform spine;          // Sleep hier het Spine object in (Armature/Hips/Spine)
  //  public float rotationSpeed = 50f;
    public float initialStartAngle = 0f;

    private float currentAngle;
    private float initialZAngle;
    private float fixedXAngle;
    private float fixedYAngle;

    private UdpReceiver udp;

    // [Header("Input Toetsen")]
    // public KeyCode bendForwardKey = KeyCode.A;
    // public KeyCode bendBackwardKey = KeyCode.Q;

    void Start()
    {
        udp = FindObjectOfType<UdpReceiver>();
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

        // // Draai vooruit/achteruit
        // if (Input.GetKey(bendForwardKey))
        //     currentAngle += rotationSpeed * Time.deltaTime;

        // if (Input.GetKey(bendBackwardKey))
        //     currentAngle -= rotationSpeed * Time.deltaTime;

        float torsoRaw = udp.LatestData.torsoBend;
      //  float torsoNorm = Mathf.InverseLerp(1, 0.46f, torsoRaw); 
        currentAngle = Mathf.Lerp(-20f, 90f, torsoRaw);

        // Pas de rotatie toe alleen in Z
        spine.localRotation = Quaternion.Euler(
            fixedXAngle,
            fixedYAngle,
            initialZAngle + currentAngle
        );
    }
}
