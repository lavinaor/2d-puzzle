using UnityEngine;

public class SkinShopUI : MonoBehaviour
{
    public static SkinShopUI Instance;

    public RectTransform gridContainer;
    public GameObject tilePrefab;
    public SkinPopup popup;

    public int defaultPrice = 100;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshGrid();
        popup.gameObject.SetActive(false);
    }

    public void RefreshGrid()
    {
        foreach (RectTransform child in gridContainer)
            Destroy(child.gameObject);

        var skins = CandySkinManager.Instance.allSkins;
        for (int i = 0; i < skins.Count; i++)
        {
            var obj = Instantiate(tilePrefab, gridContainer);
            var tile = obj.GetComponent<SkinTile>();
            tile.Setup(skins[i], i);
        }
    }

    public void OpenPopup(int skinIndex)
    {
        var skin = CandySkinManager.Instance.allSkins[skinIndex];
        popup.Prepare(skinIndex, skin, defaultPrice);
        PopUpManger.Instance.ChangeUIState((int)PopUpType.skinPopup);

    }
}

