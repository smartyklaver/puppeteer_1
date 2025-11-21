using UnityEngine;

public class CurtainsOpenSceneOne : MonoBehaviour
{
    // You can remove this reference if not used:
    // public CartController cartController;

    [Header("Curtain Settings")]
    public Vector3 closedPosition = new Vector3(0f, 0f, 0f);
    public Vector3 openOffset = new Vector3(3f, 0f, 0f);
    public float speed = 2f;

    private Vector3 openPosition;
    private bool opening = false;

    void Start()
    {
        // Set the initial position on start
        ResetForTimeline();
    }

    // Called IMMEDIATELY by TimelineRestarter when Spacebar is pressed.
    // Snaps the curtain back to the starting point.
    public void ResetForTimeline()
    {
        transform.position = closedPosition;
        openPosition = closedPosition + openOffset;
        opening = false;
    }

    // Called by the Timeline Signal Emitter to start the opening movement.
    public void StartCurtains()
    {
        // We assume ResetForTimeline() has already been called via the button press,
        // so we just set the flag to start moving.
        opening = true;
    }

    void Update()
    {
        if (!opening) return;

        // Move curtain smoothly toward the open position
        transform.position = Vector3.MoveTowards(
            transform.position,
            openPosition,
            speed * Time.deltaTime
        );

        // Stop when reached
        if (Vector3.Distance(transform.position, openPosition) < 0.01f)
        {
            transform.position = openPosition;
            opening = false;
        }
    }
}