using UnityEngine;
using UnityEngine.VFX;

public class VFXContainer : MonoBehaviour
{
    [SerializeField] public GameObject elecArcPrefab;
    [SerializeField] public GameObject[] spinSmokes = new GameObject[2];
    [SerializeField] public GameObject[] straightSmokes = new GameObject[2];
    [SerializeField] public TrailRenderer[] speedLimitRemoverTrails = new TrailRenderer[4];
    [SerializeField] GameObject[] nitroBoostVFX; 
    /*[HideInInspector]*/
    public VisualEffect electricArc;
    public enum VFXTypes
    {
        spinSmoke,
        straightSmoke,
        blackRockChips,
        electricBall,
        SpeedLimitRemover,
        Boosting, 

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
            case VFXTypes.SpeedLimitRemover:
                foreach (TrailRenderer SM in speedLimitRemoverTrails)
                {
                    SM.emitting = setTo;
                }

                break;
            case VFXTypes.Boosting:
                foreach (GameObject SM in nitroBoostVFX)
                {
                    SM.SetActive(setTo);
                }
                break; 
        }
        
        Debug.Log("Switched " + type.ToString() + "to " + setTo);
    }
}
