using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToGameObjects : MonoBehaviour
{
    public Tilemap tilemap; // ה-Tilemap המקורי
    public GameObject[] tilePrefabs; // רשימת הפריפבים המתאימים לאריחים
    private GameObject[,] tileGrid; // מערך דו-ממדי של GameObjects
    private GameObject parentGrid; // ההורה של כל האריחים

    void Start()
    {
        Debug.Log("Tilemap Bounds: " + tilemap.cellBounds);
        ConvertTilemapToGameObjects();
    }

    void ConvertTilemapToGameObjects()
    {
        BoundsInt bounds = tilemap.cellBounds;
        tileGrid = new GameObject[bounds.size.x, bounds.size.y];

        // יצירת אובייקט הורה
        parentGrid = new GameObject("TileGridParent");

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    Debug.Log(tile.name);
                    int prefabIndex = GetPrefabIndex(tile);
                    if (prefabIndex != -1)
                    {
                        Vector3 worldPos = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                        GameObject newTile = Instantiate(tilePrefabs[0], worldPos, Quaternion.identity);
                        newTile.transform.parent = parentGrid.transform; // מציב תחת ההורה
                        tileGrid[x - bounds.xMin, y - bounds.yMin] = newTile;
                    }
                }
            }
        }

        Destroy(tilemap.gameObject); // מוחק את האובייקט של ה-Tilemap אבל שומר את האריחים
    }

    int GetPrefabIndex(TileBase tile)
    {
        for (int i = 0; i < tilePrefabs.Length; i++)
        {
            if (tilePrefabs[i].name == tile.name) // הנחה: שם הפריפב זהה לשם האריח
            {
                return i;
            }
        }
        return -1;
    }
}
