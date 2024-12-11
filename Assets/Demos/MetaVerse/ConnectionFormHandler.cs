using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionFormHandler : MonoBehaviour
{
    public void setUsername(string username)
    {
        State.Username = username;
    }

    public void StartGame() 
    {
        if (string.IsNullOrEmpty(State.Username))
        {
            // Afficher le message d'erreur au niveau du form login
            Debug.LogWarning("Username is empty. Please enter a valid username.");

            return;
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartAsServer()
    {
        if (!State.IsServer)
        {
            State.IsServer = true;
            State.ServerIsOnline = true;
        }
        
        setUsername("admin");

        StartGame();
    }

    public void SetServerIPAddress(string ipAddress)
    {
        State.ServerIP = ipAddress;
    }
}
