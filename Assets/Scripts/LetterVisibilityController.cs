// LetterVisibilityController.cs
using UnityEngine;

public class LetterVisibilityController : MonoBehaviour
{
    // Drag the specific letter object into this slot in the Inspector.
    public GameObject letterObject;

    // Deze functie wordt aangeroepen door de Timeline Signal Emitter en andere scripts
    public void ShowLetter()
    {
        if (letterObject != null)
        {
            letterObject.SetActive(true);
            Debug.Log("Letter made visible.");
        }
    }

    // NIEUW: Functie om de letter onzichtbaar te maken (voor gebruik door andere controllers)
    public void HideLetter()
    {
        if (letterObject != null)
        {
            letterObject.SetActive(false);
            Debug.Log("Letter made invisible.");
        }
    }
}