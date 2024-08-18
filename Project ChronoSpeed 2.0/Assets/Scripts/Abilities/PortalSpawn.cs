using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PortalSpawn : MonoBehaviour
{
    [SerializeField] float timeTillPortalDissapears;
    [SerializeField] float maxHoldTime;
    [SerializeField] float cooldown;
    [SerializeField] GameObject portalPrefab;

    PortalVisual unlinkedPortal;
    bool useable = true;
    public void PortalDrop(CallbackContext callbackContext)
    {

        if (useable && callbackContext.started)
        {
            unlinkedPortal = Instantiate(portalPrefab, transform.position - transform.forward * 3, transform.rotation).GetComponent<PortalVisual>();
            StartCoroutine(ForceRelease());
        }
        else if (callbackContext.canceled)
        {
            OnRelease();
        }
    }


    void OnRelease()
    {
        if (unlinkedPortal == null) return;

        PortalVisual newPort = Instantiate(portalPrefab, transform.position - transform.forward * 3, transform.rotation).GetComponent<PortalVisual>();
        newPort.LinkedPortal = unlinkedPortal;
        unlinkedPortal.LinkedPortal = newPort;

        Destroy(unlinkedPortal.gameObject, timeTillPortalDissapears);
        Destroy(newPort.gameObject, timeTillPortalDissapears);
        unlinkedPortal = null;

        useable = false;
        StartCoroutine(Cooldown());
    }
    IEnumerator ForceRelease()
    {
        float timeIncrement = maxHoldTime * 0.1f;

        for (int counter = 0; counter <= 10; counter++)
        {
            if (unlinkedPortal == null) StopCoroutine("ForceRelease");
            yield return new WaitForSeconds(timeIncrement);
        }
        if (unlinkedPortal != null) OnRelease();
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        useable = true;
    }
}
