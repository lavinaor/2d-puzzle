using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCandySkin", menuName = "Candy/Candy Skin")]
public class CandySkin : ScriptableObject
{
    public string skinName;
    public List<CandyTypeSpritePair> candyPairs = new List<CandyTypeSpritePair>();
}
