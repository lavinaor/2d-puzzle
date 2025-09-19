using UnityEngine;

public class SpecialCandyShopUI : MonoBehaviour
{
    public static SpecialCandyShopUI Instance;

    [Header("UI Refs")]
    [SerializeField] private RectTransform gridContainer; // הגריד בתוך הסקרול
    [SerializeField] private GameObject tilePrefab;       // הפריפאב של הטייל

    [Header("Settings")]
    [SerializeField] private bool isInventoryMode = false;
    // ✅ אם מסומן - מצב אינוונטורי, אם לא - מצב חנות

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // משתמשים ב-Start כדי שנספיק לטעון את ה-SpecialCandyManager
        RefreshGrid();
    }

    /// <summary>
    /// בונה מחדש את הגריד של החנות או האינוונטורי
    /// </summary>
    public void RefreshGrid()
    {
        if (SpecialCandyManager.Instance == null)
        {
            Debug.LogError("SpecialCandyManager.Instance is NULL! Make sure the manager is in the scene.");
            return;
        }

        // מנקה את הגריד הקיים
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        // מושך את רשימת כל הממתקים
        var allCandies = SpecialCandyManager.Instance.allCandies;

        foreach (var candy in allCandies)
        {
            int playerAmount = SpecialCandyManager.Instance.GetCandyAmount(candy.candyId);

            // ✅ אם מצב אינוונטורי - מציגים רק אם יש לשחקן כמות > 0
            if (isInventoryMode && playerAmount <= 0)
                continue;

            var obj = Instantiate(tilePrefab, gridContainer);

            var tile = obj.GetComponent<PowerupTile>();
            tile.Setup(candy, playerAmount);

            // במצב אינוונטורי - מחביאים את כפתור הקנייה
            if (isInventoryMode)
                tile.SetBuyButtonVisible(false);
            else
                tile.SetBuyButtonVisible(true);
        }
    }
}
