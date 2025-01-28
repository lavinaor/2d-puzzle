using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyBoard : MonoBehaviour
{

    //מגדיר את גודל הלוח
    public int width = 6;
    public int height = 8;

    // מגדיר רווחים ללוח
    public float spacingX;
    public float spacingY;

    //לקבל את הפריפבים של הממתקים
    public GameObject[] candyPrefabs;

    //לקבל את הלוח ואת האובייקט
    private Node[,] candyBoard;
    public GameObject candyBoardGO;

    // ליצור פבליק לבורד
    public static CandyBoard instance;

    //אובייקט שמאכלס את כל הלוח בתוכו
    GameObject boardParent;

    //משתנה לשלוט על קנה המידה של הלוח
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
        // גודל לוח בפיקסלים
        float boardWidth = width;
        float boardHeight = height;

        // יחס למסך
        float screenWidth = Camera.main.orthographicSize * 2 * Screen.width / Screen.height;
        float screenHeight = Camera.main.orthographicSize * 2;

        // חישוב קנה המידה המתאים
        float scaleX = screenWidth / boardWidth;
        float scaleY = screenHeight / boardHeight;
        float scale = Mathf.Min(scaleX, scaleY);

        // התאם את קנה המידה לפי המשתנה boardScaleFactor
        scale *= boardScaleFactor;

        // שינוי קנה מידה של הלוח
        boardParent.transform.localScale = new Vector3(scale, scale, 1);
    }


    void initializeBoard()
    {
        candyBoard = new Node[width, height];

        // יצירת אובייקט אב ללוח
        boardParent = new GameObject("BoardParent");

        spacingX = (float)(width - 1) / 2;
        spacingY = (float)(height - 1) / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //מחשב מיקום
                Vector2 pos = new Vector2(x - spacingX, y - spacingY);

                //נותן מספר רנדומלי של איזה צבע ממתק
                int randomIndex = Random.Range(0, candyPrefabs.Length);

                //מיצר ממתק מהסוג הרנדומלי במיקום הנוככי ומגדיר אותו בקוד שלו
                GameObject candy =  Instantiate(candyPrefabs[randomIndex], pos, Quaternion.identity);

                // להציב את הממתק תחת אובייקט האב
                candy.transform.SetParent(boardParent.transform);

                //מגדיר אותו 
                candy.GetComponent<candy>().setIndicies(x, y);
                // מוסיף אותו למערך
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
        //הודעה על תחילת הבדיקה
        Debug.Log("checking board");

        // בול שמוחזר מתחיל כלא נכון ומשתנה במידה של התאמה
        bool hasMatched = false;

        // רשימה של ממתקים שצריך למחוק כי הם התאמה
        List<candy> candyToRemove = new();


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) 
            { 
                //בודק אם המיקום פעיל
                if (candyBoard[x, y].isUsabal)
                {
                    //בודק את סוג הממתק
                    candy candy = candyBoard[x, y].candy.GetComponent<candy>();

                    //בודק אם כבר מותאם
                    if (!candy.isMatched)
                    {
                        MatchResults matchCandy = IsConnected(candy);

                        if (matchCandy.connectedCandy.Count >= 3)
                        {
                            //מקום להכניס שילובים מרובים

                            //מוסיף את הממתקים הרציפים לרשימה למחיקה
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

    //בדיקה אם מחובר
    MatchResults IsConnected(candy candy)
    {
        List<candy> connectedCandy = new();
        candyType candyType = candy.candyType;

        connectedCandy.Add(candy);

        //בודק ימינה
        CheckDirection(candy, new Vector2Int(1,0), connectedCandy);

        //בודק שמאלה
        CheckDirection(candy, new Vector2Int(-1, 0), connectedCandy);

        // בודק אם יש 3 לרוחב
        if (connectedCandy.Count == 3)
        {
            // מודיע על התאמה של 3
            Debug.Log("has mached Horizontal 3 frome type: " + connectedCandy[0].candyType);

            return new MatchResults
            {
                connectedCandy = connectedCandy,
                direction = MatchDirection.Horizontal,
            };
        }

        // בודק אם יש יותר מ3 לרוחב
        else if (connectedCandy.Count > 3)
        {
            // מודיע על התאמה של 3
            Debug.Log("has Horizontal mached more then 3 frome type: " + connectedCandy[0].candyType);

            return new MatchResults
            {
                connectedCandy = connectedCandy,
                direction = MatchDirection.LongHorizontal,
            };
        }

        // אם אין התאמה מנקה את הרשימה
        connectedCandy.Clear();

        //מחזיר את הראשוני
        connectedCandy.Add(candy);

        // בוקד למעלה
        CheckDirection(candy, new Vector2Int(0, 1), connectedCandy);

        //בודק למטה
        CheckDirection(candy, new Vector2Int(0, -1), connectedCandy);

        // בודק אם יש 3 לגובה
        if (connectedCandy.Count == 3)
        {
            // מודיע על התאמה של 3
            Debug.Log("has Vertical mached 3 frome type: " + connectedCandy[0].candyType);

            return new MatchResults
            {
                connectedCandy = connectedCandy,
                direction = MatchDirection.Vertical,
            };
        }

        // בודק אם יש יותר מ3 לגובה
        else if (connectedCandy.Count > 3)
        {
            // מודיע על התאמה של 3
            Debug.Log("has Vertical mached more then 3 frome type: " + connectedCandy[0].candyType);

            return new MatchResults
            {
                connectedCandy = connectedCandy,
                direction = MatchDirection.LongVertical,
            };
        }

        //אם עדין אין הטעמה
        else
        {
            return new MatchResults
            {
                connectedCandy = connectedCandy,
                direction = MatchDirection.None,
            };
        }
    }

    //בדוק כיוון
    void CheckDirection(candy candy, Vector2Int direction, List<candy> connectedCandy)
    {
        candyType candyType = candy.candyType;
        int x = candy.xIndex + direction.x;
        int y = candy.yIndex + direction.y;

        // בודק שזה בתוך הלוח
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (candyBoard[x, y].isUsabal)
            {
                 candy neighbourCandy = candyBoard[x, y].candy.GetComponent<candy>();

                // האם הסוג ממתק זהה
                if (!neighbourCandy.isMatched && neighbourCandy.candyType == candyType)
                {
                    // מתאים אז מוסיף אותו לרשימה 
                    connectedCandy.Add(neighbourCandy);

                    // בודק עוד באותו כיוון
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


// בוחר ממתק

//מחליף את הממתק

//מחליף בפועל

//בודק אם יש מחוברים

//נפטר מהתאמות







// כיתה של התווצאה 
public class MatchResults
{
    public List<candy> connectedCandy;
    public MatchDirection direction;
}

// כיוונים אפשריים
public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super,
    None
}