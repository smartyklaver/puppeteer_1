using UnityEngine;
using FMODUnity;
using System.Collections;

public class AudioOutputSwitcher : MonoBehaviour
{
    IEnumerator Start()
    {
        // 1. Wait until FMOD is initialized
        while (!RuntimeManager.IsInitialized)
            yield return null;

        // 2. Add a small safety delay after FMOD is ready
        yield return new WaitForSeconds(0.1f);

        // 3. Log all available drivers before trying to switch
        LogAvailableDrivers();

        // 4. Attempt to switch
        SwitchToPrimary();
    }

    [Header("Device Names")]
    // NOTE: These strings should be as short and reliable as possible (e.g., "Realtek" or "Headset")
    public string primaryDevice = "Headset (Realtek(R) Audio)";
    public string secondaryDevice = "Nothing Ear (a)";

    public void SwitchToPrimary() => SwitchOutputDevice(primaryDevice);
    public void SwitchToSecondary() => SwitchOutputDevice(secondaryDevice);

    // --- New Debug Function ---
    private void LogAvailableDrivers()
    {
        var system = RuntimeManager.CoreSystem;
        system.getNumDrivers(out int numDrivers);

        Debug.Log("--- FMOD Available Drivers ---");

        // Log FMOD's currently active driver
        system.getDriver(out int currentDriverId);
        system.getDriverInfo(currentDriverId, out string currentName, 256, out _, out _, out _, out _);
        Debug.Log($"Active FMOD Driver: {currentDriverId} - {currentName}");


        for (int i = 0; i < numDrivers; i++)
        {
            // FMOD.RESULT result is important for debugging getDriverInfo errors, though usually not needed here
            FMOD.RESULT result = system.getDriverInfo(i, out string name, 256, out _, out _, out _, out _);

            if (result == FMOD.RESULT.OK)
            {
                Debug.Log($"Driver ID {i}: {name}");
            }
            else
            {
                Debug.LogError($"Failed to get info for Driver ID {i}. Result: {result}");
            }
        }
        Debug.Log("----------------------------");
    }

    public void SwitchOutputDevice(string deviceName)
    {
        var system = RuntimeManager.CoreSystem;
        system.getNumDrivers(out int numDrivers);

        int driverId = -1;

        for (int i = 0; i < numDrivers; i++)
        {
            system.getDriverInfo(i, out string name, 256, out _, out _, out _, out _);

            // Use the ToLowerInvariant method for case-insensitive matching, 
            // and use Contains() to match partial names safely.
            if (name.ToLowerInvariant().Contains(deviceName.ToLowerInvariant()))
            {
                driverId = i;
                Debug.Log($"FMOD: Found device '{name}' at Driver ID {driverId}");
                break;
            }
        }

        if (driverId == -1)
        {
            Debug.LogError($"FMOD: Device not found: '{deviceName}'. Cannot switch output.");
            return;
        }

        // --- Critical Change: Check FMOD Result for setDriver ---
        FMOD.RESULT result = system.setDriver(driverId);

        if (result == FMOD.RESULT.OK)
        {
            Debug.Log($"FMOD: **SUCCESS** Switched to output device: '{deviceName}' (ID: {driverId})");
        }
        else
        {
            // If this fails, this is the error you need to solve!
            Debug.LogError($"FMOD: **FAILED** to set driver to '{deviceName}' (ID: {driverId}). FMOD Error: {result}");
        }
    }
}