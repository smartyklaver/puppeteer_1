using UnityEngine;
using UnityEngine.Playables;

public class SupermanCheck  : MonoBehaviour
{
    public SpineController1 spine;
    public ShoulderController1 shoulders;     
    public float maxleftarm= 340f;
    public float minleftarm = 200f;
    public float maxrightarm = 350f;
    public float minrightarm = 150f;
    public bool StartChecking;
    // public TimelineRestarter1 restarter;
    // public Camera cameraA; 
    // public Camera cameraB; 
    // public CurtainsOpenSceneOne curtain1;
    // public CurtainsOpenSceneOne curtain2;
    public ArduinoButtonReader arduino;
    public PlayableDirector director;



    void Start(){
        StartChecking = false; 
    }

    public void CheckSuperman()
    {
        StartChecking = true;
        director.Pause();
        
    }

    void Update()
    {
        float torsoValue = spine.GetCurrentTorsoValue();
        float shoulderleftValue = shoulders.GetCurrentLeftShoulderValue();
        float shoulderrightValue = shoulders.GetCurrentRightShoulderValue();
        Debug.Log($"Be superman!!");

       // Debug.Log($"goeie linkerarm max, L={shoulderrightValue}");

       // Debug.Log($"Updated: L={shoulderleftValue}, R={shoulderrightValue},");
        if ((shoulderleftValue >= minleftarm && shoulderleftValue <= maxleftarm) || (shoulderrightValue >= minrightarm && shoulderrightValue <= maxrightarm))
        {
            Debug.Log($"Good Superman Pose");    
            if (Input.GetKeyDown(KeyCode.Space) && StartChecking)
            {
            // curtain1.ResetForTimeline();
            // curtain2.ResetForTimeline();
            // cameraA.targetDisplay = 1;
            // cameraB.targetDisplay = 0; 
            // restarter.RestartTimeline();
            director.Resume();
            }
        }
        else
        {

        }
    }
}
