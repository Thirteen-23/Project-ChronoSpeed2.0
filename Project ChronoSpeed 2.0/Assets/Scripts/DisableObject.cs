using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
    public float delayTime = 26.5f; // Public variable to set the delay time
    private float timer = 0f;
    private bool isEnabled = false;

    private void OnEnable()
    {
        isEnabled = true;
        timer = 0f;
    }

    private void Update()
    {
        if (isEnabled)
        {
            timer += Time.deltaTime;
            if (timer >= delayTime)
            {
                isEnabled = false;
                gameObject.SetActive(false);
            }
        }
    }
}