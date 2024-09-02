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
            if(carSpawn.childCount != 0)
            {
                Destroy(carSpawn.GetChild(0).gameObject); //Later make it portal out
            }
            
            var car = carDatabase.GetCarById(state.CharacterID);
            Instantiate(car.CarModel, carSpawn); //Later make it portal in

        }
        playerNameText.GetComponentInParent<Canvas>().enabled = true;
        playerNameText.text = $"Player {state.ClientID} (Picking...)";
        playerNameText.gameObject.SetActive(true);
        carSpawn.gameObject.SetActive(true);
    }

    public void UpdateName(bool LockedIn, ulong ClientID)
    {
        playerNameText.text = LockedIn ? $"Player {ClientID}" : $"Player {ClientID} (Picking...)";
    }
    public void DisableDisplay()
    {
        playerNameText.GetComponentInParent<Canvas>().enabled = false;
        playerNameText.gameObject.SetActive(false);
        carSpawn.gameObject.SetActive(false);
    }
}
