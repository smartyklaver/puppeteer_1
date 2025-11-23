// TickleDetector.cs
using UnityEngine;

public class TickleDetector : MonoBehaviour
{
    public CinematicManager manager;
    int frames = 0;
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Dragon")) return;
        frames++;
        if (frames > 20)
        {
            manager?.RegisterTickle();
            frames = 0;
        }
    }

    void OnTriggerExit(Collider other) { if (other.CompareTag("Dragon")) frames = 0; }
}
