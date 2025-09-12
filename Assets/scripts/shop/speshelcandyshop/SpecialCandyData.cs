using UnityEngine;

[CreateAssetMenu(fileName = "SpecialCandy", menuName = "Shop/Special Candy")]
public class SpecialCandyData : ScriptableObject
{
    public string id;           // מזהה ייחודי לשמירה
    public string displayName;  // השם שמוצג לשחקן
    public Sprite icon;         // האייקון של הממתק
    public int price;           // מחיר במטבעות
}
