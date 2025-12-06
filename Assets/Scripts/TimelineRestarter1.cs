using UnityEngine;
using UnityEngine.Playables;

public class TimelineRestarter1 : MonoBehaviour
{
    public PlayableDirector director;

    public CurtainsOpenSceneOne curtain1;
    public CurtainsOpenSceneOne curtain2;
    public CameraMovetoValue cameraController;
    public SpineController1 spinecontroller;
    public ShoulderController1 shouldercontroller;

    void Start()
    {
        

        if (director != null)
        {
            director.Play();
        }
    }


    public void RestartTimeline()
    {
        spinecontroller.ReplayPuppetSpine();
        shouldercontroller.ReplayPuppetShoulders();
        PerformInstantReset();
        ExecuteTimelineControl();
    }


    private void PerformInstantReset()
    {
        if (curtain1 != null) curtain1.ResetForTimeline();
        if (curtain2 != null) curtain2.ResetForTimeline();
        if (cameraController != null) cameraController.ResetForTimeline();
    }

    private void ExecuteTimelineControl()
    {
        director.Stop();
        director.RebuildGraph();
        var root = director.playableGraph.GetRootPlayable(0);
        root.SetSpeed(1);
        director.time = 0;
        director.Play();
    }
}