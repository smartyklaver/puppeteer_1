using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEnder : MonoBehaviour
{
    public TimelineRestarter1 restarter;
    public CurtainsOpenSceneOne curtain1;
    public CurtainsOpenSceneOne curtain2;
    public CameraSwitcher cameraSwitcher;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }

    public void EndScene()
    {
        curtain1.ResetForTimeline();
        curtain2.ResetForTimeline();
        cameraSwitcher.SwitchCameraDisplays();
        restarter.RestartTimeline();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
