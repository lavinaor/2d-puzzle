using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSettings : MonoBehaviour
{
    public void ButtonOpenSettings()
    {
        PopUpManger.Instance.ChangeUIState(1);
    }
}
