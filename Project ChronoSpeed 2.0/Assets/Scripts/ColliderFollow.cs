using UnityEngine;

public class ColliderFollow : MonoBehaviour
{
    Transform carBase;

    private void Start()
    {
        carBase = transform.root;
        transform.parent = null;
    }

    private void Update()
    {
        transform.position = carBase.position;
        transform.rotation = carBase.rotation;
    }

}
