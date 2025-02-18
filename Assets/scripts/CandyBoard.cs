using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
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

    //רשימת ממתקים להריסה
    public List<GameObject> candyToDestroy = new();

    // שיקוי שנבחר אחרון להוזזה
    [SerializeField]
    private candy selectedCandy;

    //האם אני מוזיז כרגע
    [SerializeField]
    private bool isProcessingMove;

    // רשימה של ממתקים שצריך למחוק כי הם התאמה
    [SerializeField]
    List<candy> candyToRemove = new();

    //אובייקט שמאכלס את כל הלוח בתוכו
    GameObject boardParent;

    //משתנה לשלוט על קנה המידה של הלוח
    public float boardScaleFactor = 1.0f;

    //קנה מידה אחרי חישוב יכנס לכאן
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
            //שולח בדיקה לאיפה שהשחקן לחץ ושומר במה פגע
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            //בודק אם פגע במשהו ואם הוא ממתק
            if (hit.collider != null && hit.collider.gameObject.GetComponent<candy>())
            {
                //אם הוא עדין במהלך אחר אז בטל
                if(isProcessingMove)
                {
                    return;
                }

                //שומר את הממתק שנבחר
                candy candy = hit.collider.gameObject.GetComponent<candy>();
                
                //רושם על ממתק שנבחר
                Debug.Log("לחצתי על ממתק :" + candy.gameObject);

                SelectCandy(candy);
            }
        }
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

        //שומר את הגודל
        boardScale = scale;

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
                int randomIndex = UnityEngine.Random.Range(0, candyPrefabs.Length);

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

        //מנקה את הרשימה
        candyToDestroy.Clear();

        //מגדיר מחדש את הממתקים כלא מותאמים
        foreach(Node nodeCandy in candyBoard)
        {
            //רק אם הם קיימים
            if (nodeCandy.candy != null)
            {
                //מגדיר כלא מותאמים
                nodeCandy.candy.GetComponent<candy>().isMatched = false;
            }
        }

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
                            MatchResults superMatchedPotions = SuperMach(matchCandy);

                            //מוסיף את הממתקים הרציפים לרשימה למחיקה
                            candyToRemove.AddRange(superMatchedPotions.connectedCandy);

                            foreach (candy can in superMatchedPotions.connectedCandy)
                            {
                                can.isMatched = true;
                            }

                            //הופך את אם יש התאמות לנכון
                            hasMatched = true;
                        }
                    }
                }
            }
        }
        
        //מחזיר אם יש התאמות
        return hasMatched;
    }

    public IEnumerator ProsesTurnOnMatchedBoard(bool _subtractMoves)
    {
        //מגדיר את כל הממתקים להוצא כלא בהתאמה
        foreach (candy candyToRemove1 in candyToRemove)
        {
            candyToRemove1.isMatched = false;
        }

        //מנקה ומחליף את כל השיקויים הנכונים
        RemoveAndRefill(candyToRemove);

        //משנה ניקוד וכמות מהלכים
        GameManager.Instance.ProcessTurn(candyToRemove.Count, _subtractMoves);

        //מחכה קצת כדי שיראה את זה בלוח 
        yield return new WaitForSeconds(0.4f);

        if (CheckBoard())
        {
            StartCoroutine(ProsesTurnOnMatchedBoard(false));
        }
    }


    //מחק את הנכונים ותמלא מחדש
    private void RemoveAndRefill(List<candy> candyToRemove)
    {
        //מוחק את הממתקים ומפנה את הלוח
        foreach (candy candy in candyToRemove)
        {
            // שומר את הx ו y
            int xIndex = candy.xIndex;
            int yIndex = candy.yIndex;

            if (candy != null)
            {
                Destroy(candy.gameObject);
                candyBoard[xIndex, yIndex] = new Node(true, null);
            }
            else
            {
                Debug.LogWarning("המשתנה candy לא קיים, לא ניתן למחוק את האובייקט.");
            }

/*            //מוחק ממתק
            Destroy(candy.gameObject);

            //מיצר ריק בנקודה הזאת
            candyBoard[xIndex, yIndex] = new Node(true, null);*/
        }

        //לולאה שעוברת על הלוח
        for (int x = 0; x< width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (candyBoard[x,y].candy == null)
                {
                    //שולח הודעה על ניסיון מילוי
                    Debug.Log("the location x: " + x + "y:" + y + " is empty, attempting to refil it.");

                    //שולח למילוי הממתקים
                    RefillCandy(x, y);
                }
            }
        }
    }

    // ממלא מחדש
    private void RefillCandy(int x, int y)
    {
        //מוסיף לy תזוזה
        int yOffset = 1;

        //כשהזה שמעל שווה לnull וזה מתחת לגובה המקסימלי 
        while (y +  yOffset < height && candyBoard[x, y + yOffset].candy == null)
        {
            //הגדל את הyoffset
            Debug.Log("the candy above is null, but its not att the top of the bord so try again");

            yOffset++;
        }

        //או שזה הלמעלה של הלוח או שמצאתי ממתק

        if (y + yOffset < height && candyBoard[x, y + yOffset].candy != null)
        {
            //מצא ממתק להוריד
            candy candyAbove = candyBoard[x, y + yOffset].candy.GetComponent<candy>();

            // הזז אותו למיקום הנוכחי
            Vector3 targetCandy = new Vector3((x - spacingX) * boardScale, (y - spacingY) * boardScale, candyAbove.transform.position.z);

            //מדווח על ההוזזה
            Debug.Log("i've found a candy ant it si in: " + x + "," + (y + yOffset) + " ande moved it to: " + x + "," + y);

            //זוז למיקום
            candyAbove.MoveToTarget(targetCandy);

            //מעדכן מיקום אצלהם
            candyAbove.setIndicies(x, y);

            // מעדכן את הלוח
            candyBoard[x,y] = candyBoard[x, y + yOffset];

            //מייצר את המקום שממנו נלקח השיקוי מחדש
            candyBoard[x, y + yOffset] = new Node(true, null);
        }

        //אם מצא את הסוף של הלוח בלי למצו שיקוי
        if(y + yOffset == height)
        {
            //מודיע שהגיע ללמעלה
            Debug.Log("i got to the top");

            //מזמן חדשים בלמעלה
            SpawnCandyAtTop(x);
        }
    }

    //מזמן חדשים בלמעלה של הלוח
    private void SpawnCandyAtTop(int x)
    {
        //מוצא את המיקום הכי נמוך בלי כלום
        int index = FindIndexOfLowestNull(x);

        //מחשב את המיקום לפי הגובה
        int locationToMoveTo = height - index;

        //מדווח על הניסיון
        Debug.Log("about to spawn a candy,");

        //משיג שיקוי רנדומלי
        int randomIndex = UnityEngine.Random.Range(0, candyPrefabs.Length);
        GameObject newCandy = Instantiate(candyPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);

        // להציב את הממתק תחת אובייקט האב
        newCandy.transform.SetParent(boardParent.transform);
        
        //מחשב את הקנה מידה ביחס לגודל של ההורה ככה שזה יהיה בקנה מידה נכון
        newCandy.transform.localScale = new Vector3(
            newCandy.transform.localScale.x * boardScale,
            newCandy.transform.localScale.y * boardScale,
            newCandy.transform.localScale.z);

        // קביעת המיקום המתוקן בתוך ההורה
        newCandy.transform.localPosition = new Vector3(
            (x - spacingX) * boardScale,
            (height - spacingY) * boardScale,
            0);
        
        //דהגדר מיקומים
                newCandy.GetComponent<candy>().setIndicies(x, index);

        //הגדר אותם לוח
        candyBoard[x, index] = new Node(true, newCandy);

        //הוזז אותם למקום
        Vector3 targetPosition = new Vector3(newCandy.transform.localPosition.x, newCandy.transform.localPosition.y - (locationToMoveTo * boardScale), newCandy.transform.position.z);
        newCandy.GetComponent<candy>().MoveToTarget(targetPosition);
    }


    //מוצא את הכי נמוך 
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
        // בודק אם יש לרוחב או לרוחב גדול
        if (matchCandy.direction == MatchDirection.Horizontal || matchCandy.direction == MatchDirection.LongHorizontal)
        {
            //עובר על כל השיקויים של ההתאמה 
            foreach (candy candy in matchCandy.connectedCandy)
            {
                //פותח רשימת תוצאות חדשה
                List<candy> extraConnectedCandy = new();

                //בודק למעלה ולמטה
                CheckDirection(candy, new Vector2Int(0,1), extraConnectedCandy);
                CheckDirection(candy, new Vector2Int(0,-1), extraConnectedCandy);

                //בודק אם יש יותר מ2 ביחד 
                if (extraConnectedCandy.Count >= 2)
                {
                    //מוציא אישור שיש יותר מ2 משמע יש התאמה סופר גדולה
                    Debug.Log("super Horizontal match");
                    
                    //מוסיף את ההתאמה לתוצאות
                    extraConnectedCandy.AddRange(matchCandy.connectedCandy);

                    //מחזיר תוצאות חדשות עם ההטעמה הגדולה
                    return new MatchResults
                    {
                        connectedCandy = extraConnectedCandy,
                        direction = MatchDirection.Super
                    };
                }

            }
            //אם אין הטעמות מחזיר את ההתאמה הרגילה שהייתה
            return new MatchResults
            {
                connectedCandy = matchCandy.connectedCandy,
                direction = matchCandy.direction,
            };
        }

        // בודק אם יש לגובה או לגובה גדול
        else if (matchCandy.direction == MatchDirection.Vertical || matchCandy.direction == MatchDirection.LongVertical)
        {
            //עובר על כל השיקויים של ההתאמה 
            foreach (candy candy in matchCandy.connectedCandy)
            {
                //פותח רשימת תוצאות חדשה
                List<candy> extraConnectedCandy = new();

                //בודק למעלה ולמטה
                CheckDirection(candy, new Vector2Int(1, 0), extraConnectedCandy);
                CheckDirection(candy, new Vector2Int(-1, 0), extraConnectedCandy);

                //בודק אם יש יותר מ2 ביחד 
                if (extraConnectedCandy.Count >= 2)
                {
                    //מוציא אישור שיש יותר מ2 משמע יש התאמה סופר גדולה
                    Debug.Log("super Vertical match");

                    //מוסיף את ההתאמה לתוצאות
                    extraConnectedCandy.AddRange(matchCandy.connectedCandy);

                    //מחזיר תוצאות חדשות עם ההטעמה הגדולה
                    return new MatchResults
                    {
                        connectedCandy = extraConnectedCandy,
                        direction = MatchDirection.Super
                    };
                }

            }
            //אם אין הטעמות מחזיר את ההתאמה הרגילה שהייתה
            return new MatchResults
            {
                connectedCandy = matchCandy.connectedCandy,
                direction = matchCandy.direction,
            };
        }

        //אם עבר משהו שהוא לא תוצאה בכלל
        return null;
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

    // בוחר ממתק

    public void SelectCandy(candy _candy)
    {
        // אם אין לי ממתק בבחירה אז תבחר חדש
        if (selectedCandy == null)
        {
            Debug.Log(_candy);
            selectedCandy = _candy;
        }
        //אם בחר אותו שיקוי בטל את הבחירה שלו 
        else if (selectedCandy == _candy)
        {
            selectedCandy = null;
        }

        //אם לא אותו דבר אז מחליף
        else if (selectedCandy != _candy)
        {
            SwapCandy(selectedCandy, _candy);
            selectedCandy = null;
        }
    }

    //מחליף את הממתק
    private void SwapCandy(candy _candy1, candy _candy2)
    {
        //אם זה לא אחד ליד השני
        if (!IsAdjacent(_candy1, _candy2))
        {
            return;
        }

        // התחל החלפה
        DoSwap(_candy1, _candy2);

        //מעדכן שהתחיל מהלך ולא אפשרי לבצע כרגע עוד אחד
        isProcessingMove = true;

        //מתחיל כורוטינה למציאת התאמות
        StartCoroutine(ProcessMatches(_candy1, _candy2));
    }

    //מחליף בפועל
    private void DoSwap(candy _candy1, candy _candy2)
    {

        //שומר את הראשון
        GameObject temp = candyBoard[_candy1.xIndex, _candy1.yIndex].candy;

        // מחליף ראשון בשני
        candyBoard[_candy1.xIndex, _candy1.yIndex].candy = candyBoard[_candy2.xIndex, _candy2.yIndex].candy;

        //מחליף שני בשמירה של ראשון
        candyBoard[_candy2.xIndex, _candy2.yIndex].candy = temp;

        //מעדכן מיקומים
        //שומר מיקום זמנית
        int tempXIndex =_candy1.xIndex;
        int tempYIndex = _candy1.yIndex;

        //מחליף ראשון בשני
        _candy1.xIndex = _candy2.xIndex;
        _candy1.yIndex = _candy2.yIndex;

        //מחליף שני בקופי של הראשון
        _candy2.xIndex = tempXIndex;
        _candy2.yIndex = tempYIndex;

        // מחשב את המיקומים עם קנה מידה
        Vector3 pos1 = new Vector3(
            (_candy1.xIndex - spacingX) * boardScale,
            (_candy1.yIndex - spacingY) * boardScale,
            _candy1.transform.position.z
        );

        // מחשב את המיקומים עם קנה מידה
        Vector3 pos2 = new Vector3(
            (_candy2.xIndex - spacingX) * boardScale,
            (_candy2.yIndex - spacingY) * boardScale,
            _candy2.transform.position.z
        );

        // מזיז את הראשון והשני למיקומים החדשים עם התחשבות בקנה מידה
        _candy1.MoveToTarget(pos1);
        _candy2.MoveToTarget(pos2);
    }

    private IEnumerator ProcessMatches(candy _candy1, candy _candy2)
    {
        //מחכה שההחלפה תסתיים אם אני רוצה להוסיף זמנים שונים צריך לפתור פה את זה 
        yield return new WaitForSeconds(0.2f);
        
        //בודק אם יש התאמה
        if (CheckBoard())
        {
            //מתחיל קורוטינה שתטפל בתוצאות
            StartCoroutine(ProsesTurnOnMatchedBoard(true));
        }

        //אם אין התאמה אז הם יחזרו לאחור
        else
        {
            DoSwap(_candy1, _candy2);
        }

        //מעדכן שסיים מהלך ואפשר לבצע עוד אחד
        isProcessingMove = false;
    }

    //בודק אם הם אחד ליד השני
    private bool IsAdjacent(candy _candy1, candy _candy2)
    {
        return Mathf.Abs(_candy1.xIndex - _candy2.xIndex) + Mathf.Abs(_candy1.yIndex - _candy2.yIndex) == 1;
    }

    //נפטר מהתאמות




    // קוד ישן שמור לממקרה שאני אצטרך בעתיד
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