using UnityEngine;
using TMPro;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System;

public class ArduinoConnector : MonoBehaviour
{
    [Header("Connection")]
    public string arduinoIP = "192.168.1.102";
    public int arduinoPort = 12345;
    public int connectTimeoutMs = 5000;

    [Header("UI (optional)")]
    public TMP_Text connectionLabel;
    public TMP_Text lastReplyLabel;

    private TcpClient client;
    private NetworkStream stream;
    private Thread connectionThread;
    private volatile bool isConnected = false;
    private bool sentBaseOnConnect = false;

    void Update()
    {
        if (connectionLabel) connectionLabel.text = isConnected ? "Connected" : "Not Connected";
    }

    void OnApplicationQuit()
    {
        isConnected = false;
        try { stream?.Close(); } catch { }
        try { client?.Close(); } catch { }
        try { connectionThread?.Abort(); } catch { }
    }

    public void ConnectToArduino()
    {
        StartCoroutine(ConnectIfNeeded());
    }

    // ---------- Public helpers for other scripts ----------
    public void SendToU3(int value) { StartCoroutine(EnsureConnectedThenSendAddressed("U3", value)); }
    public void SendToU4(int value) { StartCoroutine(EnsureConnectedThenSendAddressed("U4", value)); }

    // Generic addressed send: "U3 750\n" / "U4 430\n" / "ALL 512\n"
    public void SendAddressed(string id, int value)
    {
        StartCoroutine(EnsureConnectedThenSendAddressed(id, value));
    }

    // ---------- Internals ----------
    private IEnumerator EnsureConnectedThenSendAddressed(string id, int value)
    {
        yield return StartCoroutine(ConnectIfNeeded());
        if (!isConnected || stream == null)
        {
            Debug.LogError("Not connected; cannot send.");
            yield break;
        }

        if (!SafeWrite($"{id} {value}\n"))
        {
            Debug.LogWarning("Write failed; reconnecting...");
            isConnected = false;
            yield return StartCoroutine(ConnectIfNeeded());
            if (!isConnected || stream == null || !SafeWrite($"{id} {value}\n"))
            {
                Debug.LogError("Failed to send after reconnect.");
                yield break;
            }
        }

        // Read a short one-line reply (ACK/OK/ERR) if available
        yield return StartCoroutine(ReadOneLineIntoLabel(1.0f));
    }

    private IEnumerator ConnectIfNeeded()
    {
        if (isConnected && stream != null) yield break;

        bool done = false;
        string connError = null;

        connectionThread = new Thread(() =>
        {
            try
            {
                var c = new TcpClient();
                IAsyncResult ar = c.BeginConnect(arduinoIP, arduinoPort, null, null);
                bool ok = ar.AsyncWaitHandle.WaitOne(connectTimeoutMs);
                if (!ok) { c.Close(); throw new SocketException((int)SocketError.TimedOut); }
                c.EndConnect(ar);
                c.NoDelay = true;

                client = c;
                stream = client.GetStream();
                isConnected = true;
            }
            catch (SocketException e)
            {
                connError = e.Message;
                isConnected = false;
            }
            finally { done = true; }
        });
        connectionThread.IsBackground = true;
        connectionThread.Start();

        float t0 = Time.time;
        float maxWait = (connectTimeoutMs / 1000f) + 0.25f;
        while (!done && Time.time - t0 < maxWait) yield return null;

        if (!isConnected || stream == null)
        {
            Debug.LogError("Connect failed: " + (connError ?? "unknown"));
            yield break;
        }

        Debug.Log("Connected to Arduino.");

        // On FIRST successful connection, push base positions:
        if (!sentBaseOnConnect)
        {
            sentBaseOnConnect = true;
            // U3 base = 750, U4 base = 430
            SafeWrite("U3 750\n");
            SafeWrite("U4 430\n");
            Debug.Log("Initial bases sent: U3 750, U4 430");
            // optional: try reading quick replies
            yield return StartCoroutine(ReadOneLineIntoLabel(0.5f));
            yield return StartCoroutine(ReadOneLineIntoLabel(0.5f));
        }
    }

    private bool SafeWrite(string s)
    {
        if (!isConnected || stream == null) return false;
        byte[] data = Encoding.UTF8.GetBytes(s);
        try { stream.Write(data, 0, data.Length); return true; }
        catch { return false; }
    }

    private IEnumerator ReadOneLineIntoLabel(float timeoutSec)
    {
        if (!isConnected || stream == null) yield break;

        var sb = new StringBuilder();
        float t0 = Time.time;

        while (Time.time - t0 < timeoutSec)
        {
            while (isConnected && stream != null && stream.DataAvailable)
            {
                int b;
                try { b = stream.ReadByte(); }
                catch { isConnected = false; yield break; }

                if (b < 0) { isConnected = false; yield break; }

                char c = (char)b;
                if (c == '\n' || c == '\r')
                {
                    if (sb.Length > 0)
                    {
                        string line = sb.ToString();
                        if (lastReplyLabel) lastReplyLabel.text = line;
                        Debug.Log("Arduino replied: " + line);
                        yield break;
                    }
                }
                else sb.Append(c);
            }
            yield return null;
        }
    }
}
