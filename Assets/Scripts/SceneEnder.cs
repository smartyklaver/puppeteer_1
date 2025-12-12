using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEnder : MonoBehaviour
{
    public TimelineRestarter1 restarter;
    public CurtainsOpenSceneOne curtain1;
    public CurtainsOpenSceneOne curtain2;
    public CameraSwitcher cameraSwitcher;
    public AudioOutputSwitcher audioSwitcher;
    public SupermanCheck supermanCheck;
    public ArduinoButtonReader arduino;

    private bool replay;
    public string nextSceneName;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        replay = false;
    }

    public void EndScene()
    {
        if(replay){
            SceneManager.LoadScene(nextSceneName);
        }
        replay = true;
        arduino.ClosePort();
        curtain1.ResetForTimeline();
        curtain2.ResetForTimeline();
        supermanCheck.StopSupermanSound();
        cameraSwitcher.SwitchCameraDisplays();
        restarter.RestartTimeline();
        audioSwitcher.SwitchToSecondary();

    }

    // Update is called once per frame
    void Update()
    {   

    }
}
