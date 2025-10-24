using UnityEngine;
using System.Collections;

public class CurtainMove : MonoBehaviour
{
    [Header("Curtain Settings")]
    public Vector3 closedPosition = new Vector3(0f, 0f, 0f);  // where the curtain starts
    public Vector3 openOffset = new Vector3(3f, 0f, 0f);      // how far to move right
    public float speed = 2f;                                  // movement speed

    private Vector3 openPosition;

    void Start()
    {
        // Force the curtain to start closed
        transform.position = closedPosition;

        // Calculate the open position (right side)
        openPosition = closedPosition + openOffset;

        // Start opening the curtain
        StartCoroutine(OpenCurtain());
    }

    IEnumerator OpenCurtain()
    {
        while (Vector3.Distance(transform.position, openPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, speed * Time.deltaTime);
            yield return null;
        }

        // Snap exactly to final position
        transform.position = openPosition;
    }
}
