using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json.Linq; // ⇒ 事前に Newtonsoft JSON Package を追加

public class UdpListener : MonoBehaviour
{
    [SerializeField] int listenPort = 5005;
    [SerializeField] ParticleSystem snowEffect;

    UdpClient client;
    Thread recvThread;
    ConcurrentQueue<string> msgQueue = new ConcurrentQueue<string>();

    void Start()
    {
        client = new UdpClient(listenPort);
        recvThread = new Thread(ReceiveLoop) { IsBackground = true };
        recvThread.Start();

        if (snowEffect == null)
            Debug.LogError("ParticleSystem 未割当です");
    }

    void Update()
    {
        while (msgQueue.TryDequeue(out var json))
        {
            try
            {
                var jo = JObject.Parse(json);
                string gesture = jo["gesture"]?.ToString();
                HandleGesture(gesture);
            }
            catch (Exception e) { Debug.LogWarning(e); }
        }
    }

    void HandleGesture(string gesture)
    {
        switch (gesture)
        {
            case "open_hand":
                snowEffect.Play();           // 非同期に雪発生
                Debug.Log("Open Hand Detected");
                break;
            // 今後の拡張:
            // case "thumbs_up":  sparkles.Play(); break;
        }
    }

    void ReceiveLoop()
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, listenPort);
        try
        {
            while (true)
            {
                byte[] data = client.Receive(ref ep);
                string json = Encoding.UTF8.GetString(data);
                msgQueue.Enqueue(json);
            }
        }
        catch (SocketException) { /* 終了 */ }
    }

    void OnApplicationQuit()
    {
        client?.Close();
        recvThread?.Abort();
    }
}
