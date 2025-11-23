using UnityEngine;

public class CurtainsOpenSceneOne : MonoBehaviour
{
    [Header("Curtain Settings")]
    public Vector3 closedPosition = new Vector3(0f, 0f, 0f);
    public Vector3 openOffset = new Vector3(3f, 0f, 0f);
    public float speed = 2f;

    private Vector3 openPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        ResetForTimeline();
    }

    // Snaps the curtain back to the starting point (closed)
    public void ResetForTimeline()
    {
        transform.position = closedPosition;
        openPosition = closedPosition + openOffset;
        targetPosition = closedPosition;
        isMoving = false;
    }

    // Aangeroepen door de Timeline Signal Emitter om het gordijn te openen.
    public void StartCurtains()
    {
        targetPosition = openPosition;
        isMoving = true;
    }

    // Aangeroepen door TimelineRestarter om het gordijn te sluiten.
    public void CloseCurtains()
    {
        targetPosition = closedPosition;
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving) return;

        // Beweeg het gordijn vloeiend naar de ingestelde doelpositie
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Stop de beweging wanneer de doelpositie is bereikt
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }
}