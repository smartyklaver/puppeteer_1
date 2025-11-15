using UnityEngine;
using System.Collections;


public class TickleDetector: MonoBehaviour
{
    public CinematicManager manager;
    int tickleFrames = 0;

    void OnTriggerStay(Collider col)
    {
        if (!col.CompareTag("Dragon")) return;

        tickleFrames++;

        if (tickleFrames > 20) // ± 0.3 sec
            manager.RegisterTickle();
    }
}
