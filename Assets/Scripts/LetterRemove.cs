using UnityEngine;
using UnityEngine.Events; // REQUIRED for UnityEvent

public class InputRemover : MonoBehaviour
{
    public GameObject letterObject;

    // This is the public output signal that the TimelineRestarter listens to
    public UnityEvent OnSpacebarPressed = new UnityEvent();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformActionAndSignal();
        }
    }

    public void PerformActionAndSignal()
    {
        // 1. Perform the primary job of this script (removing the letter)
        if (letterObject != null)
        {
            letterObject.SetActive(false);
        }

        // 2. Trigger the signal (which the Restarter will hear)
        OnSpacebarPressed.Invoke();
    }
}