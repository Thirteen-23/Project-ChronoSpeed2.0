using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CarDatabase", menuName = "CarSelection/CarStorage")]
public class CarCharacterStorage : ScriptableObject
{
    [SerializeField] private CarCharacter[] cars = new CarCharacter[0];

    public CarCharacter[] GetAllCars() => cars;

    public CarCharacter GetCarById(int id)
    {
        foreach (var car in cars)
        {
            if (car.Id == id)
            {
                return car;
            }
        }
        return null;
    }

    public bool IsValidCharacterId(int id)
    {
        return cars.Any(x => x.Id == id);
    }
}
