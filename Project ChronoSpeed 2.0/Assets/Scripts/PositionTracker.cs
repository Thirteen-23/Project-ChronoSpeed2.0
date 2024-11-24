using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PositionTracker : NetworkBehaviour
{
    [SerializeField] Transform minimapParent;
    [SerializeField] Image playerIconPrefab;
    [SerializeField] Image otherPlayerIconPrefab;
    [SerializeField] Image aiIconPrefab;
    [SerializeField] Vector3 offset;
    Collider boundsToUVCol;
    Vector3 extents;
    Vector2 minimapSizeDelta;

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

        minimapSizeDelta = minimapParent.GetComponent<RectTransform>().sizeDelta;
    }

    void AddPlayer(Transform plyr, Image plyrIcon)
    {
        players.Add(new PlayerMapInfo(plyr, plyrIcon));
    }

    void RemovePlayer(Transform plyr)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].PlayerTrans == plyr)
            {
                Destroy(players[i].PlayerIcon);
                players.Remove(players[i]);
                break;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var fkCol = other.GetComponent<FakeCollision>();
        if(fkCol != null)
        {
            if(fkCol.myTransform.tag == "AI")
            { AddPlayer(fkCol.myTransform, Instantiate(aiIconPrefab, minimapParent)); }
            else if(fkCol.myTransform.tag == "OtherPlayer")
            { AddPlayer(fkCol.myTransform, Instantiate(otherPlayerIconPrefab, minimapParent)); }
            else if(fkCol.myTransform.tag == "Player")
            { AddPlayer(fkCol.myTransform, Instantiate(playerIconPrefab, minimapParent)); }
        }
    }

    private void Update()
    {
        for(int i = 0; i < players.Count; i++)
        {
            Vector2 mappedVal;
            mappedVal.x = (players[i].PlayerTrans.position.x - offset.x - mins.x) / xRange;
            mappedVal.y = (players[i].PlayerTrans.position.z - offset.z - mins.z) / zRange;

            players[i].PlayerIcon.rectTransform.anchoredPosition = 
                mappedVal * minimapSizeDelta;
        }
        
    }
}
