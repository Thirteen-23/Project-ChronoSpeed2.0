using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarSpotlight : MonoBehaviour
{
    [SerializeField] private CarCharacterStorage carDatabase;
    [SerializeField] TMP_Text playerNameText;
    private Transform carSpawn;

    private void Awake()
    {
        carSpawn = transform.GetChild(0);
    }

    //TODO car select animation
    public void UpdateDisplay(CharacterSelectState state)
    {
        if (state.CharacterID != -1)
        {
            Destroy(carSpawn.GetChild(0)); //Later make it portal out
            var car = carDatabase.GetCarById(state.CharacterID);
            Instantiate(car.CarModel, carSpawn); //Later make it portal in

        }
        playerNameText.GetComponentInParent<Canvas>().enabled = true;
        playerNameText.text = $"Player {state.ClientID}";
        carSpawn.gameObject.SetActive(true);
    }

    public void DisableDisplay()
    {
        playerNameText.GetComponentInParent<Canvas>().enabled = false;
        carSpawn.gameObject.SetActive(false);
    }
}
