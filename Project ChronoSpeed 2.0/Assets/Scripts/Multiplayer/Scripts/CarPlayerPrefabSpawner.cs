using Unity.Netcode;
using UnityEngine;

public class CarPlayerPrefabSpawner : NetworkBehaviour
{
    [SerializeField] public CarCharacterStorage carDatabase;
    [SerializeField] public GameObject UtopiaAiCar;
    [SerializeField] public GameObject PresentAiCar;
    [SerializeField] public GameObject DystopiaAiCar;
    [SerializeField] public Transform[] startingPositions = new Transform[12];
    
}
