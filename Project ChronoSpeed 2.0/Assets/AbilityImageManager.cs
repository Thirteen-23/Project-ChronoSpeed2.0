using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityImageManager : MonoBehaviour
{


    [SerializeField] AbilityManager m_AbilityReference;
    [SerializeField] TimeRecording checkingRewindTime; 
    [SerializeField] Image[] m_abilityImages = new Image[2];
    [SerializeField] Image m_Boost;
    [SerializeField] Image m_Rewind;
    [SerializeField] float daBar; 
    // Start is called before the first frame update
    void Start()
    {
        m_AbilityReference = GetComponentInParent<AbilityManager>();
        checkingRewindTime = GetComponentInParent<TimeRecording>();
       
    }

    private void FixedUpdate()
    {
        AbilityCheck();
        BoostCheck();
        TimeRewind();
    }

    private void AbilityCheck()
    {
        
        foreach(Image r in m_abilityImages)
        {
            r.fillAmount = m_AbilityReference.currentAbilityValue / 50f;
        }

    }
    private void BoostCheck()
    {
        m_Boost.fillAmount = m_AbilityReference.boostBarBar.fillAmount; 
    }

    private void TimeRewind()
    {
        m_Rewind.fillAmount = checkingRewindTime.countDown / checkingRewindTime.cooldown;
      
    }
}
