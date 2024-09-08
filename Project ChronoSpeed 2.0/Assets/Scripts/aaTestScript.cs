using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class aaTestScript : MonoBehaviour
{
    
    public void LoadScenesdhasd()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("RaceTrack", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
