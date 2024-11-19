using UnityEngine;
using UnityEngine.VFX;

public class VFXContainer : MonoBehaviour
{
    [SerializeField] public GameObject elecArcPrefab;
    [SerializeField] public GameObject[] spinSmokes = new GameObject[2];
    [SerializeField] public GameObject[] straightSmokes = new GameObject[2];
    /*[HideInInspector]*/
    public VisualEffect electricArc;
    public enum VFXTypes
    {
        spinSmoke,
        straightSmoke,
        blackRockChips,
        electricBall,
    }

    public void SetVFX(VFXTypes type, bool setTo)
    {
        switch(type)
        {
            case VFXTypes.electricBall:
                electricArc.enabled = setTo;
                break;
            case VFXTypes.spinSmoke:
                foreach(GameObject SM in spinSmokes)
                {
                    SM.SetActive(setTo);
                }
                break;
            case VFXTypes.straightSmoke:
                foreach(GameObject SM in straightSmokes)
                {
                    SM.SetActive(setTo);
                }
                break;
        }
        
        Debug.Log("Switched " + type.ToString() + "to " + setTo);
    }
}
