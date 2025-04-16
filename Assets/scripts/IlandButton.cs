using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IlandButton : SceneChanger
{
    [SerializeField] int lockeLevel = 0;
    public override void OnChangeSeneDeley(string sceneName)
    {
        if (lockeLevel < SaveManager.Instance.GetStarsInTotal())
        {
            base.OnChangeSeneDeley(sceneName);
        }
    }
}
