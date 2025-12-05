using UnityEngine;

public class Scene2Initializer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Scene 2 initializer running");
        FindObjectOfType<UdpReceiver>()?.BeginNewRecording();
    }
}