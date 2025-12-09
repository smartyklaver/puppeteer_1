using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera cameraA;
    public Camera cameraB;

    void Start()
    {
        if (cameraA != null) cameraA.targetDisplay = 0;
        if (cameraB != null) cameraB.targetDisplay = 1;

    }

    public void SwitchCameraDisplays()
    {
        
        if (cameraA != null) cameraA.targetDisplay = 1;
        if (cameraB != null) cameraB.targetDisplay = 0;

    }
}