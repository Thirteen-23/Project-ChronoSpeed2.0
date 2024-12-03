using UnityEngine;

public class ColliderFollow : MonoBehaviour
{
    Transform carBase;
    [SerializeField] Vector3 offset;
    private void Start()
    {
        carBase = transform.root;
        transform.parent = null;
    }

    private void Update()
    {
        transform.position = carBase.position + offset;
        transform.rotation = carBase.rotation;
    }

}
