using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class UDPService : MonoBehaviour
{
    UdpClient udp;
    IPEndPoint localEP;

    public delegate void UDPMessageReceive(string message, IPEndPoint sender);

    public event UDPMessageReceive OnMessageReceived;
    
    public async Task<bool> PingServer(int timeoutMilliseconds = 1000)
    {
        if (udp == null)
        {
            Debug.LogWarning("UDP client not initialized. Call InitClient() first.");
            return false;
        }

        IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(State.ServerIP), State.ServerPORT);
        
        SendUDPMessage("PING", serverEP);

        bool serverIsOnline = false;

        Task receiveTask = Task.Run(() =>
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                // Boucle pour écouter la réponse
                while (true)
                {
                    if (udp.Available > 0)
                    {
                        byte[] receivedBytes = udp.Receive(ref remoteEP);
                        string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedBytes);

                        if (receivedMessage == "PONG" && remoteEP.Equals(serverEP))
                        {
                            serverIsOnline = true;
                            break;
                        }
                    }
                }
            }
            catch
            {
                Debug.Log("Pas de client disponible");
            }
        });

        await Task.WhenAny(receiveTask, Task.Delay(timeoutMilliseconds));

        return serverIsOnline;
    }

    public bool Listen(int port) {
        if (udp != null) {
            Debug.LogWarning("Socket already initialized! Close it first.");
            return false;
        }

        try
        {
            // Local End-Point
            localEP = new IPEndPoint(IPAddress.Any, port);
            
            // Create the listener
            udp = new UdpClient();
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udp.ExclusiveAddressUse = false;
            udp.Client.Bind(localEP);

            Debug.Log("Server listening on port: " + port);

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Error creating UDP listener on port: " + port + ": " + ex.Message);
            CloseUDP();
            return false;
        }
    }

    public bool InitClient() {
        if (udp != null) {
            Debug.LogWarning("Socket already initialized! Close it first.");
            return false;
        }

        try
        {
            udp = new UdpClient();
            localEP = new IPEndPoint(IPAddress.Any, 0);
            udp.Client.Bind(localEP);
        } catch (System.Exception ex)
        {
            Debug.LogWarning("Error creating UDP client: " + ex.Message);
            CloseUDP();
            return false;
        }
        return true;
    }
    
    private void CloseUDP() {
        if (udp != null) {
            udp.Close();
            udp = null;
        }
    }

    void Update() {
        ReceiveUDP();
    }

    private void ReceiveUDP() {
        if (udp == null) { return; }

        while (udp.Available > 0)
		{
            IPEndPoint sourceEP = new IPEndPoint(IPAddress.Any, 0);
			byte[] data = udp.Receive(ref sourceEP);

			try
			{
                string receivedMessage = System.Text.Encoding.UTF8.GetString(data);

                if (receivedMessage == "PING")
                {
                    SendUDPMessage("PONG", sourceEP);

                    return;
                }
                
                ParseString(data, sourceEP);
			}
			catch (System.Exception ex)
			{
				Debug.LogWarning("Error receiving UDP message: " + ex.Message);
			}
		}
    }

    private void ParseString(byte[] bytes, IPEndPoint sender) {
        string message = System.Text.Encoding.UTF8.GetString(bytes);
        OnMessageReceived.Invoke(message, sender);
    }

    public void SendUDPMessage(string message, IPEndPoint destination) {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
        SendUDPBytes(bytes, destination);
    }

    private void SendUDPBytes(byte[] bytes, IPEndPoint destination) {
        if (udp == null) { 
            Debug.LogWarning("Trying to send a message on socket that is not yet open");
            return; 
        }

        try {
            udp.Send(bytes, bytes.Length, destination);
            
        } catch (SocketException e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
