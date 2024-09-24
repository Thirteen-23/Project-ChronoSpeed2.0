using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Blink : MonoBehaviour
{
    [SerializeField] VolumeProfile chargeProfile;
    [SerializeField] VolumeProfile warpProfile;

    VFXContainer m_VFXContainer;

    Mirage currentMirage;
    Coroutine chargeCoroutine;
    Coroutine warpCoroutine;
    Coroutine dischargeCoroutine;

    private void Awake()
    {
        m_VFXContainer = GetComponent<VFXContainer>();
    }
    public void SpawnMirage()
    {
        m_VFXContainer.SetVFX(VFXManager.VFXTypes.electricBall, true);

        //Maybe do a little 0.5 second charge sound
        currentMirage.enabled = true;
        currentMirage.transform.position = Vector3.zero;
        chargeCoroutine = StartCoroutine(ChargeVisual());
        //tell the server
    }

    public void BreakMirage()
    {
        m_VFXContainer.SetVFX(VFXManager.VFXTypes.electricBall, true);

        StopCoroutine(chargeCoroutine);
        dischargeCoroutine = StartCoroutine(DischargeVisual());
        currentMirage.enabled = false;
        //tell the server
    }

    public void BlinkTo()
    {
        m_VFXContainer.SetVFX(VFXManager.VFXTypes.electricBall, true);

        StopCoroutine(chargeCoroutine);
        warpCoroutine = StartCoroutine(WarpVisual());

        //Do sound

        transform.position = currentMirage.transform.position;
        currentMirage.enabled = false;
    }


    public IEnumerator ChargeVisual()
    {
        Volume v = FindAnyObjectByType<MainCamera>().GetComponent<Volume>();
        v.profile = chargeProfile;
        v.weight = 0;
        while (v.weight < 1)
        {
            //Maybe increase sound for somethin
            v.weight += Time.deltaTime / 3f;
            yield return null;
        }
        v.weight = 1;
        chargeCoroutine = null;
    }
    public IEnumerator WarpVisual()
    {
        Volume v = FindAnyObjectByType<MainCamera>().GetComponent<Volume>();
        v.profile = warpProfile;
        v.weight = 1;

        while(v.weight > 0)
        {
            v.weight -= Time.deltaTime * 2f;
            yield return null;
        }
        v.weight = 0;
        warpCoroutine = null;
    }

    public IEnumerator DischargeVisual()
    {
        Volume v = FindAnyObjectByType<MainCamera>().GetComponent<Volume>();

        while (v.weight > 0)
        {
            v.weight -= Time.deltaTime * 2f;
            yield return null;
        }
        v.weight = 0;
        
    }

    public void SetMirage(Mirage curMir)
    {
        currentMirage = curMir;

        GetComponent<VFXContainer>().electricArc.transform.transform.parent = currentMirage.transform;
    }
}