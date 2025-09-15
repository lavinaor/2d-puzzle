using UnityEngine;

public class SpecialCandyShopUI : MonoBehaviour
{
    public static SpecialCandyShopUI Instance;

    [Header("UI Refs")]
    [SerializeField] private RectTransform gridContainer;  // הגריד בתוך הסקרול
    [SerializeField] private GameObject tilePrefab;        // הפריפאב של הטייל

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // מעדיפים Start על OnEnable כדי שה-Manager יספיק להתאפס ב-Awake
        RefreshGrid();
    }

    /// <summary>
    /// בונה מחדש את הגריד של החנות לפי כל הממתקים במשחק
    /// </summary>
    public void RefreshGrid()
    {
        // אם אין Manager פעיל, לא עושים כלום
        if (SpecialCandyManager.Instance == null)
        {
            Debug.LogError("SpecialCandyManager.Instance is NULL! Make sure the manager is in the scene.");
            return;
        }

        // מנקה את הגריד
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        // מושך את הרשימה הראשית של כל הממתקים
        var allCandies = SpecialCandyManager.Instance.allCandies;

        foreach (var candy in allCandies)
        {
            var obj = Instantiate(tilePrefab, gridContainer);

            // מושך את הכמות שיש לשחקן מהממתק הזה
            int playerAmount = SpecialCandyManager.Instance.GetCandyAmount(candy.candyId);

            // שולח את כל המידע לטייל
            var tile = obj.GetComponent<PowerupTile>();
            tile.Setup(candy, playerAmount);
        }
    }
}
