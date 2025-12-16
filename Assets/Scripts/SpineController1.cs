using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class SpineController1 : MonoBehaviour
{
    [Header("Spine Setup")]
    public Transform spine;
    public float initialStartAngle = 0f;

    private float currentAngle;
    private float initialZAngle;
    private float fixedXAngle;
    private float fixedYAngle;

    private UdpReceiver udp;

    private List<UdpReceiver.FrameData> allFrames;  

    public UnityEvent CloseCurtains;

    public bool Replay = false;
    private float torsoRaw =0;
    private float replayStartTime;
    private float currentReplayTime;
    private int frameindex;
    private float replayDuration;

    void Start()
    {
        udp = FindObjectOfType<UdpReceiver>();
        
        allFrames = udp.GetRecordedData();

        if (spine != null)
        {
            Vector3 startRotation = spine.localEulerAngles;
            fixedXAngle = startRotation.x;
            fixedYAngle = startRotation.y;
            initialZAngle = startRotation.z;

            currentAngle = initialStartAngle;
        }
    }

    //  void Awake()
    //  {
    //     allFrames = new List<UdpReceiver.FrameData>();
    //     Debug.Log("awake");
    //  }

public void ReplayPuppetSpine()
{
    if (allFrames.Count < 2) return;

    udp.freezeInput = true;

    float t0 = allFrames[0].timeStamp;
    for (int i = 0; i < allFrames.Count; i++)
        allFrames[i].timeStamp -= t0;

    replayStartTime = Time.time;
    Replay = true;
    frameindex = 0;

}


    public float GetCurrentTorsoValue()
    {
        return currentAngle;
    }


    void Update()
    {
        if (spine == null) return;

if(!Replay) { torsoRaw = udp.LatestData.torsoBend; } else { currentReplayTime = Time.time - replayStartTime; while(frameindex < allFrames.Count - 1 && allFrames[frameindex + 1].timeStamp<= currentReplayTime) { frameindex++; }
            torsoRaw = allFrames[frameindex].torsoBend;

      
        float endTime = allFrames[allFrames.Count - 1].timeStamp;

        if (currentReplayTime >= endTime - 0.0001f)
        {
            // force last frame
            frameindex = allFrames.Count - 1;
            torsoRaw = allFrames[frameindex].torsoBend;

            Replay = false;

            udp.freezeInput = false;
            udp.BeginNewRecording();

            CloseCurtains?.Invoke();

            Debug.Log("Replay ended safely");
        }
        }
        
        currentAngle = Mathf.Lerp(-25f, 80f, torsoRaw);

        spine.localRotation = Quaternion.Euler(
            fixedXAngle,
            fixedYAngle,
            initialZAngle + currentAngle
        );
    }
}
