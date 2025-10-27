using UnityEngine;

public class PortraitShopUI : MonoBehaviour
{
    public static PortraitShopUI Instance;

    public RectTransform gridContainer;
    public GameObject tilePrefab;
    public bool isInventoryMode = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshGrid();
    }

    public void RefreshGrid()
    {
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        var portraits = PlayerImageManager.Instance.allPortraits;

        for (int i = 0; i < portraits.Count; i++)
        {
            bool unlocked = PlayerImageManager.Instance.IsPortraitUnlocked(i);

            if (isInventoryMode && !unlocked)
                continue;

            var obj = Instantiate(tilePrefab, gridContainer);
            var tile = obj.GetComponent<PlayerPortraitTile>();
            tile.Setup(portraits[i], i, unlocked);
        }
    }
}
