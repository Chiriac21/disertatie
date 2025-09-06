using UnityEngine;
using TMPro;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ArduinoConnector : MonoBehaviour
{
    [Header("Connection Settings")]
    public string arduinoIP = "192.168.43.50"; // Arduino static IP
    public int arduinoPort = 12345;           // Same as Arduino WiFiServer

    [Header("UI")]
    public TMP_Text connectionLabel;

    private TcpClient client;
    private NetworkStream stream;
    private Thread connectionThread;
    private volatile bool isConnected = false;

    // Called when pressing the Connect button
    public void ConnectToArduino()
    {
        if (isConnected)
        {
            Debug.Log("Already connected to Arduino.");
            return;
        }

        connectionThread = new Thread(StartConnection);
        connectionThread.IsBackground = true;
        connectionThread.Start();
    }

    private void StartConnection()
    {
        try
        {
            client = new TcpClient();
            client.Connect(arduinoIP, arduinoPort);
            stream = client.GetStream();
            isConnected = true;

            Debug.Log("Connected to Arduino!");
        }
        catch (SocketException e)
        {
            Debug.LogError("Connection failed: " + e.Message);
            isConnected = false;
        }
    }

    public void SendMessageToArduino(string message)
    {
        if (isConnected && stream != null)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            try
            {
                stream.Write(data, 0, data.Length);
                Debug.Log("Sent to Arduino: " + message);
            }
            catch
            {
                Debug.LogWarning("Lost connection to Arduino.");
                isConnected = false;
            }
        }
        else
        {
            Debug.LogWarning("Not connected, cannot send message.");
        }
    }

    void Update()
    {
        if (connectionLabel != null)
        {
            connectionLabel.text = isConnected ? "Connected" : "Not Connected";
        }
    }

    void OnApplicationQuit()
    {
        isConnected = false;
        stream?.Close();
        client?.Close();
        connectionThread?.Abort();
    }
}