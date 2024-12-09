using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlScheme : MonoBehaviour
{
    Image XboxControls;
    Image PS4Controls;

    private void Start()
    {
        if(XboxControls != null & PS4Controls != null)
        {
            if(OptionsManager.instance.IsControllerXbox)
            {
                XboxControls.enabled = true;
                PS4Controls.enabled = false;
            }  
            else
            {
                XboxControls.enabled = false;
                PS4Controls.enabled = true;
            }
        }
    }
    public void SetIsXbox(bool isXbox)
    {
        OptionsManager.instance.IsControllerXbox = isXbox;
    }

}
