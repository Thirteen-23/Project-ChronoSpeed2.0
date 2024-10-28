using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfOnLeftDisable : MonoBehaviour
{
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(0).gameObject.SetActive(IsPlayerLeft());
    }

    bool IsPlayerLeft()
    {
        Vector3 direction = Quaternion.Inverse(transform.rotation) * (player.position - transform.position);

        bool isForward = direction.z > 0;
        return isForward;
    }
}
