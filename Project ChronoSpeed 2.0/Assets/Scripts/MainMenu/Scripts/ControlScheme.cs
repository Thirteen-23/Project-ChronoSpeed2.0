using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlScheme : MonoBehaviour
{
    [SerializeField] Sprite XboxControls;
    [SerializeField] Sprite PS4Controls;

    private void Start()
    {
        if(XboxControls != null & PS4Controls != null)
        {
            Image image = GetComponent<Image>();
            if(OptionsManager.instance.IsControllerXbox)
            {
                image.sprite = XboxControls;
            }  
            else
            {
                image.sprite = XboxControls;
            }
        }
    }
    public void SetIsXbox(bool isXbox)
    {
        OptionsManager.instance.IsControllerXbox = isXbox;
    }

}
