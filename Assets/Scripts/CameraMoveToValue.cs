using UnityEngine;

public class CameraMoveBetweenPoints : MonoBehaviour
{
    public float delay = 2f;      // How long to wait before starting
    public float moveTime = 3f;   // How long the move should take

    public Vector3 startPosition;
    public Vector3 startRotation;

    public Vector3 endPosition;
    public Vector3 endRotation;

    private float elapsed = 0f;
    private bool startMove = false;
   
    

    void Start()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(startRotation);

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
        t = Mathf.SmoothStep(0, 1, t);

        transform.position = Vector3.Lerp(startPosition, endPosition, t);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), t);

       
    }
}
