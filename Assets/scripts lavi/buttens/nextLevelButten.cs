using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nextLevelButten : MonoBehaviour
{
    public void GoNextLevelButten()
    {
        WorldManager.Instance.LoadNextLevel();
    }
}
