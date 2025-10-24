using UnityEngine;

public class CurtainsOpen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
    void Update()
    {
        if (timer < moveDuration)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            timer += Time.deltaTime;
        }
    }
}
