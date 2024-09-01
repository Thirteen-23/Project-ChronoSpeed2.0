using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    private Image iconImage;
    private Button button;
    private MultiplayerCarSelection carSelect;

    public CarCharacter CarC { get; private set; }
    public bool IsDisabled { get; private set; }
    private void Awake()
    {
        iconImage = GetComponent<Image>();
    }
    public void SetCharacter(MultiplayerCarSelection carselect, CarCharacter carC)
    {
        iconImage.sprite = carC.CarIcon.sprite;
        iconImage.color = carC.CarIcon.color;
        carSelect = carselect;
        CarC = carC;
    }

    public void SelectCharacter()
    {
        carSelect.Select(CarC);
    }

    public void SetDisabled()
    {
        IsDisabled = true;
        button.interactable = false;
    }

}
