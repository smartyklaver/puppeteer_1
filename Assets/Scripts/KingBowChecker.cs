using UnityEngine;
using UnityEngine.Playables;

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
    public ArduinoButtonReader arduino;
    public PlayableDirector director;
    public UdpReceiver udp;
    private float TimeStartChecking;
    private float TimeDoneChecking;



    void Start(){
        StartChecking = false; 
    }

    public void CheckBow()
    {
        StartChecking = true;
        director.Pause();
        TimeStartChecking = Time.time;
        
    }

    void Update()
    {
        float torsoValue = spine.GetCurrentTorsoValue();
       // Debug.Log("checking bowing");
        Debug.Log($"Timedonechecking={TimeDoneChecking}, Timestartchecking={TimeStartChecking},Timenow={Time.time}");
        if (torsoValue >= requiredBend)
        {
            //Debug.Log(StartChecking);
           // if (arduino.WasButtonPressedThisFrame() && StartChecking)
            if ((Input.GetKeyDown(KeyCode.Space) && StartChecking) || ((Time.time - TimeStartChecking >= TimeDoneChecking) && udp.freezeInput==true && StartChecking))
            {
            TimeDoneChecking = Time.time - TimeStartChecking;
            StartChecking = false;
            director.Resume();
            var root = director.playableGraph.GetRootPlayable(0);
            root.SetSpeed(1);
            director.Resume();
            }
        }
        else
        {

        }
    }
}
