using UnityEngine;

public class ShieldBlockZone : MonoBehaviour
{
    [Header("Shield Detection")]
    public bool shieldLocked = false;
    public float requiredHoldTime = 0.4f;

    float timer = 0f;
    bool shieldInside = false;

    public AudioSource instructionAudio;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowZone()
    {
        shieldLocked = false;
        shieldInside = false;
        timer = 0f;
        gameObject.SetActive(true);

        if (instructionAudio != null)
            instructionAudio.Play();
    }

void Update()
{
    if (!shieldInside) return;

    var cm = FindObjectOfType<CinematicManager>();
    if (cm != null && cm.IsSpacePressed())   // of Arduino-knop
    {
        shieldLocked = true;
        Debug.Log("🛡 Shield confirmed with quick press!");

        cm.SendLampStateForced(false);       // LAMPJE UIT

        gameObject.SetActive(false);         // Zone verbergen indien gewenst
    }
}




void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Shield"))
    {
        shieldInside = true;
        Debug.Log("🟩 Shield entered the zone...");

        var cm = FindObjectOfType<CinematicManager>();
        if (cm != null)
            cm.SendLampStateForced(true);   // LAMPJE AAN
    }
}


void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Shield"))
    {
        shieldInside = false;
        Debug.Log("🟥 Shield left the zone");

        var cm = FindObjectOfType<CinematicManager>();
        if (cm != null)
            cm.SendLampStateForced(false);  // LAMPJE UIT
    }
}

}
