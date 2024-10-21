using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOthersOnDisable : MonoBehaviour
{
    // GameObjects to enable when this GameObject is disabled
    [SerializeField] private GameObject[] gameObjects;

    private void OnDisable()
    {
        // Enable specified GameObjects
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}
