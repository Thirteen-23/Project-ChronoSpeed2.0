using UnityEngine;
using UnityEngine.VFX;

public class VFXContainer : MonoBehaviour
{
    [SerializeField] public GameObject elecArcPrefab;
    [SerializeField] public GameObject[] spinSmokes = new GameObject[2];
    [SerializeField] public GameObject[] straightSmokes = new GameObject[2];
    [SerializeField] public TrailRenderer[] speedLimitRemoverTrails = new TrailRenderer[4];
    [SerializeField] public TrailRenderer[] drifting;
    [SerializeField] GameObject[] nitroBoostVFX;
    [SerializeField] ParticleSystem[] m_NBoost; 

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
        Drifting, 

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
                //foreach (GameObject SM in nitroBoostVFX)
                //{
                //    SM.SetActive(setTo);
                   
                //}
                foreach (ParticleSystem SM in m_NBoost)
                {if (setTo)
                    { 
                        SM.Play(true); 
                    }
                else
                    {
                        SM.Stop(); 
                    }
                

                }
                    break;
            case VFXTypes.Drifting:
                foreach (TrailRenderer SM in drifting)
                {
                    SM.emitting = setTo;
                }

                break;
        }
    }
}
