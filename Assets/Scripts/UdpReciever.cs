using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class UdpReceiver : MonoBehaviour
{
    [System.Serializable]
    public class UdpPayload
    {
        public float leftShoulderValue;
        public float rightShoulderValue;
        public float torsoBend;
    }

    [Header("UDP Settings")]
    public int port = 56000;

    public UdpPayload LatestData { get; private set; } = new UdpPayload();

    private UdpClient client;
    private CancellationTokenSource cts;
    private bool running;
    private string _lastMessage;

    async void Start()
    {
        LatestData.leftShoulderValue = 0.5f;
        LatestData.rightShoulderValue = 0.5f;
        LatestData.torsoBend = 0.5f;

        try
        {
            client = new UdpClient(port);
            client.Client.ReceiveTimeout = 1000;
            running = true;
            cts = new CancellationTokenSource();
            Debug.Log($"[UDP] Listening on port {port}");
            _ = ReceiveLoop(cts.Token);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UDP] Failed to start: {ex.Message}");
        }
    }

    private async Task ReceiveLoop(CancellationToken token)
    {
        while (running && !token.IsCancellationRequested)
        {
            try
            {
                UdpReceiveResult result = await client.ReceiveAsync();
                string text = Encoding.UTF8.GetString(result.Buffer);
                _lastMessage = text;

                UdpPayload data = JsonUtility.FromJson<UdpPayload>(text);
                if (data != null)
                {
                    LatestData = data;
                    Debug.Log($"[UDP] Updated: L={data.leftShoulderValue}, R={data.rightShoulderValue}, torso={data.torsoBend}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[UDP] Receive failed: {ex.Message}");
            }
        }
    }

    void OnApplicationQuit() => Shutdown();
    void OnDestroy() => Shutdown();
    void OnDisable() => Shutdown();

    void Shutdown()
    {
        running = false;
        try { cts?.Cancel(); } catch { }
        try { client?.Close(); } catch { }
        Debug.Log("[UDP] Shutdown");
    }
}