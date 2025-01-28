using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyBoard : MonoBehaviour
{

    //����� �� ���� ����
    public int width = 6;
    public int height = 8;

    // ����� ������ ����
    public float spacingX;
    public float spacingY;

    //���� �� �������� �� �������
    public GameObject[] candyPrefabs;

    //���� �� ���� ��� ��������
    private Node[,] candyBoard;
    public GameObject candyBoardGO;

    // ����� ����� �����
    public static CandyBoard instance;

    //������� ������ �� �� ���� �����
    GameObject boardParent;

    //����� ����� �� ��� ����� �� ����
    public float boardScaleFactor = 1.0f;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        initializeBoard();
    }

    void ScaleBoardToFitScreen()
    {
        // ���� ��� ��������
        float boardWidth = width;
        float boardHeight = height;

        // ��� ����
        float screenWidth = Camera.main.orthographicSize * 2 * Screen.width / Screen.height;
        float screenHeight = Camera.main.orthographicSize * 2;

        // ����� ��� ����� ������
        float scaleX = screenWidth / boardWidth;
        float scaleY = screenHeight / boardHeight;
        float scale = Mathf.Min(scaleX, scaleY);

        // ���� �� ��� ����� ��� ������ boardScaleFactor
        scale *= boardScaleFactor;

        // ����� ��� ���� �� ����
        boardParent.transform.localScale = new Vector3(scale, scale, 1);
    }


    void initializeBoard()
    {
        candyBoard = new Node[width, height];

        // ����� ������� �� ����
        boardParent = new GameObject("BoardParent");

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
                GameObject candy =  Instantiate(candyPrefabs[randomIndex], pos, Quaternion.identity);

                // ����� �� ����� ��� ������� ���
                candy.transform.SetParent(boardParent.transform);

                //����� ���� 
                candy.GetComponent<candy>().setIndicies(x, y);
                // ����� ���� �����
                candyBoard[x,y] = new Node(true, candy);
            }
        }
        if (CheckBoard())
        {
            Debug.Log("ther are maches recreate the bord");
            Destroy(boardParent);
            initializeBoard();
        }
        else
        {
            Debug.Log("ther are no maches");
            ScaleBoardToFitScreen();
        }
    }

    public bool CheckBoard()
    {
        //����� �� ����� ������
        Debug.Log("checking board");

        // ��� ������ ����� ��� ���� ������ ����� �� �����
        bool hasMatched = false;

        // ����� �� ������ ����� ����� �� �� �����
        List<candy> candyToRemove = new();


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) 
            { 
                //���� �� ������ ����
                if (candyBoard[x, y].isUsabal)
                {
                    //���� �� ��� �����
                    candy candy = candyBoard[x, y].candy.GetComponent<candy>();

                    //���� �� ��� �����
                    if (!candy.isMatched)
                    {
                        MatchResults matchCandy = IsConnected(candy);

                        if (matchCandy.connectedCandy.Count >= 3)
                        {
                            //���� ������ ������� ������

                            //����� �� ������� ������� ������ ������
                            candyToRemove.AddRange(matchCandy.connectedCandy);

                            foreach (candy can in matchCandy.connectedCandy)
                            {
                                can.isMatched = true;
                            }

                            hasMatched = true;
                        }
                    }
                }
            }
        }
        
        return hasMatched;
    }

    //����� �� �����
    MatchResults IsConnected(candy candy)
    {
        List<candy> connectedCandy = new();
        candyType candyType = candy.candyType;

        connectedCandy.Add(candy);

        //���� �����
        CheckDirection(candy, new Vector2Int(1,0), connectedCandy);

        //���� �����
        CheckDirection(candy, new Vector2Int(-1, 0), connectedCandy);

        // ���� �� �� 3 �����
        if (connectedCandy.Count == 3)
        {
            // ����� �� ����� �� 3
            Debug.Log("has mached Horizontal 3 frome type: " + connectedCandy[0].candyType);

            return new MatchResults
            {
                connectedCandy = connectedCandy,
                direction = MatchDirection.Horizontal,
            };
        }

        // ���� �� �� ���� �3 �����
        else if (connectedCandy.Count > 3)
        {
            // ����� �� ����� �� 3
            Debug.Log("has Horizontal mached more then 3 frome type: " + connectedCandy[0].candyType);

            return new MatchResults
            {
                connectedCandy = connectedCandy,
                direction = MatchDirection.LongHorizontal,
            };
        }

        // �� ��� ����� ���� �� ������
        connectedCandy.Clear();

        //����� �� �������
        connectedCandy.Add(candy);

        // ���� �����
        CheckDirection(candy, new Vector2Int(0, 1), connectedCandy);

        //���� ����
        CheckDirection(candy, new Vector2Int(0, -1), connectedCandy);

        // ���� �� �� 3 �����
        if (connectedCandy.Count == 3)
        {
            // ����� �� ����� �� 3
            Debug.Log("has Vertical mached 3 frome type: " + connectedCandy[0].candyType);

            return new MatchResults
            {
                connectedCandy = connectedCandy,
                direction = MatchDirection.Vertical,
            };
        }

        // ���� �� �� ���� �3 �����
        else if (connectedCandy.Count > 3)
        {
            // ����� �� ����� �� 3
            Debug.Log("has Vertical mached more then 3 frome type: " + connectedCandy[0].candyType);

            return new MatchResults
            {
                connectedCandy = connectedCandy,
                direction = MatchDirection.LongVertical,
            };
        }

        //�� ���� ��� �����
        else
        {
            return new MatchResults
            {
                connectedCandy = connectedCandy,
                direction = MatchDirection.None,
            };
        }
    }

    //���� �����
    void CheckDirection(candy candy, Vector2Int direction, List<candy> connectedCandy)
    {
        candyType candyType = candy.candyType;
        int x = candy.xIndex + direction.x;
        int y = candy.yIndex + direction.y;

        // ���� ��� ���� ����
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (candyBoard[x, y].isUsabal)
            {
                 candy neighbourCandy = candyBoard[x, y].candy.GetComponent<candy>();

                // ��� ���� ���� ���
                if (!neighbourCandy.isMatched && neighbourCandy.candyType == candyType)
                {
                    // ����� �� ����� ���� ������ 
                    connectedCandy.Add(neighbourCandy);

                    // ���� ��� ����� �����
                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }
}


// ���� ����

//����� �� �����

//����� �����

//���� �� �� �������

//���� �������







// ���� �� ������� 
public class MatchResults
{
    public List<candy> connectedCandy;
    public MatchDirection direction;
}

// ������� �������
public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super,
    None
}