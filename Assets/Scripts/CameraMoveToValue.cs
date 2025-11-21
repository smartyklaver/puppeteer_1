using UnityEngine;

public class CameraMovetoValue : MonoBehaviour
{
   
    public float moveTime = 3f;

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

       
    }

    // CHANGED: Added 'public' so the Signal Receiver can find it
    public void BeginMove()
    {
        startMove = true;
    }

    void Update()
    {
        if (!startMove) return;

        elapsed += Time.deltaTime;

        // Added a safety check so it doesn't count up forever
        if (elapsed > moveTime) elapsed = moveTime;

        float t = elapsed / moveTime;
        t = Mathf.SmoothStep(0, 1, t);

        transform.position = Vector3.Lerp(startPosition, endPosition, t);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), t);
    }
}