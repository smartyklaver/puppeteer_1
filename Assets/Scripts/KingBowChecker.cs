using UnityEngine;

public class KingBowChecker  : MonoBehaviour
{
    public SpineController1 spine;   
    public float requiredBend = 0.45f;
    public bool StartChecking;
    public TimelineRestarter1 restarter;
    public Camera cameraA; 
    public Camera cameraB; 
    public CurtainsOpenSceneOne curtain1;
    public CurtainsOpenSceneOne curtain2;


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
       // Debug.Log("checking bowing");
       // Debug.Log(torsoValue);
        if (torsoValue >= requiredBend)
        {
            
            Debug.Log(StartChecking);
            if (Input.GetKeyDown(KeyCode.Space) && StartChecking)
            {
            curtain1.ResetForTimeline();
            curtain2.ResetForTimeline();
            cameraA.targetDisplay = 1;
            cameraB.targetDisplay = 0; 
            restarter.RestartTimeline();
            }
        }
        else
        {

        }
    }
}
