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
        if (goals.Count < 1)
        {
            Destroy(this.gameObject);
        }

        for (int i = goalsOBG.Count - 1; i >= 0; i--)
        {
            if (i < goals.Count)
            {
                goalsOBG[i].SetLook(goals[i]);
            }
            else
            {
                GoalDesplayer goalDesplayer = goalsOBG[i];
                goalsOBG.RemoveAt(i);
                Destroy(goalDesplayer.gameObject);
            }
        }

    }
}
