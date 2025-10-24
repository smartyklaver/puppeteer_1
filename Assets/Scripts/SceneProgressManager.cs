using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneProgressManager : MonoBehaviour
{
    [Header("Pose Matching Settings")]
    public PoseMatcher poseMatcher;   // Sleep hier je pose-detector in
    public float delayAfterPose = 20f;

    [Header("Next Scene Settings")]
    public string nextSceneName = "Scene2"; // De exacte naam van je tweede scene (zoals in de Project-tab)

    private bool transitionStarted = false;

    void Update()
    {
        if (poseMatcher == null) return;

        // Wanneer de speler de pose behaalt
        if (poseMatcher.IsPoseMatched && !transitionStarted)
        {
            transitionStarted = true;
            Debug.Log($"✅ Pose completed! Loading {nextSceneName} in {delayAfterPose} seconds...");
            Invoke(nameof(LoadNextScene), delayAfterPose);
        }
    }

    void LoadNextScene()
    {
        Debug.Log($"🎬 Loading next scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}
