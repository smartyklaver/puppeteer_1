using UnityEngine;

public class SceneEnder : MonoBehaviour
{
    public TimelineRestarter1 restarter;
    public Camera cameraA; 
    public Camera cameraB; 
    public CurtainsOpenSceneOne curtain1;
    public CurtainsOpenSceneOne curtain2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }

    public void EndScene()
    {
        curtain1.ResetForTimeline();
        curtain2.ResetForTimeline();
        cameraA.targetDisplay = 1;
        cameraB.targetDisplay = 0; 
        restarter.RestartTimeline();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
