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
    private Coroutine curtainRoutine;

    void Awake()
    {
        openPosition = closedPosition + openOffset;
    }

    // Timeline can call this
    public void StartCurtain()
    {
        if (curtainRoutine != null)
            StopCoroutine(curtainRoutine);

        curtainRoutine = StartCoroutine(OpenCurtain());
    }

    // Timeline can call this too
    public void ResetCurtain()
    {
        transform.position = closedPosition;
        OnReset?.Invoke();
    }

    IEnumerator OpenCurtain()
    {
        transform.position = closedPosition;

        while (Vector3.Distance(transform.position, openPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = openPosition;
      //  OnCurtainOpened?.Invoke();
    }

    // Timeline can call this
    public void ReplayEverything()
    {
        spinecontroller.ReplayPuppetSpine();
        shouldercontroller.ReplayPuppetShoulders();
    }
}
