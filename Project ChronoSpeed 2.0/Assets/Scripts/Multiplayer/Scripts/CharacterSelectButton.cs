using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    private Image iconImage;
    private MultiplayerCarSelection carSelect;
    private CarCharacter carC;

    private void Awake()
    {
        iconImage.GetComponent<Image>();
    }
    public void SetCharacter(MultiplayerCarSelection carselect, CarCharacter carc)
    {
        iconImage = carc.CarIcon;
        carSelect = carselect;
        carC = carc;
    }

    public void SelectCharacter()
    {
        carSelect.Select(carC);
    }

}
