using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
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

    //����� ������ ������
    public List<GameObject> candyToDestroy = new();

    // ����� ����� ����� ������
    [SerializeField]
    private candy selectedCandy;

    //��� ��� ����� ����
    [SerializeField]
    private bool isProcessingMove;

    // ����� �� ������ ����� ����� �� �� �����
    [SerializeField]
    List<candy> candyToRemove = new();

    //������� ������ �� �� ���� �����
    GameObject boardParent;

    //����� ����� �� ��� ����� �� ����
    public float boardScaleFactor = 1.0f;

    //��� ���� ���� ����� ���� ����
    public float boardScale = 1.0f;

    private void Awake()
    {
        instance = this;
    }



    void Start()
    {
        initializeBoard();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //���� ����� ����� ������ ��� ����� ��� ���
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            //���� �� ��� ����� ��� ��� ����
            if (hit.collider != null && hit.collider.gameObject.GetComponent<candy>())
            {
                //�� ��� ���� ����� ��� �� ���
                if(isProcessingMove)
                {
                    return;
                }

                //���� �� ����� �����
                candy candy = hit.collider.gameObject.GetComponent<candy>();
                
                //���� �� ���� �����
                Debug.Log("����� �� ���� :" + candy.gameObject);

                SelectCandy(candy);
            }
        }
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

        //���� �� �����
        boardScale = scale;

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
                int randomIndex = UnityEngine.Random.Range(0, candyPrefabs.Length);

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

        //���� �� ������
        candyToDestroy.Clear();

        //����� ���� �� ������� ��� �������
        foreach(Node nodeCandy in candyBoard)
        {
            //�� �� �� ������
            if (nodeCandy.candy != null)
            {
                //����� ��� �������
                nodeCandy.candy.GetComponent<candy>().isMatched = false;
            }
        }

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
                            MatchResults superMatchedPotions = SuperMach(matchCandy);

                            //����� �� ������� ������� ������ ������
                            candyToRemove.AddRange(superMatchedPotions.connectedCandy);

                            foreach (candy can in superMatchedPotions.connectedCandy)
                            {
                                can.isMatched = true;
                            }

                            //���� �� �� �� ������ �����
                            hasMatched = true;
                        }
                    }
                }
            }
        }
        
        //����� �� �� ������
        return hasMatched;
    }

    public IEnumerator ProsesTurnOnMatchedBoard(bool _subtractMoves)
    {
        //����� �� �� ������� ����� ��� ������
        foreach (candy candyToRemove1 in candyToRemove)
        {
            candyToRemove1.isMatched = false;
        }

        //���� ������ �� �� �������� �������
        RemoveAndRefill(candyToRemove);

        //���� ����� ����� ������
        GameManager.Instance.ProcessTurn(candyToRemove.Count, _subtractMoves);

        //���� ��� ��� ����� �� �� ���� 
        yield return new WaitForSeconds(0.4f);

        if (CheckBoard())
        {
            StartCoroutine(ProsesTurnOnMatchedBoard(false));
        }
    }


    //��� �� ������� ����� ����
    private void RemoveAndRefill(List<candy> candyToRemove)
    {
        //���� �� ������� ����� �� ����
        foreach (candy candy in candyToRemove)
        {
            // ���� �� �x � y
            int xIndex = candy.xIndex;
            int yIndex = candy.yIndex;

            if (candy != null)
            {
                Destroy(candy.gameObject);
                candyBoard[xIndex, yIndex] = new Node(true, null);
            }
            else
            {
                Debug.LogWarning("������ candy �� ����, �� ���� ����� �� ��������.");
            }

/*            //���� ����
            Destroy(candy.gameObject);

            //���� ��� ������ ����
            candyBoard[xIndex, yIndex] = new Node(true, null);*/
        }

        //����� ������ �� ����
        for (int x = 0; x< width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (candyBoard[x,y].candy == null)
                {
                    //���� ����� �� ������ �����
                    Debug.Log("the location x: " + x + "y:" + y + " is empty, attempting to refil it.");

                    //���� ������ �������
                    RefillCandy(x, y);
                }
            }
        }
    }

    // ���� ����
    private void RefillCandy(int x, int y)
    {
        //����� �y �����
        int yOffset = 1;

        //����� ���� ���� �null ��� ���� ����� �������� 
        while (y +  yOffset < height && candyBoard[x, y + yOffset].candy == null)
        {
            //���� �� �yoffset
            Debug.Log("the candy above is null, but its not att the top of the bord so try again");

            yOffset++;
        }

        //�� ��� ������ �� ���� �� ������ ����

        if (y + yOffset < height && candyBoard[x, y + yOffset].candy != null)
        {
            //��� ���� ������
            candy candyAbove = candyBoard[x, y + yOffset].candy.GetComponent<candy>();

            // ��� ���� ������ ������
            Vector3 targetCandy = new Vector3((x - spacingX) * boardScale, (y - spacingY) * boardScale, candyAbove.transform.position.z);

            //����� �� ������
            Debug.Log("i've found a candy ant it si in: " + x + "," + (y + yOffset) + " ande moved it to: " + x + "," + y);

            //��� ������
            candyAbove.MoveToTarget(targetCandy);

            //����� ����� �����
            candyAbove.setIndicies(x, y);

            // ����� �� ����
            candyBoard[x,y] = candyBoard[x, y + yOffset];

            //����� �� ����� ����� ���� ������ ����
            candyBoard[x, y + yOffset] = new Node(true, null);
        }

        //�� ��� �� ���� �� ���� ��� ���� �����
        if(y + yOffset == height)
        {
            //����� ����� ������
            Debug.Log("i got to the top");

            //���� ����� ������
            SpawnCandyAtTop(x);
        }
    }

    //���� ����� ������ �� ����
    private void SpawnCandyAtTop(int x)
    {
        //���� �� ������ ��� ���� ��� ����
        int index = FindIndexOfLowestNull(x);

        //���� �� ������ ��� �����
        int locationToMoveTo = height - index;

        //����� �� �������
        Debug.Log("about to spawn a candy,");

        //���� ����� �������
        int randomIndex = UnityEngine.Random.Range(0, candyPrefabs.Length);
        GameObject newCandy = Instantiate(candyPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);

        // ����� �� ����� ��� ������� ���
        newCandy.transform.SetParent(boardParent.transform);
        
        //���� �� ���� ���� ���� ����� �� ����� ��� ��� ���� ���� ���� ����
        newCandy.transform.localScale = new Vector3(
            newCandy.transform.localScale.x * boardScale,
            newCandy.transform.localScale.y * boardScale,
            newCandy.transform.localScale.z);

        // ����� ������ ������ ���� �����
        newCandy.transform.localPosition = new Vector3(
            (x - spacingX) * boardScale,
            (height - spacingY) * boardScale,
            0);
        
        //����� �������
                newCandy.GetComponent<candy>().setIndicies(x, index);

        //���� ���� ���
        candyBoard[x, index] = new Node(true, newCandy);

        //���� ���� �����
        Vector3 targetPosition = new Vector3(newCandy.transform.localPosition.x, newCandy.transform.localPosition.y - (locationToMoveTo * boardScale), newCandy.transform.position.z);
        newCandy.GetComponent<candy>().MoveToTarget(targetPosition);
    }


    //���� �� ��� ���� 
    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = (height - 1); y>= 0; y--)
        {
            if (candyBoard[x, y].candy == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }



    private MatchResults SuperMach(MatchResults matchCandy)
    {
        // ���� �� �� ����� �� ����� ����
        if (matchCandy.direction == MatchDirection.Horizontal || matchCandy.direction == MatchDirection.LongHorizontal)
        {
            //���� �� �� �������� �� ������ 
            foreach (candy candy in matchCandy.connectedCandy)
            {
                //���� ����� ������ ����
                List<candy> extraConnectedCandy = new();

                //���� ����� �����
                CheckDirection(candy, new Vector2Int(0,1), extraConnectedCandy);
                CheckDirection(candy, new Vector2Int(0,-1), extraConnectedCandy);

                //���� �� �� ���� �2 ���� 
                if (extraConnectedCandy.Count >= 2)
                {
                    //����� ����� ��� ���� �2 ���� �� ����� ���� �����
                    Debug.Log("super Horizontal match");
                    
                    //����� �� ������ �������
                    extraConnectedCandy.AddRange(matchCandy.connectedCandy);

                    //����� ������ ����� �� ������ ������
                    return new MatchResults
                    {
                        connectedCandy = extraConnectedCandy,
                        direction = MatchDirection.Super
                    };
                }

            }
            //�� ��� ������ ����� �� ������ ������ ������
            return new MatchResults
            {
                connectedCandy = matchCandy.connectedCandy,
                direction = matchCandy.direction,
            };
        }

        // ���� �� �� ����� �� ����� ����
        else if (matchCandy.direction == MatchDirection.Vertical || matchCandy.direction == MatchDirection.LongVertical)
        {
            //���� �� �� �������� �� ������ 
            foreach (candy candy in matchCandy.connectedCandy)
            {
                //���� ����� ������ ����
                List<candy> extraConnectedCandy = new();

                //���� ����� �����
                CheckDirection(candy, new Vector2Int(1, 0), extraConnectedCandy);
                CheckDirection(candy, new Vector2Int(-1, 0), extraConnectedCandy);

                //���� �� �� ���� �2 ���� 
                if (extraConnectedCandy.Count >= 2)
                {
                    //����� ����� ��� ���� �2 ���� �� ����� ���� �����
                    Debug.Log("super Vertical match");

                    //����� �� ������ �������
                    extraConnectedCandy.AddRange(matchCandy.connectedCandy);

                    //����� ������ ����� �� ������ ������
                    return new MatchResults
                    {
                        connectedCandy = extraConnectedCandy,
                        direction = MatchDirection.Super
                    };
                }

            }
            //�� ��� ������ ����� �� ������ ������ ������
            return new MatchResults
            {
                connectedCandy = matchCandy.connectedCandy,
                direction = matchCandy.direction,
            };
        }

        //�� ��� ���� ���� �� ����� ����
        return null;
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

    // ���� ����

    public void SelectCandy(candy _candy)
    {
        // �� ��� �� ���� ������ �� ���� ���
        if (selectedCandy == null)
        {
            Debug.Log(_candy);
            selectedCandy = _candy;
        }
        //�� ��� ���� ����� ��� �� ������ ��� 
        else if (selectedCandy == _candy)
        {
            selectedCandy = null;
        }

        //�� �� ���� ��� �� �����
        else if (selectedCandy != _candy)
        {
            SwapCandy(selectedCandy, _candy);
            selectedCandy = null;
        }
    }

    //����� �� �����
    private void SwapCandy(candy _candy1, candy _candy2)
    {
        //�� �� �� ��� ��� ����
        if (!IsAdjacent(_candy1, _candy2))
        {
            return;
        }

        // ���� �����
        DoSwap(_candy1, _candy2);

        //����� ������ ���� ��� ����� ���� ���� ��� ���
        isProcessingMove = true;

        //����� �������� ������ ������
        StartCoroutine(ProcessMatches(_candy1, _candy2));
    }

    //����� �����
    private void DoSwap(candy _candy1, candy _candy2)
    {

        //���� �� ������
        GameObject temp = candyBoard[_candy1.xIndex, _candy1.yIndex].candy;

        // ����� ����� ����
        candyBoard[_candy1.xIndex, _candy1.yIndex].candy = candyBoard[_candy2.xIndex, _candy2.yIndex].candy;

        //����� ��� ������ �� �����
        candyBoard[_candy2.xIndex, _candy2.yIndex].candy = temp;

        //����� �������
        //���� ����� �����
        int tempXIndex =_candy1.xIndex;
        int tempYIndex = _candy1.yIndex;

        //����� ����� ����
        _candy1.xIndex = _candy2.xIndex;
        _candy1.yIndex = _candy2.yIndex;

        //����� ��� ����� �� ������
        _candy2.xIndex = tempXIndex;
        _candy2.yIndex = tempYIndex;

        // ���� �� �������� �� ��� ����
        Vector3 pos1 = new Vector3(
            (_candy1.xIndex - spacingX) * boardScale,
            (_candy1.yIndex - spacingY) * boardScale,
            _candy1.transform.position.z
        );

        // ���� �� �������� �� ��� ����
        Vector3 pos2 = new Vector3(
            (_candy2.xIndex - spacingX) * boardScale,
            (_candy2.yIndex - spacingY) * boardScale,
            _candy2.transform.position.z
        );

        // ���� �� ������ ����� �������� ������ �� ������� ���� ����
        _candy1.MoveToTarget(pos1);
        _candy2.MoveToTarget(pos2);
    }

    private IEnumerator ProcessMatches(candy _candy1, candy _candy2)
    {
        //���� ������� ������ �� ��� ���� ������ ����� ����� ���� ����� �� �� �� 
        yield return new WaitForSeconds(0.2f);
        
        //���� �� �� �����
        if (CheckBoard())
        {
            //����� �������� ����� �������
            StartCoroutine(ProsesTurnOnMatchedBoard(true));
        }

        //�� ��� ����� �� �� ����� �����
        else
        {
            DoSwap(_candy1, _candy2);
        }

        //����� ����� ���� ����� ���� ��� ���
        isProcessingMove = false;
    }

    //���� �� �� ��� ��� ����
    private bool IsAdjacent(candy _candy1, candy _candy2)
    {
        return Mathf.Abs(_candy1.xIndex - _candy2.xIndex) + Mathf.Abs(_candy1.yIndex - _candy2.yIndex) == 1;
    }

    //���� �������




    // ��� ��� ���� ������ ���� ����� �����
    /*    private void DestroyCandy()
        {
            if (candyToDestroy != null)
            {
                foreach (GameObject candy in candyToDestroy)
                {
                    Destroy(candy);
                }
                candyToDestroy.Clear();
            }
        }*/
}




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