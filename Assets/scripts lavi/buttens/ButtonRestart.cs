using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonRestart : MonoBehaviour
{
    public void RestartSeane()
    {
        string active = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(active);
    }
}
