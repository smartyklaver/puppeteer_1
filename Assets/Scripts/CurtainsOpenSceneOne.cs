using UnityEngine;
using UnityEngine.Events;

public class CurtainsOpenSceneOne : MonoBehaviour
{
    [Header("Curtain Settings")]
    public Vector3 closedPosition = new Vector3(0f, 0f, 0f);
    public Vector3 openOffset = new Vector3(3f, 0f, 0f);
    public float speed = 2f;

    [Header("Events")]
    public UnityEvent OnCurtainsClosed = new UnityEvent();

    private Vector3 openPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isClosing = false;

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
        isClosing = false;
    }

    // Called by Timeline Signal Emitter to open the curtain
    public void StartCurtains()
    {
        targetPosition = openPosition;
        isMoving = true;
        isClosing = false;
    }

    // Called by TimelineRestarter to close the curtain
    public void CloseCurtains()
    {
        targetPosition = closedPosition;
        isMoving = true;
        isClosing = true;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Stop moving when the target is reached
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;

            // Fire event ONLY when the curtain has finished closing
            if (isClosing)
            {
                OnCurtainsClosed?.Invoke();
                isClosing = false;
            }
        }
    }
}
