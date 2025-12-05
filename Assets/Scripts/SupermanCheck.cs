using UnityEngine;
using UnityEngine.Playables;

public class SupermanCheck  : MonoBehaviour
{
    public SpineController1 spine;
    public ShoulderController1 shoulders;     
    public float requiredBend = 0.45f;
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

        if (torsoValue >= requiredBend)
        {
            
            //Debug.Log(StartChecking);
           // if (arduino.WasButtonPressedThisFrame() && StartChecking)
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
