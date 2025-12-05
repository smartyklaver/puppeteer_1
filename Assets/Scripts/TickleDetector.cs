using UnityEngine;

public class TickleDetector : MonoBehaviour
{
    public CinematicManager cm;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dragon"))
        {
            cm.RegisterTickleHit();
        }
    }
}
