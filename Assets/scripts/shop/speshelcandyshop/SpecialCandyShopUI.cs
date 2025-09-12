using UnityEngine;

public class SpecialCandyShopUI : MonoBehaviour
{
    public static SpecialCandyShopUI Instance;

    [Header("UI Refs")]
    [SerializeField] private RectTransform gridContainer;  // ����� ���� ������
    [SerializeField] private GameObject tilePrefab;        // ������� �� �����

    [Header("Data")]
    [SerializeField] private SpecialCandyData[] allCandies; // �� ���� ������� ��������

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        RefreshGrid();
    }

    public void RefreshGrid()
    {
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        foreach (var candy in allCandies)
        {
            var obj = Instantiate(tilePrefab, gridContainer);
            var tile = obj.GetComponent<PowerupTile>();
            tile.Setup(candy);
        }
    }
}
