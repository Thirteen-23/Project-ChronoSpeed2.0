using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInterface : MonoBehaviour
{
    private BoxCollider BC;
    [SerializeField] private Animation open;
    [SerializeField] private Animation close;

    private void Awake()
    {
        BC = GetComponent<BoxCollider>();

    }
    public void TurnOff()
    {
        BC.enabled = false;
        if(close != null) close.Play();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }
    public void TurnOn() 
    {
        BC.enabled = true;
        if(open != null) open.Play();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
    }
}
