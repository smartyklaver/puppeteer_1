using UnityEngine;
using System.Collections.Generic;

public class ShoulderController1 : MonoBehaviour
{
    [Header("Shoulder Setup")]
    public Transform leftShoulder;
    public Transform rightShoulder;

    private float leftYAngle;
    private float rightYAngle;

    private UdpReceiver udp;
    private List<UdpReceiver.FrameData> allFrames;

    public bool Replay = false;
    private float replayStartTime;
    private float currentReplayTime;
    private int frameindex;

    void Start()
    {
        udp = FindObjectOfType<UdpReceiver>();
        allFrames = udp.GetRecordedData();
    }

    public void ReplayPuppetShoulders()
    {
        if (allFrames == null || allFrames.Count < 2)
        {
            Debug.LogWarning("[Shoulders] Not enough frames for replay");
            return;
        }

        float t0 = allFrames[0].timeStamp;
        for (int i = 0; i < allFrames.Count; i++)
            allFrames[i].timeStamp -= t0;

        replayStartTime = Time.time;
        Replay = true;
        frameindex = 0;
    }

    void Update()
    {
        if (!Replay)
        {
            // LIVE INPUT
            leftYAngle = udp.LatestData.leftShoulderValue + 90f;
            rightYAngle = udp.LatestData.rightShoulderValue + 90f;
        }
        else
        {
            // SAFETY: if list was cleared we stop replay
            if (allFrames == null || allFrames.Count < 2)
            {
                Debug.LogWarning("[Shoulders] Replay aborted: no frames");
                Replay = false;
                return;
            }

            currentReplayTime = Time.time - replayStartTime;

            // Prevent overflow:
            if (frameindex >= allFrames.Count)
                frameindex = allFrames.Count - 1;

            // Advance through frames
            while (frameindex < allFrames.Count - 1 &&
                   allFrames[frameindex + 1].timeStamp <= currentReplayTime)
            {
                frameindex++;
            }

            // SAFETY: avoid out-of-range
            frameindex = Mathf.Clamp(frameindex, 0, allFrames.Count - 1);

            // Apply frame
            leftYAngle = allFrames[frameindex].leftShoulder + 90f;
            rightYAngle = allFrames[frameindex].rightShoulder + 90f;

            // End of replay
            float endTime = allFrames[allFrames.Count - 1].timeStamp;
            if (currentReplayTime >= endTime - 0.0001f)
            {
                Replay = false;
                Debug.Log("[Shoulders] Replay finished");
            }
        }

        // Apply rotations
        if (leftShoulder != null)
        {
            Vector3 e = leftShoulder.localEulerAngles;
            e.z = leftYAngle;
            leftShoulder.localEulerAngles = e;
        }

        if (rightShoulder != null)
        {
            Vector3 e = rightShoulder.localEulerAngles;
            e.z = rightYAngle;
            rightShoulder.localEulerAngles = e;
        }
    }
}
