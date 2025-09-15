using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CandySkin))]
public class CandySkinEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CandySkin skin = (CandySkin)target;

        if (GUILayout.Button("Fill All Candy Types"))
        {
            FillCandyTypes(skin);
        }

        DrawDefaultInspector();
    }

    private void FillCandyTypes(CandySkin skin)
    {
        skin.candyPairs.Clear();
        foreach (CandyType type in System.Enum.GetValues(typeof(CandyType)))
        {
            CandyTypeSpritePair newPair = new CandyTypeSpritePair();
            newPair.type = type;
            newPair.sprite = null;
            skin.candyPairs.Add(newPair);
        }
        EditorUtility.SetDirty(skin);
    }
}

