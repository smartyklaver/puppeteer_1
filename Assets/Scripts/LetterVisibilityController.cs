using UnityEngine;

public class LetterVisibilityController : MonoBehaviour
{
    // Drag the specific letter object into this slot in the Inspector.
    public GameObject letterObject;

    // This function is called by a Timeline Signal Emitter.
    public void ShowLetter()
    {
        if (letterObject != null)
        {
            letterObject.SetActive(true);
            Debug.Log("Letter made visible by Timeline Signal.");
        }
    }
}