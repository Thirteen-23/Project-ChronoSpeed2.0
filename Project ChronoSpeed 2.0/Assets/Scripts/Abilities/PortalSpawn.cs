using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PortalSpawn : MonoBehaviour
{
    [SerializeField] float maxHoldTime;
    [SerializeField] float cooldown;
    [SerializeField] float portalLast = 60f;

    Coroutine curForceReleaseCor;

    Vector3 startPos;
    Quaternion startRot;
    public void PortalDrop(CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            startPos = transform.position + transform.forward * -4;
            startRot = transform.rotation;
            MultiplayerGameManager.Singleton.SpawnPortalStandInRpc(startPos, startRot);
            curForceReleaseCor = StartCoroutine(ForceRelease());
        }
        else if (callbackContext.canceled)
        {
            OnRelease();
        }
    }

    void OnRelease()
    {
        if(startPos == Vector3.zero && startRot == Quaternion.identity) { return; }
        MultiplayerGameManager.Singleton.SpawnPortalRpc(startPos, startRot, transform.position + transform.forward * -4, transform.rotation, portalLast);

        startPos = Vector3.zero;
        startRot = Quaternion.identity;

        if (curForceReleaseCor != null)
        {
            StopCoroutine(ForceRelease());
            curForceReleaseCor = null;
        }
    }
    IEnumerator ForceRelease()
    {
        yield return new WaitForSeconds(maxHoldTime);
        OnRelease();
    }

    public void PortalLeft(CallbackContext callbackContext)
    {
        if (!callbackContext.performed)
            return;
        Vector3 startV, endV;
        Quaternion startQ, endQ;
        startV = transform.position + transform.forward * 4;
        endV = transform.position - transform.right * 4;
        startQ = transform.rotation;
        endQ = Quaternion.Euler(transform.eulerAngles - new Vector3(0, 90, 0));

        MultiplayerGameManager.Singleton.SpawnPortalRpc(startV, startQ, endV, endQ, 0.5f);
    }

    public void PortalRight(CallbackContext callbackContext)
    {
        if (!callbackContext.performed)
            return;
        Vector3 startV, endV;
        Quaternion startQ, endQ;
        startV = transform.position + transform.forward * 4;
        endV = transform.position + transform.right * 4;
        startQ = transform.rotation;
        endQ = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 90, 0));

        MultiplayerGameManager.Singleton.SpawnPortalRpc(startV, startQ, endV, endQ, 0.5f);
    }
}
