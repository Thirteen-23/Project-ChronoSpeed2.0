using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PositionTracker : NetworkBehaviour
{
    [SerializeField] Transform minimapParent;
    [SerializeField] Image playerIconPrefab;
    Collider boundsToUVCol;
    Vector3 extents;
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
        AddPlayer(FindAnyObjectByType<Car_Movement>().transform, Instantiate(playerIconPrefab, minimapParent));
    }

    void AddPlayer(Transform plyr, Image plyrIcon)
    {
        players.Add(new PlayerMapInfo(plyr, plyrIcon));
    }

    private void OnTriggerStay(Collider other)
    {
        var fkCol = other.GetComponent<FakeCollision>();
        if(fkCol != null)
        {
            Vector3 plyrPos = fkCol.myTransform.position;

            Vector2 newIconPos = new Vector2(plyrPos.x / (extents.x * 2),
                                                plyrPos.z / (extents.z * 2));

            for(int i = 0; i < players.Count; i++)
            {
                if (players[i].PlayerTrans == fkCol.myTransform)
                {
                    var rectTrans = players[i].PlayerIcon.transform.GetComponent<RectTransform>();
                    rectTrans.anchoredPosition = newIconPos * rectTrans.parent.GetComponent<RectTransform>().sizeDelta;
                    Debug.Log(rectTrans.parent.GetComponent<RectTransform>().sizeDelta);
                }
            }
        }
    }
}
