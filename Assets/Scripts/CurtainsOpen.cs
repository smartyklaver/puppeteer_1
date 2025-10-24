using UnityEngine;
using System.Collections;

public class CurtainMove : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(3f, 0f, 0f); // how far to move right
    public float speed = 2f; // how fast to move

    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + openOffset;

        StartCoroutine(OpenCurtain());
    }

    IEnumerator OpenCurtain()
    {
        while (Vector3.Distance(transform.position, openPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = openPosition; // snap exactly to final spot
    }
}
