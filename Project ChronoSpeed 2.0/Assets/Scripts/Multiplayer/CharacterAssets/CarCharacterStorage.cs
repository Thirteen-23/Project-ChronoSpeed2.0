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
}
