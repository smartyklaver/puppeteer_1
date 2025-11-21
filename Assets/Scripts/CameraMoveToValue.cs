using UnityEngine;

public class CameraMovetoValue : MonoBehaviour
{
    // public float delay = 2f; // Removed, handled by Timeline
    public float moveTime = 3f;   // How long the move should take

    public Vector3 startPosition;
    public Vector3 startRotation;

    public Vector3 endPosition;
    public Vector3 endRotation;

    private float elapsed = 0f;
    private bool startMove = false;

    void Start()
    {
        // Set initial position
        ResetForTimeline();
    }

    // Called IMMEDIATELY by TimelineRestarter when Spacebar is pressed.
    public void ResetForTimeline()
    {
        elapsed = 0f;
        startMove = false;
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(startRotation);
    }

    // Called by the Timeline Signal Emitter to begin movement.
    public void BeginMove()
    {
        startMove = true;
        elapsed = 0f;
    }

    void Update()
    {
        if (!startMove) return;

        elapsed += Time.deltaTime;

        // Safety check to prevent overflow
        if (elapsed > moveTime) elapsed = moveTime;

        float t = elapsed / moveTime;
        // Use smoothstep for easing
        t = Mathf.SmoothStep(0, 1, t);

        transform.position = Vector3.Lerp(startPosition, endPosition, t);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), t);
    }
}