using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    private Image iconImage;
    private Button button;
    [SerializeField] private MultiplayerCarSelection carSelect;
    [SerializeField] private CarCharacter CarC;

    public bool IsDisabled { get; private set; }
    private void Awake()
    {
        iconImage = GetComponent<Image>();
    }
    public void SetCharacter(MultiplayerCarSelection carselect, CarCharacter carC)
    {
        carSelect = carselect;
        CarC = carC;
    }

    public void SelectCharacter()
    {
        carSelect.SelectSkin(CarC);
    }

    public void SetDisabled()
    {
        IsDisabled = true;
        button.interactable = false;
    }

}
