using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ServerStatusIndicator : MonoBehaviour
{
    public Image panelImage;
    public TextMeshProUGUI statusText;

    private bool isServerUp = false;

    void Start()
    {
        UpdateStatus();
    }

    private void Update()
    {
        if (State.ServerIsOnline == isServerUp) return;
        
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        isServerUp = State.ServerIsOnline;

        statusText.color = Color.white;

        if (isServerUp)
        {
            statusText.text = "En ligne";
            panelImage.color = Color.green;

            return;
        }

        statusText.text = "Hors ligne";
        panelImage.color = Color.red;
    }
}