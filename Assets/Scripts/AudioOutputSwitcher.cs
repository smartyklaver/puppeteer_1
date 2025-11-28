using UnityEngine;
using FMODUnity;
using System.Collections;

public class AudioOutputSwitcher : MonoBehaviour
{
    void Start()
    {
        // Wait one frame to ensure FMOD is initialized
        StartCoroutine(InitAudio());
    }

    private IEnumerator InitAudio()
    {
        yield return null; // wait one frame
        SwitchToPrimary();
    }

    [Header("Device Names")]
    public string primaryDevice = "Speakers (Realtek(R) Audio)";
    public string secondaryDevice = "Headset (soundcore Space One)";

    public void SwitchToPrimary() => SwitchOutputDevice(primaryDevice);
    public void SwitchToSecondary() => SwitchOutputDevice(secondaryDevice);

    private void SwitchOutputDevice(string deviceName)
    {
        FMOD.System system = RuntimeManager.CoreSystem;
        system.getNumDrivers(out int numDrivers);

        int driverId = -1;

        for (int i = 0; i < numDrivers; i++)
        {
            system.getDriverInfo(i, out string name, 256, out _, out _, out _, out _);
            if (name.Contains(deviceName)) // flexible match
            {
                driverId = i;
                break;
            }
        }

        if (driverId == -1)
        {
            Debug.LogWarning("FMOD: Device not found: " + deviceName);
            return;
        }

        system.setDriver(driverId);
        Debug.Log("FMOD: Switched to output device: " + deviceName);
    }
}
