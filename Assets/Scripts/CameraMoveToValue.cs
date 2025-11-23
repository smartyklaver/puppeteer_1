using UnityEngine;

public class CameraMovetoValue : MonoBehaviour
{
    public float moveTime = 3f;  // Hoe lang de beweging duurt

    public Vector3 startPosition;
    public Vector3 startRotation;

    public Vector3 endPosition;
    public Vector3 endRotation;

    private float elapsed = 0f;
    private bool startMove = false;
    private bool movingForward = true; // NIEUW: Bepaalt of elapsed oploopt (zoom in) of afloopt (zoom uit).

    void Start()
    {
        // Zet de startpositie en -rotatie
        ResetForTimeline();
    }

    // Snapt de camera terug naar het startpunt.
    public void ResetForTimeline()
    {
        elapsed = 0f;
        startMove = false;
        movingForward = true; // Zorg ervoor dat de volgende beweging 'Zoom In' is.
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(startRotation);
    }

    // Called by the Timeline Signal Emitter to begin movement (Zoom IN).
    public void BeginMove()
    {
        movingForward = true; // Zet de richting op 'vooruit'
        startMove = true;
        elapsed = 0f; // Start de tijd bij nul
    }

    // Called by TimelineRestarter to move the camera back (Zoom OUT).
    public void ReturnToStart()
    {
        // Zet de positie op END, en start met terugtellen
        transform.position = endPosition;
        transform.rotation = Quaternion.Euler(endRotation);

        movingForward = false; // Zet de richting op 'achteruit'
        startMove = true;
        elapsed = moveTime; // Start de tijd op de maximale duur
    }

    void Update()
    {
        if (!startMove) return;

        // 1. Richtingscontrole: Tijd oploopt of afloopt
        if (movingForward)
        {
            elapsed += Time.deltaTime;
        }
        else
        {
            elapsed -= Time.deltaTime;
        }

        // 2. Clamping en stoppen
        if (elapsed >= moveTime)
        {
            elapsed = moveTime;
            if (movingForward) startMove = false; // Stop wanneer volledig ingezoomd
        }
        if (elapsed <= 0)
        {
            elapsed = 0;
            if (!movingForward) startMove = false; // Stop wanneer volledig uitgezoomd
        }

        // 3. Beweging (de LERP loopt altijd van startPosition naar endPosition)
        float t = elapsed / moveTime;
        t = Mathf.SmoothStep(0, 1, t);

        // De Lerp gebruikt 't' (van 0-1 of 1-0) om de camera correct te bewegen.
        transform.position = Vector3.Lerp(startPosition, endPosition, t);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), t);
    }
}