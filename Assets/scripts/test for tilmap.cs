using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToGameObjects : MonoBehaviour
{
    public Tilemap tilemap; // �-Tilemap ������
    public GameObject[] tilePrefabs; // ����� �������� �������� �������
    private GameObject[,] tileGrid; // ���� ��-���� �� GameObjects
    private GameObject parentGrid; // ����� �� �� �������

    void Start()
    {
        Debug.Log("Tilemap Bounds: " + tilemap.cellBounds);
        ConvertTilemapToGameObjects();
    }

    void ConvertTilemapToGameObjects()
    {
        BoundsInt bounds = tilemap.cellBounds;
        tileGrid = new GameObject[bounds.size.x, bounds.size.y];

        // ����� ������� ����
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
                        newTile.transform.parent = parentGrid.transform; // ���� ��� �����
                        tileGrid[x - bounds.xMin, y - bounds.yMin] = newTile;
                    }
                }
            }
        }

        Destroy(tilemap.gameObject); // ���� �� �������� �� �-Tilemap ��� ���� �� �������
    }

    int GetPrefabIndex(TileBase tile)
    {
        for (int i = 0; i < tilePrefabs.Length; i++)
        {
            if (tilePrefabs[i].name == tile.name) // ����: �� ������ ��� ��� �����
            {
                return i;
            }
        }
        return -1;
    }
}
