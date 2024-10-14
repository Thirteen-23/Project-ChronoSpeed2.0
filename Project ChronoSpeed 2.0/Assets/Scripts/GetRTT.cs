using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class GetRTT : MonoBehaviour

{
    public TextMeshProUGUI text;   
    void Update()
    {
        text.text = "Ping: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.ServerClientId).ToString() + " ms";  
    }
}
