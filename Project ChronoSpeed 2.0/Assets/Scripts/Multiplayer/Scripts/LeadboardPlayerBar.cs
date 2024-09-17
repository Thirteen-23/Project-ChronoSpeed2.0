using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeadboardPlayerBar : MonoBehaviour
{
    [SerializeField] public TMP_Text playerNameText;

    [SerializeField] public TMP_Text lapCountText;

    [SerializeField] public TMP_Text raceCompletionText;

    [SerializeField] public TMP_Text placementText;

    [SerializeField] public Transform YouSign;

    public void SetFinishedTime(bool setTo)
    {
        raceCompletionText.gameObject.SetActive(setTo);
        lapCountText.gameObject.SetActive(!setTo);
    }

    public void SetYouSign(bool setTo)
    {
        YouSign.GetComponentInChildren<Image>().enabled = setTo;
        YouSign.GetComponentInChildren<TMP_Text>().enabled = setTo;
    }
}
