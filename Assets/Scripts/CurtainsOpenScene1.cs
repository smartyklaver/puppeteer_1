using UnityEngine;
using System.Collections;

public class CurtainsOpenSceneOne : MonoBehaviour
{
    public CartController cartController;

    [Header("Curtain Settings")]
    public Vector3 closedPosition = new Vector3(0f, 0f, 0f);
    public Vector3 openOffset = new Vector3(3f, 0f, 0f);
    public float speed = 2f;

    public float delayBeforeOpening = 6f;

    private Vector3 openPosition;

    void Start()
    {
        transform.position = closedPosition;
        openPosition = closedPosition + openOffset;

        StartCoroutine(OpenCurtain());
    }

    IEnumerator OpenCurtain()
    {
        //Wait before moving the curtain
        yield return new WaitForSeconds(delayBeforeOpening);

        // Now start moving the curtain
        while (Vector3.Distance(transform.position, openPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = openPosition;
    }
}
