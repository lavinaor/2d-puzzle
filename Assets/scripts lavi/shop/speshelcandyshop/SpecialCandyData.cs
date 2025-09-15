using UnityEngine;

[CreateAssetMenu(fileName = "SpecialCandy", menuName = "Shop/Special Candy")]
public class SpecialCandyData : ScriptableObject
{
    public string id;           // ���� ������ ������
    public string displayName;  // ��� ����� �����
    public Sprite icon;         // ������� �� �����
    public int price;           // ���� �������
}
