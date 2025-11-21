using UnityEngine;
using UnityEngine.Playables;

public class TimelineRestarter : MonoBehaviour
{
    // Timeline Director reference
    public PlayableDirector director;

    // References for immediate reset (must be linked in Inspector)
    public CurtainsOpenSceneOne curtain1;
    public CurtainsOpenSceneOne curtain2;
    public CameraMovetoValue cameraController;

    void Start()
    {
        // 🌟 NEW: This makes the Timeline start playing automatically once, 
        // as soon as the scene loads.
        if (director != null)
        {
            director.Play();
        }
    }

    // This function is called by the UnityEvent from InputRemover (Spacebar)
    public void RestartTimeline()
    {
        // The Spacebar press triggers both the immediate reset and the playback restart.
        PerformInstantReset();
        ExecuteTimelineControl();
    }

    // --- Helper Methods for Clarity ---

    private void PerformInstantReset()
    {
        // 1. **IMMEDIATE RESET** of all managed objects (snaps them to start position)
        if (curtain1 != null) curtain1.ResetForTimeline();
        if (curtain2 != null) curtain2.ResetForTimeline();
        if (cameraController != null) cameraController.ResetForTimeline();
    }

    private void ExecuteTimelineControl()
    {
        // 2. Timeline Control (Stops, rewinds, and plays)
        if (director != null)
        {
            director.Stop();
            director.time = 0;
            director.Play();
        }
    }
}