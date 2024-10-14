using UnityEngine;
using UnityEngine.VFX;

public class VFXContainer : MonoBehaviour
{
    [SerializeField] public GameObject elecArcPrefab;

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
}
