using UnityEngine;

public class CurtainsOpenSceneOne : MonoBehaviour
{
    public CartController cartController;

    [Header("Curtain Settings")]
    public Vector3 closedPosition = new Vector3(0f, 0f, 0f);
    public Vector3 openOffset = new Vector3(3f, 0f, 0f);
    public float speed = 2f;

    private Vector3 openPosition;
    private bool opening = false;

    void Start()
    {
        transform.position = closedPosition;
        openPosition = closedPosition + openOffset;
    }

    // This is the method Timeline will call
    public void StartCurtains()
    {
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
