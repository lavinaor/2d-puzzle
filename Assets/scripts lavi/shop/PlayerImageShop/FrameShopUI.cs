using UnityEngine;

public class FrameShopUI : MonoBehaviour
{
    public static FrameShopUI Instance;

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

        var frames = PlayerImageManager.Instance.allFrames;

        for (int i = 0; i < frames.Count; i++)
        {
            bool unlocked = PlayerImageManager.Instance.IsFrameUnlocked(i);

            if (isInventoryMode && !unlocked)
                continue;

            var obj = Instantiate(tilePrefab, gridContainer);
            var tile = obj.GetComponent<PlayerFrameTile>();
            tile.Setup(frames[i], i, unlocked);
        }
    }
}
