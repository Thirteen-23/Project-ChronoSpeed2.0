using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PositionTracker : NetworkBehaviour
{
    [SerializeField] Transform minimapParent;
    [SerializeField] Image playerIconPrefab;
    [SerializeField] Vector3 offset;
    Collider boundsToUVCol;
    Vector3 extents;

    Vector3 maxes;
    Vector3 mins;
    private float xRange;
    private float zRange;

    class PlayerMapInfo
    {
        public Transform PlayerTrans;
        public Image PlayerIcon;
        public PlayerMapInfo(Transform plyr, Image plyrIcon)
        {
            PlayerTrans = plyr; PlayerIcon = plyrIcon; 
        }
    }
    List<PlayerMapInfo> players = new List<PlayerMapInfo>();

    // Start is called before the first frame update
    void Start()
    {
        boundsToUVCol = GetComponent<Collider>();
        extents = boundsToUVCol.bounds.extents;

        maxes = transform.position + extents;
        mins = transform.position - extents;
        xRange = maxes.x - mins.x;
        zRange = maxes.z - mins.z;
    }

    void AddPlayer(Transform plyr, Image plyrIcon)
    {
        players.Add(new PlayerMapInfo(plyr, plyrIcon));
    }

    private void OnTriggerEnter(Collider other)
    {
        var fkCol = other.GetComponent<FakeCollision>();
        if(fkCol != null)
        {
            AddPlayer(fkCol.myTransform, Instantiate(playerIconPrefab, minimapParent));
        }
    }
    private void OnTriggerStay(Collider other)
    {
        var fkCol = other.GetComponent<FakeCollision>();
        if(fkCol != null)
        {
            Vector2 mappedVal;
            mappedVal.x = (fkCol.myTransform.position.x - offset.x - mins.x) / xRange;
            mappedVal.y = (fkCol.myTransform.position.z - offset.z - mins.z) / zRange;
            for(int i = 0; i < players.Count; i++)
            {
                if (players[i].PlayerTrans == fkCol.myTransform)
                {
                    var rectTrans = players[i].PlayerIcon.transform.GetComponent<RectTransform>();
                    rectTrans.anchoredPosition = mappedVal * rectTrans.parent.GetComponent<RectTransform>().sizeDelta;
                }
            }
        }
    }
}
