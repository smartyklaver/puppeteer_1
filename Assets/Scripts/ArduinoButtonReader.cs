using System.IO.Ports;
using UnityEngine;

public class ArduinoButtonReader : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "COM5";  // On Windows: COM3, COM4, etc.
                                     // On Mac/Linux: "/dev/tty.usbmodemXXXX" or "/dev/ttyACM0"
    public int baudRate = 9600;

    private SerialPort serialPort;
    private bool buttonPressedThisFrame = false;

    void Start()
    {
        //TryOpenSerialPort();
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 20;  // small timeout so we don't block forever
            serialPort.Open();
            Debug.Log("Serial port opened: " + portName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
    }

    void Update()
    {
        // Reset flag at start of frame
        buttonPressedThisFrame = false;

        if (serialPort == null || !serialPort.IsOpen)
            return;

        try
        {
            // Read all available lines for this frame
            while (serialPort.BytesToRead > 0)
            {
                string line = serialPort.ReadLine().Trim();
                // Debug.Log("Arduino says: " + line);

                if (line == "success")
                {
                    buttonPressedThisFrame = true;

                    // React immediately here if you want:
                    OnArduinoButtonPressed();
                }
            }
        }
        catch (System.TimeoutException)
        {
            // Normal: no data this frame
        }
    }

    void TryOpenSerialPort()
{
    try
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();

        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 20;
        serialPort.Open();

        Debug.Log("Serial port opened: " + portName);
    }
    catch (System.Exception e)
    {
        Debug.LogError("Failed to open serial port: " + e.Message);
    }
}

    private void OnArduinoButtonPressed()
    {
        // Put whatever you want to happen in the game here
        Debug.Log(">>> Arduino button pressed!");
        // Example: jump, fire, open door, etc.
        // player.Jump();
    }

    public bool WasButtonPressedThisFrame()
    {
        return buttonPressedThisFrame;
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            serialPort.Dispose();
        }
    }
    public void SendLampStateForced(bool on)
    {
        SendToArduino(on ? "L" : "l");
    }
    public void SendToArduino(string message)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                serialPort.Write(message);
            }
            catch { }
        }

    }

    public void ClosePort()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }

}


