using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyBoard : MonoBehaviour
{

    //����� �� ���� ����
    public int width = 6;
    public int height = 8;
    public int spacingDevidor = 2;

    // ����� ������ ����
    public float spacingX;
    public float spacingY;

    //���� �� �������� �� �������
    public GameObject[] candyPrefabs;

    //���� �� ���� ��� ��������
    private Node[,] candyBoard;
    public GameObject candyBoardGO;

    //layoutarray
    

    // ����� ����� �����
    public static CandyBoard instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        initializeBoard();
    }

    void initializeBoard()
    {
        candyBoard = new Node[width, height];

        spacingX = (float)(width - 1) / 2;
        spacingY = (float)(height - 1) / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //���� �����
                Vector2 pos = new Vector2(x - spacingX, y - spacingY);

                //���� ���� ������� �� ���� ��� ����
                int randomIndex = Random.Range(0, candyPrefabs.Length);

                //���� ���� ����� �������� ������ ������ ������ ���� ���� ���
                GameObject candy =  Instantiate(candyPrefabs[randomIndex], pos / spacingDevidor, Quaternion.identity);
                //����� ���� 
                candy.GetComponent<candy>().setIndicies(x / spacingDevidor, y / spacingDevidor);
                // ����� ���� �����
                candyBoard[x,y] = new Node(true, candy);
            }
        }
    }
}
