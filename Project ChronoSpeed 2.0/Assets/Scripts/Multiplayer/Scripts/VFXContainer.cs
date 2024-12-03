using UnityEngine;
using UnityEngine.VFX;

public class VFXContainer : MonoBehaviour
{
    [SerializeField] public GameObject elecArcPrefab;
    [SerializeField] public TrailRenderer[] speedLimitRemoverTrails = new TrailRenderer[4];
    [SerializeField] public TrailRenderer[] drifting;
    [SerializeField] ParticleSystem[] m_NBoost;
    [SerializeField] MeshRenderer[] m_MRS;
    [SerializeField] Material hologramMat;
    [SerializeField] Material basicMat;

    [HideInInspector] public VisualEffect electricArc;
    public enum VFXTypes
    {
        spinSmoke,
        straightSmoke,
        blackRockChips,
        electricBall,
        SpeedLimitRemover,
        Boosting, 
        Drifting, 
        Ghosting,
    }

    public void SetVFX(VFXTypes type, bool setTo)
    {
        switch(type)
        {
            case VFXTypes.electricBall:
                electricArc.enabled = setTo;
                break;
            case VFXTypes.SpeedLimitRemover:
                foreach (TrailRenderer SM in speedLimitRemoverTrails)
                {
                    SM.emitting = setTo;
                }

                break;
            case VFXTypes.Boosting:
                foreach (ParticleSystem SM in m_NBoost)
                {
                    if (setTo)
                        SM.Play();
                    else
                        SM.Stop();  
                }
                break;
            case VFXTypes.Drifting:
                foreach (TrailRenderer SM in drifting)
                {
                    SM.emitting = setTo;
                }

                break;
            case VFXTypes.Ghosting:
                foreach(MeshRenderer SM in m_MRS)
                {
                    Material[] tempMats = SM.materials;
                    tempMats[1] = setTo ? hologramMat : basicMat;
                    SM.materials = tempMats;

                }
                break;
        }
    }
}
