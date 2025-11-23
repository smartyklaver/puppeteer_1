using UnityEngine;

public class KingBowChecker  : MonoBehaviour
{
    public SpineController1 spine;   
    public float requiredBend = 0.45f;
    public bool StartChecking;
    public TimelineRestarter restarter;


    void Start(){
        StartChecking = false; 
    }

    public void CheckBow()
    {
        StartChecking = true;
        
    }

    void Update()
    {
        float torsoValue = spine.GetCurrentTorsoValue();
        Debug.Log("checking bowing");
        Debug.Log(torsoValue);
        if (torsoValue >= requiredBend)
        {
 
            if (Input.GetKeyDown(KeyCode.Space) && StartChecking)
            {
            restarter.RestartTimeline();
            }
        }
        else
        {

        }
    }
}
