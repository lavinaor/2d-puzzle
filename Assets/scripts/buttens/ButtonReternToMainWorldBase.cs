using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonReternToMainWorldBase : MonoBehaviour
{
    public void ReternToMainWorldBase()
    {
        string mainSceneName = WorldManager.Instance.GetMainlevelSceneName(WorldManager.Instance.getLevel());
        SceneManager.LoadScene(mainSceneName);
    }
}
