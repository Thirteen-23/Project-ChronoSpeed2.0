using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeadboardPlayerBar : MonoBehaviour
{
    [SerializeField] public TMP_Text playerNameText;

    [SerializeField] public TMP_Text lapCountText;

    [SerializeField] public TMP_Text raceCompletionText;

    [SerializeField] public Transform YouSign;

    public void SetFinishedTime(bool setTo)
    {
        raceCompletionText.gameObject.SetActive(setTo);
        lapCountText.gameObject.SetActive(!setTo);
    }

    public void SetYouSign(bool setTo)
    {
        foreach (var image in YouSign.GetComponentsInChildren<Image>())
        {
            image.enabled = setTo;
        }
    }
}
