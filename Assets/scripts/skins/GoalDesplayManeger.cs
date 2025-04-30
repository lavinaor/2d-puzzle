using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDesplayManeger : MonoBehaviour
{
    [SerializeField]
    private List<GoalDesplayer> goalsOBG = new List<GoalDesplayer>();

    // Update is called once per frame
    void Update()
    {
        var goals = GameManager.Instance.GetGoals();
        int  i = 0;
        foreach (GoalDesplayer goal in goalsOBG)
        {
            goal.SetLook(goals[i]);
            i++;
        }
    }
}
