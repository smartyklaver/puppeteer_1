using UnityEngine;

public class CameraMoveBetweenPoints : MonoBehaviour
{
    public float delay = 2f;      // How long to wait before starting
    public float moveTime = 3f;   // How long the move should take

    // Set these directly in the inspector
    public Vector3 startPosition;
    public Vector3 startRotation; // Euler angles

    public Vector3 endPosition;
    public Vector3 endRotation;   // Euler angles

    private float elapsed = 0f;
    private bool startMove = false;

    void Start()
    {
        // Place the camera at the starting values immediately
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(startRotation);

        // Wait before starting movement
        Invoke(nameof(BeginMove), delay);
    }

    void BeginMove()
    {
        startMove = true;
    }

    void Update()
    {
        if (!startMove) return;

        elapsed += Time.deltaTime;
        float t = elapsed / moveTime;

        // Makes the movement smooth instead of linear
        t = Mathf.SmoothStep(0, 1, t);

        transform.position = Vector3.Lerp(startPosition, endPosition, t);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), t);
    }
}
