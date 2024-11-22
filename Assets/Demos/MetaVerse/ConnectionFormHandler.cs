using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionFormHandler : MonoBehaviour
{
    private string _username;

    public void setUsername(string username)
    {
        _username = username;
    }

    public void StartGame() 
    {
        if (string.IsNullOrEmpty(_username))
        {
            // Afficher le message d'erreur au niveau du form login
            Debug.LogWarning("Username is empty. Please enter a valid username.");

            return;
        }
        
        if (State.IsServer) State.IsServer = false;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartAsServer()
    {
        if (!State.IsServer) State.IsServer = true;
        
        setUsername("server_host");

        StartGame();
    }
}
