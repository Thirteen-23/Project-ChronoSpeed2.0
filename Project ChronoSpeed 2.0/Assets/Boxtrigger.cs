using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boxtrigger : MonoBehaviour
{
    [SerializeField] Image[] arrowSignForDirection;
    [SerializeField] int arrowIndex; 

    private void FixedUpdate()
    {
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody.tag == "Player")
        {
            arrowSignForDirection[arrowIndex].color = new Color(255, 255, 255, 1); 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody.tag == "Player")
        {
            arrowSignForDirection[arrowIndex].color = new Color(255, 255, 255, 0);
        }
    }
}
