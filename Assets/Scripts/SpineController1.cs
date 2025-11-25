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

    public void ReplayPuppetSpine()
    {
        replayStartTime = Time.time;
        Replay = true;
        frameindex = 0;

        replayDuration = allFrames[allFrames.Count - 1].timeStamp;
    }

    public float GetCurrentTorsoValue()
    {
    return currentAngle;
    }


    void Update()
    {
        if (spine == null) return;

        if (!Replay)
        {
            torsoRaw = udp.LatestData.torsoBend;
        }
        else
        {

            currentReplayTime = Time.time - replayStartTime;
            float t = currentReplayTime / replayDuration;

            frameindex = Mathf.Clamp(
                Mathf.FloorToInt(t * (allFrames.Count - 1)),
                0,
                allFrames.Count - 1
            );

            torsoRaw = allFrames[frameindex].torsoBend;


            if (frameindex >= allFrames.Count - 1)
            {
                Replay = false;
                CloseCurtains?.Invoke();
            }
        }
        
        currentAngle = Mathf.Lerp(-40f, 120f, torsoRaw);

        spine.localRotation = Quaternion.Euler(
            fixedXAngle,
            fixedYAngle,
            initialZAngle + currentAngle
        );
    }
}
