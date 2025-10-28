using UnityEngine;

public class FrameShopUI : MonoBehaviour
{
    public static FrameShopUI Instance;

    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private bool isInventoryMode = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (PlayerImageManager.Instance == null)
        {
            Debug.LogError("[FrameShopUI] PlayerImageManager.Instance is missing!");
            return;
        }

        RefreshGrid();
    }

    public void RefreshGrid()
    {
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        var frames = PlayerImageManager.Instance.allFrames;
        if (frames == null || frames.Count == 0)
        {
            Debug.LogWarning("[FrameShopUI] No frames found!");
            return;
        }

        for (int i = 0; i < frames.Count; i++)
        {
            bool unlocked = PlayerImageManager.Instance.IsFrameUnlocked(i);

            if (isInventoryMode && !unlocked)
                continue;

            var obj = Instantiate(tilePrefab, gridContainer);
            var tile = obj.GetComponent<PlayerFrameTile>();
            if (tile == null)
            {
                Debug.LogError("[FrameShopUI] Missing PlayerFrameTile script on prefab!");
                continue;
            }
            tile.Setup(frames[i], i, unlocked);
        }
    }


    public void RefreshAllTiles()
    {
        foreach (Transform child in gridContainer)
        {
            var tile = child.GetComponent<PlayerFrameTile>();
            if (tile != null)
                tile.ApplyStateVisuals();
        }
    }
}
