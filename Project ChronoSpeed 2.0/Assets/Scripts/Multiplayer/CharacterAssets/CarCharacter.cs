using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "CarSelection/Car")]
public class CarCharacter : ScriptableObject
{
    [SerializeField] private int id = -1;
    [SerializeField] private string carName = "New Display Name";
    [TextArea(15, 20)]
    [SerializeField] private string carDescription = "New Description";
    [SerializeField] private Image carIcon;
    [SerializeField] private GameObject carModel;
    [SerializeField] private GameObject carPlayable;

    public int Id => id;
    public string CarName => carName;
    public string CarDesc => carDescription;
    public Image CarIcon => carIcon;
    public GameObject CarModel => carModel;

    public GameObject CarPlayable => carPlayable;
}
