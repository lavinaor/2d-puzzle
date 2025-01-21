using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class candy : MonoBehaviour
{

    public candyType candyType;

    public int xIndex;
    public int yIndex;

    public bool isMatched;
    private Vector2 current;
    private Vector2 targetPause;

    public bool isMoving;

    public candy(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    public void setIndicies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }


}

public enum candyType
{
    Red,
    Blue,
    Green,
    Purple,
    White
}
