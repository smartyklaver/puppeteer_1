using UnityEngine;
using UnityEngine.Playables;
using FMODUnity;

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
    public UdpReceiver udp;
    private float TimeStartChecking;
    private float TimeDoneChecking;
    public StudioEventEmitter superman;
    private bool LightCanTurnOn = true;



    void Start(){
        StartChecking = false; 
    }

    public void CheckSuperman()
    {
        StartChecking = true;
        director.Pause();
        TimeStartChecking = Time.time;
        superman.Play();    
        
    }

    public void StopSupermanSound(){
        superman.Stop();
    }

    void Update()
    {
        float torsoValue = spine.GetCurrentTorsoValue();
        float shoulderleftValue = shoulders.GetCurrentLeftShoulderValue();
        float shoulderrightValue = shoulders.GetCurrentRightShoulderValue();
        //Debug.Log($"Be superman!!");
        if(StartChecking){
            Debug.Log($"het werkt");
        }


        //Debug.Log($"Updated: L={shoulderleftValue}, R={shoulderrightValue},");
        if ((shoulderleftValue >= minleftarm && shoulderleftValue <= maxleftarm) || (shoulderrightValue >= minrightarm && shoulderrightValue <= maxrightarm))
        {
            if(LightCanTurnOn)
            {
                arduino.SendLampStateForced(true);   
            }
            //Debug.Log($"Good Superman Pose");   
            if (arduino.WasButtonPressedThisFrame() && StartChecking  || ((Time.time - TimeStartChecking >= TimeDoneChecking) && udp.freezeInput==true && StartChecking)) 
            //if (Input.GetKeyDown(KeyCode.Space) && StartChecking || ((Time.time - TimeStartChecking >= TimeDoneChecking) && udp.freezeInput==true && StartChecking))
            {
            TimeDoneChecking = Time.time - TimeStartChecking;
            StartChecking = false; 
            LightCanTurnOn = false;
            director.Resume();
            var root = director.playableGraph.GetRootPlayable(0);
            root.SetSpeed(1);
            director.Resume();
            }
        }
        else
        {
            arduino.SendLampStateForced(false);
        }
    }
}
