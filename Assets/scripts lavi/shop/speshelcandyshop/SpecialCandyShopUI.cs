using UnityEngine;

public class SpecialCandyShopUI : MonoBehaviour
{
    public static SpecialCandyShopUI Instance;

    [Header("UI Refs")]
    [SerializeField] private RectTransform gridContainer;  // ����� ���� ������
    [SerializeField] private GameObject tilePrefab;        // ������� �� �����

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // ������� Start �� OnEnable ��� ��-Manager ����� ������ �-Awake
        RefreshGrid();
    }

    /// <summary>
    /// ���� ���� �� ����� �� ����� ��� �� ������� �����
    /// </summary>
    public void RefreshGrid()
    {
        // �� ��� Manager ����, �� ����� ����
        if (SpecialCandyManager.Instance == null)
        {
            Debug.LogError("SpecialCandyManager.Instance is NULL! Make sure the manager is in the scene.");
            return;
        }

        // ���� �� �����
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        // ���� �� ������ ������ �� �� �������
        var allCandies = SpecialCandyManager.Instance.allCandies;

        foreach (var candy in allCandies)
        {
            var obj = Instantiate(tilePrefab, gridContainer);

            // ���� �� ����� ��� ����� ������ ���
            int playerAmount = SpecialCandyManager.Instance.GetCandyAmount(candy.candyId);

            // ���� �� �� ����� �����
            var tile = obj.GetComponent<PowerupTile>();
            tile.Setup(candy, playerAmount);
        }
    }
}
