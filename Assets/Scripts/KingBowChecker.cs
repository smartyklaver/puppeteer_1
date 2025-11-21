using UnityEngine;

public class KingBowChecker  : MonoBehaviour
{
    public SpineController1 spine;   
    public float requiredBend = 0.45f;
    public bool canReset = false;
    public TimelineRestarter restarter;

    public void CheckBow()
    {
        float torsoValue = spine.GetCurrentTorsoValue();
        Debug.Log("checking");
        Debug.Log(torsoValue);
        if (torsoValue >= requiredBend)
        {
            Debug.Log("Koning: goed gebogen!");
            canReset = true; 
            if (Input.GetKeyDown(KeyCode.Space))
            {
            restarter.RestartTimeline();
            }
        }
        else
        {
            Debug.Log("Koning: buig dieper!");
            canReset = false;
        }
    }
}
