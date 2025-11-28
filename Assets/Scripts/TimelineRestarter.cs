using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using UnityEngine.SceneManagement;   


public class TimelineRestarter : MonoBehaviour
{
    
    public PlayableDirector director;
    public CurtainsOpenSceneOne curtain1;
    public CurtainsOpenSceneOne curtain2;
    public CameraMovetoValue cameraController;
    public SpineController1 spinecontroller;
    public ShoulderController1 shouldercontroller;
    public LetterRemove LetterRemove;
    

    private float recordingStartTime = 0f;

    void Start()
    {
        director.Play();

        recordingStartTime = Time.time;

        LetterRemove.ShowLetter();
    }

    public void RestartTimeline()
    {
        LetterRemove.ShowLetter();

        float timeOfPress = LetterRemove.timeOfSpacebarPress;

        // Berekende duur: Tijd tussen start opname en spatiebalk-druk
        float recordedDuration = timeOfPress - recordingStartTime;

        // Start de replay van de animatie controllers
        spinecontroller.ReplayPuppetSpine();
        shouldercontroller.ReplayPuppetShoulders();

        PerformInstantReset();
        ExecuteTimelineControl();

        if (recordedDuration > 0)
        {
            StopAllCoroutines();
            StartCoroutine(HideLetterAfterDelay(recordedDuration));
        }
    }

    private void PerformInstantReset()
    {
        curtain1.ResetForTimeline();
        curtain2.ResetForTimeline();
        cameraController.ResetForTimeline();
    }

    private void ExecuteTimelineControl()
    {
        director.Stop();
        director.time = 0;
        director.Play();
    }

    private IEnumerator HideLetterAfterDelay(float delay)
    {
        // Wacht de duur van de opgenomen actie af
        yield return new WaitForSeconds(delay);

        // Verberg de letter
        float audioDelay = LetterRemove.HideLetterAndPlayAudio();

        yield return new WaitForSeconds(audioDelay);

        // Sluit de gordijnen
        CloseCurtainsAndZoomOut();
    }

    private void CloseCurtainsAndZoomOut()
    {
        curtain1.CloseCurtains();
        curtain2.CloseCurtains();
        cameraController.ReturnToStart();
    }

    public void LoadNextScene()
{
    // Load the next scene in the build index
    int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
    SceneManager.LoadScene(nextScene);
}

}