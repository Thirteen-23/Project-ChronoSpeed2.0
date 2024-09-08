using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class CarPlayerPrefabSpawner : NetworkBehaviour
{
    [SerializeField] private CarCharacterStorage carDatabase;
    [SerializeField] private Transform[] startingPositions = new Transform[12];
    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            int i = 11;
            foreach (var client in ServerManager.Singleton.ClientDic)
            {
                var car = carDatabase.GetCarById(client.Value.CharacterId);
                if (car != null)
                {
                    var playerObject = Instantiate(car.CarPlayable, startingPositions[i]);
                    playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.Value.ClientId);
                    i--;
                }
            }

            //Spawn AI in the rest of the slots
            while (i >= 0)
            {
                i--;
            }
        }
    }
}
