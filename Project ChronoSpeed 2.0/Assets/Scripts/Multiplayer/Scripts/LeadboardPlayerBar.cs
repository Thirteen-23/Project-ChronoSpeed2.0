using TMPro;
using UnityEngine;

public class LeadboardPlayerBar : MonoBehaviour
{
    [SerializeField] public TMP_Text playerNameText;

    [SerializeField] public TMP_Text lapCountText;

    [SerializeField] public TMP_Text raceCompletionText;
    [SerializeField] public TMP_Text raceCompletionDivider;

    public void SetFinishedTime(bool setTo)
    {
        raceCompletionText.gameObject.SetActive(setTo);
        raceCompletionDivider.gameObject.SetActive(setTo);
    }


}
