using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXContainer : MonoBehaviour
{
    [SerializeField] GameObject elecArcPrefab;

    [HideInInspector] public VisualEffect electricArc;

    public void SetVFX(VFXManager.VFXTypes type, bool setTo)
    {
        switch(type)
        {
            case VFXManager.VFXTypes.electricBall:
                electricArc.enabled = setTo;
                break;
        }
    }
    private void Awake()
    {
        electricArc = Instantiate(elecArcPrefab, Vector3.zero, Quaternion.identity).GetComponent<VisualEffect>();
    }
}
