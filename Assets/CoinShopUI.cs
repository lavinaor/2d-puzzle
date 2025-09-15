using UnityEngine;

public class CoinShopUI : MonoBehaviour
{
    public RectTransform gridContainer;
    public GameObject tilePrefab;

    private void Start()
    {
        RefreshGrid();
    }

    public void RefreshGrid()
    {
        if (CoinShopManager.Instance == null)
        {
            Debug.LogWarning("CoinShopManager is null!");
            return;
        }

        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        foreach (var package in CoinShopManager.Instance.allPackages)
        {
            GameObject obj = Instantiate(tilePrefab, gridContainer);
            var tile = obj.GetComponent<CoinPackageTile>();
            tile.Setup(package);
        }
    }


}
