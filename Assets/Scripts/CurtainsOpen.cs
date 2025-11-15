using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CurtainMove : MonoBehaviour
{
    public SpineController1 spinecontroller;
    public ShoulderController1 shouldercontroller;

    public UnityEvent OnCurtainOpened;
    public UnityEvent OnReset;
    
    [Header("Curtain Settings")]
    public Vector3 closedPosition = new Vector3(0f, 0f, 0f);
    public Vector3 openOffset = new Vector3(3f, 0f, 0f);
    public float speed = 2f;

    private Vector3 openPosition;

    public KeyCode RestartKey = KeyCode.R;

    private Coroutine curtainRoutine;

    private int framecounter = 0; 

    void Start()
    {
        transform.position = closedPosition;
        openPosition = closedPosition + openOffset;

        curtainRoutine = StartCoroutine(OpenCurtain());
    }

    IEnumerator OpenCurtain()
    {
        int framecounter = 0; 
        transform.position = closedPosition;

        while (Vector3.Distance(transform.position, openPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = openPosition;
        OnCurtainOpened?.Invoke();
    }

    void Update()
    {
        if (Input.GetKeyDown(RestartKey))
        {
            // Stop current coroutine if active
            if (curtainRoutine != null)
                StopCoroutine(curtainRoutine);

            spinecontroller.ReplayPuppetSpine();
            shouldercontroller.ReplayPuppetShoulders();
            OnReset?.Invoke();
            curtainRoutine = StartCoroutine(OpenCurtain());
        }
    }
}
