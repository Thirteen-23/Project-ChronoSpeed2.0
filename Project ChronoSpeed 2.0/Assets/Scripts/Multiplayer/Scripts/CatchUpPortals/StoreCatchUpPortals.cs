using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreCatchUpPortals : MonoBehaviour
{
    private PortalInterface[] portals;
    private void Awake()
    {
        portals = GetComponentsInChildren<PortalInterface>();
    }
    public void OpenPortalPair()
    {
        for(int i = 0; i < portals.Length; i++)
        {
            portals[i].TurnOn();
        }
    }
    public void ClosePortalPair()
    {
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].TurnOff();
        }
    }
}
