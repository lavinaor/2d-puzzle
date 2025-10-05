using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.VFX;
using DG.Tweening; // לוודא שהוספת DOTween למעלה


public class CandyBoard : MonoBehaviour
{
    // קוד לבדיקת אופציה של  הכנת מפות עם טילמפ
    public bool hasTilmap = false;
    public Tilemap tilemap; // ה-Tilemap המקורי
    public Tilemap backgroundTilemap;  // ה-reference ל-Tilemap של הרקע
    public TileBase backgroundTile;    // האריח עצמו – מים או שחור

    //מצלמה של הסצנה
    public Camera cam;

    //מגדיר את גודל הלוח
    public int width = 6;
    public int height = 8;

    // מגדיר רווחים ללוח
    public float spacingX;
    public float spacingY;

    //לקבל את הפריפבים של הממתקים
    public GameObject[] candyPrefabs;

    //לקבל את הפריפבים של הממתקים
    public GameObject[] specialCandyPrefabs;

    //לקבל את הלוח ואת האובייקט
    private Node[,] candyBoard;
    public GameObject candyBoardGO;

    // ליצור פבליק לבורד
    public static CandyBoard instance;


    // שיקוי שנבחר אחרון להוזזה
    [SerializeField]
    private candy selectedCandy;
    private Vector2Int lastTouchedCandyPos = new Vector2Int(-1, -1);


    //האם אני מוזיז כרגע
    [SerializeField]
    private bool isProcessingMove;

    // רשימה של ממתקים שצריך למחוק כי הם התאמה
    [SerializeField]
    List<candy> candyToRemove = new();

    // רשימה של תוצאות שצריך למחוק כי הם התאמה
    [SerializeField]
    List<MatchResults> lastMatchResults = new();

    [SerializeField]
    private float delayBetweenMatches = 0.4f;

    //אובייקט שמאכלס את כל הלוח בתוכו
    GameObject boardParent;

    //משתנה לשלוט על קנה המידה של הלוח
    public float boardScaleFactor = 1.0f;
    //משתנה לשלוט על המרכז של הלוח של הלוח
    public Vector2 boardOffsetFactor = new Vector2(0f, 0f);

    // אבא של כל התאורה
    public GameObject lightParent;

    //קנה מידה אחרי חישוב יכנס לכאן
    public float boardScale = 1.0f;

    //אפקט לאורך
    [SerializeField]
    private GameObject VFXVerticalPrefab;
    private VisualEffect VFXVertical;

    //אפקט לרוחב
    [SerializeField]
    private GameObject VFXHorizontalPrefab;
    private VisualEffect VFXHorizontal;

    //סופר קורוטינות פעילות
    private int activeCoroutines = 0;

    //רשימה של ממתקים לניקוד
    List<candy> candyForScoring = new ();

    private void Awake()
    {
        instance = this;
    }



    void Start()
    {
        if (hasTilmap)
            ConvertTilemapToGameBoard();
        else
            initializeBoard();
    }

/*    private void Update()
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
    }*/

/*    void Update()
    {
        Vector2 inputPosition = Vector2.zero;
        bool inputDetected = false;

        // בדיקה למובייל
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            inputPosition = Input.GetTouch(0).position;
            inputDetected = true;
        }

        // בדיקה למחשב
        if (Input.GetMouseButtonDown(0))
        {
            inputPosition = Input.mousePosition;
            inputDetected = true;
        }

        if (inputDetected)
        {

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(inputPosition);
            Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<candy>())
            {
                Debug.Log("פגע באובייקט: " + hit.collider.name);

                //אם הוא עדין במהלך אחר אז בטל
                if (isProcessingMove)
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
    }*/

    private candy draggedCandy = null;
    private Vector2 dragStartPos;
    private bool isDragging = false;

    void Update()
    {
        Vector2 inputPosition = Vector2.zero;

        // אם הוא עדיין במהלך אחר אז בטל
        if (isProcessingMove || Time.timeScale == 0f)
        {
            return;
        }

        // בדיקה למובייל
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPosition = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TryStartDrag(inputPosition);
                    break;
                case TouchPhase.Moved:
                    Drag(inputPosition);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    EndDrag();
                    break;
            }
        }

        // בדיקה למחשב
        else
        {
            inputPosition = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                TryStartDrag(inputPosition);
            }
            else if (Input.GetMouseButton(0))
            {
                Drag(inputPosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }
    }

    // התחלת גרירה
    void TryStartDrag(Vector2 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.GetComponent<candy>())
        {
            candy c = hit.collider.gameObject.GetComponent<candy>();
            StartDrag(c);
        }
    }

    void StartDrag(candy c)
    {
        draggedCandy = c;
        dragStartPos = GetCandyPosition(c);

        // אפקט הגדלה
        draggedCandy.transform.localScale = Vector3.one * 1.2f;

        // נצנוץ
        StartCoroutine(CandyTwinkle(draggedCandy));
    }

    Vector2 GetCandyPosition(candy c)
    {
        return c.transform.position;
    }

    candy GetAdjacentCandy(candy origin, Vector2 direction)
    {
        if (origin == null)
        {
            Debug.LogWarning("GetAdjacentCandy called with null origin!");
            return null;
        }

        int x = origin.xIndex + (int)direction.x;
        int y = origin.yIndex + (int)direction.y;

        if (x >= 0 && x < width && y >= 0 && y < height && candyBoard[x, y].isUsabal == true)
        {
            return candyBoard[x, y].candy.GetComponent<candy>();
        }

        return null;
    }

    IEnumerator CandyTwinkle(candy c)
    {
        SpriteRenderer sr = c.GetComponent<SpriteRenderer>();
        float duration = 0.3f;
        float elapsed = 0f;
        Color originalColor = sr.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.PingPong(elapsed * 5, 1f);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0.7f, 1f, alpha));
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = originalColor;
    }


    void Drag(Vector2 screenPos)
    {
        if (draggedCandy == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector2 dragVector = (Vector2)worldPos - dragStartPos;

        if (dragVector.magnitude > 0.3f)
        {
            Vector2 dir = Vector2.zero;

            if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
                dir = dragVector.x > 0 ? Vector2.right : Vector2.left;
            else
                dir = dragVector.y > 0 ? Vector2.up : Vector2.down;

            candy targetCandy = GetAdjacentCandy(draggedCandy, dir);

            if (targetCandy != null)
            {
                SelectCandy(draggedCandy); // כמו לחיצה ראשונה
                SelectCandy(targetCandy);  // כמו לחיצה שניה
            }

            EndDrag(); // סיום הגרירה אחרי החלפה
        }
    }

    void EndDrag()
    {
        if (draggedCandy != null)
        {
            // החזרת גודל רגיל
            draggedCandy.transform.localScale = Vector3.one;
        }

        draggedCandy = null;
    }

    void ScaleBoardToFitScreen()
    {

        // שמור את הערכים הראשוניים של המצלמה והתאורה
        float initialOrthoSize = Camera.main.orthographicSize;

        // גודל הלוח בפיקסלים
        float boardWidth = width;
        float boardHeight = height;

        // יחס מסך
        float aspectRatio = (float)Screen.width / Screen.height;

        // חישוב קנה המידה המתאים
        float scaleX = boardWidth / (2 * aspectRatio);
        float scaleY = boardHeight / 2;
        float newOrthoSize = Mathf.Max(scaleX, scaleY);

        // התאם את הגודל לפי boardScaleFactor (אם צריך שוליים נוספים)
        newOrthoSize *= boardScaleFactor;

        // הגדר את גודל המצלמה החדש
        Camera.main.orthographicSize = newOrthoSize;

        // הזז את המצלמה בהתאם ל־offset שהוזן, על בסיס הגודל החדש
        Vector3 cameraPosition = Camera.main.transform.position;

        // חשב את רוחב וגובה העולם (ביחידות Unity) שהמצלמה רואה
        float cameraHeight = Camera.main.orthographicSize * 2f;
        float cameraWidth = cameraHeight * aspectRatio;

        // יישם את הסטייה על מיקום המצלמה
        cameraPosition.x = boardOffsetFactor.x * cameraWidth / 2f;
        cameraPosition.y = boardOffsetFactor.y * cameraHeight / 2f;

        // שמור את מיקום המצלמה החדש
        Camera.main.transform.position = cameraPosition;

        // חישוב יחס השינוי במצלמה
        float scaleFactor = newOrthoSize / initialOrthoSize;

        //מוודא שיש אובייקט כזה
        if (lightParent != null)
        {
            // התאם את קנה המידה של האובייקט שמכיל את התאורה
            lightParent.transform.localScale *= scaleFactor;
        }

    }

    void ConvertTilemapToGameBoard()
    {
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        int boardWidth = bounds.size.x;
        int boardHeight = bounds.size.y;

        int offsetX = bounds.xMin;
        int offsetY = bounds.yMin;

        width = boardWidth;
        height = boardHeight;

        spacingX = (float)(width - 1) / 2;
        spacingY = (float)(height - 1) / 2;

        CreateBackgroundTiles(bounds);
        CenterBackgroundTilemap(bounds);

        candyBoard = new Node[width, height];
        boardParent = new GameObject("BoardParent");

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                int arrayX = x - offsetX;
                int arrayY = y - offsetY;

                if (tile != null)
                {
                    Debug.Log(tile.name);

                    bool isSpecial = false;
                    int prefabIndex = GetPrefabIndex(tile);
                    if (prefabIndex == -1)
                    {
                        isSpecial = true;
                        prefabIndex = GetSpecialPrefabIndex(tile);
                    }

                    if (prefabIndex != -1)
                    {
                        Vector2 pos = new Vector2(arrayX - spacingX, arrayY - spacingY);

                        GameObject prefab = !isSpecial
                            ? candyPrefabs[prefabIndex]
                            : specialCandyPrefabs[prefabIndex];

                        GameObject newTile = Instantiate(prefab, pos, Quaternion.identity);
                        newTile.transform.parent = boardParent.transform;

                        newTile.GetComponent<candy>().setIndicies(arrayX, arrayY);
                        candyBoard[arrayX, arrayY] = new Node(true, newTile);
                    }
                }
                else
                {
                    candyBoard[arrayX, arrayY] = new Node(false, null);
                    Debug.Log(" is null");
                }
            }
        }

        if (CheckBoard())
        {
            OnTilmapRandomMaches();
        }

        Destroy(tilemap.gameObject);
        ScaleBoardToFitScreen();
    }


    void CreateBackgroundTiles(BoundsInt bounds)
    {
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // רק אם יש tile בלוח המשחק
                if (tilemap.HasTile(pos))
                {
                    backgroundTilemap.SetTile(pos, backgroundTile);
                }
            }
        }
    }

    void CenterBackgroundTilemap(BoundsInt bounds)
    {
        if (backgroundTilemap != null)
        {
            // חישוב מרכז התאים של ה־tilemap בדיוק (כולל חצאים)
            Vector3 centerCell = new Vector3(
                bounds.xMin + bounds.size.x / 2f,
                bounds.yMin + bounds.size.y / 2f,
                0);

            // מקבל את המרכז במרחב המקומי המדויק (כולל שברים)
            Vector3 centerLocal = backgroundTilemap.layoutGrid.CellToLocalInterpolated(centerCell);

            // המרה למיקום עולמי לפי הטיילמפ
            Vector3 centerWorld = backgroundTilemap.transform.TransformPoint(centerLocal);

            // הזזת הטיילמפ כך שמרכזו יישב על (0,0)
            backgroundTilemap.transform.position = -centerWorld;
        }
    }


    void OnTilmapRandomMaches()
    {
        foreach (candy candyToRemove in candyToRemove)
        {
            // שומר את הx ו y
            int xIndex = candyToRemove.xIndex;
            int yIndex = candyToRemove.yIndex;

            if (candyToRemove != null)
            {
                Destroy(candyToRemove.gameObject);

                //משיג שיקוי רנדומלי
                int randomIndex = UnityEngine.Random.Range(0, candyPrefabs.Length);
                GameObject newCandy = Instantiate(candyPrefabs[randomIndex], new Vector2(xIndex - spacingX, yIndex - spacingY), Quaternion.identity);

                // להציב את הממתק תחת אובייקט האב
                newCandy.transform.SetParent(boardParent.transform);


                //דהגדר מיקומים
                newCandy.GetComponent<candy>().setIndicies(xIndex, yIndex);

                //הגדר אותם לוח
                candyBoard[xIndex, yIndex] = new Node(true, newCandy);
            }
        }
        if (CheckBoard())
        {
            OnTilmapRandomMaches();
        }
    }

    int GetPrefabIndex(TileBase tile)
    {
        for (int i = 0; i < candyPrefabs.Length; i++)
        {
            if (candyPrefabs[i].name == tile.name) // הנחה: שם הפריפב זהה לשם האריח
            {
                return i;
            }
            else if("random" == tile.name)
            {
                return UnityEngine.Random.Range(0, candyPrefabs.Length);
            }
        }
        return -1;
    }
    int GetSpecialPrefabIndex(TileBase tile)
    {
        for (int i = 0; i < specialCandyPrefabs.Length; i++)
        {
            if (specialCandyPrefabs[i].name == tile.name) // הנחה: שם הפריפב זהה לשם האריח
            {
                return i;
            }
        }
        return -1;
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
            Debug.Log("ther are maches proses the bord");

            ScaleBoardToFitScreen();

            //מתחיל קורוטינה שתטפל בתוצאות
            StartCoroutine(ProsesTurnOnMatchedBoard(true, 0f));
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

        //מנקה את הרשימות
        candyToRemove.Clear();
        lastMatchResults.Clear();

        //מגדיר מחדש את הממתקים כלא מותאמים
        foreach (Node nodeCandy in candyBoard)
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
                            //שילובים מרובים
                            MatchResults superMatchedCandys = SuperMach(matchCandy);

                            //מוסיף את המתאים ביותר
                            if (matchCandy != superMatchedCandys)
                                lastMatchResults.Add(superMatchedCandys);
                            else
                                lastMatchResults.Add(matchCandy);

                            //מוסיף את הממתקים הרציפים לרשימה למחיקה
                            candyToRemove.AddRange(superMatchedCandys.connectedCandy);

                            foreach (candy can in superMatchedCandys.connectedCandy)
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

    public IEnumerator ProsesTurnOnMatchedBoard(bool _subtractMoves, float deley)
    {
        if (candyForScoring.Count > 0)
        {
            //משנה ניקוד וכמות מהלכים
            GameManager.Instance.ProcessTurn(candyForScoring, false);
            candyForScoring.Clear();
        }

        //רשימה של האפקטים
        List<GameObject> efects = new List<GameObject>();

        //מגדיר את כל הממתקים להוצא כלא בהתאמה
        foreach (candy candyToRemove1 in candyToRemove)
        {
            if (candyToRemove1 != null)
                efects.Add(candyToRemove1.OnDestroyVFX());
            candyToRemove1.isMatched = false;
        }

        Remove(candyToRemove);

        //מחכה קצת כדי שיראה את זה בלוח 
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject efect in efects)
        {
            Destroy(efect);
        }

        isSpecial();
        Refill();

        //משנה ניקוד וכמות מהלכים
        GameManager.Instance.ProcessTurn(candyToRemove, _subtractMoves);

        //מחכה קצת כדי שיראה את זה בלוח 
        yield return new WaitForSeconds(deley);

        if (CheckBoard())
        {
            StartCoroutine(ProsesTurnOnMatchedBoard(false, deley));
        }
        else
        {
            //מעדכן שסיים מהלך ואפשר לבצע עוד אחד
            isProcessingMove = false;
            Debug.Log("end");
        }
    }

    private void Remove(List<candy> candyToRemove)
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
            Debug.Log("1111111111");
        }
    }

    private void isSpecial()
    {
        // בודק האם יש התאמות מיוחדות
        var specialMatches = lastMatchResults.Where(r =>
            r.direction == MatchDirection.LongVertical ||
            r.direction == MatchDirection.LongHorizontal ||
            r.direction == MatchDirection.Super).ToList();

        if (specialMatches.Count > 0)
        {
            Debug.Log("נמצאו " + specialMatches.Count + " התאמות מיוחדות!");

            foreach (var match in specialMatches)
            {
                // בוחר את המקום הטוב ביותר עבור כל התאמה מיוחדת
                Vector2Int bestCandy = GetBestPositionForSpecialCandy(match);

                Debug.Log("מיקום הממתק המיוחד שנמצא: " + bestCandy.x + "  " + bestCandy.y); // בודק את המיקום

                if (bestCandy != null)
                {
                    // בוחר את המקום הטוב ביותר עבור כל התאמה מיוחדת
                    int prefabIndex = GetSpecialPrefabIndex(match);

                    //מזמן את הממתק המיוחד
                    Vector3 newPosishen = new Vector3((bestCandy.x - spacingX) * boardScale, (bestCandy.y - spacingY) * boardScale, 0);
/*                    //GameObject newCandy = Instantiate(specialCandyPrefabs[prefabIndex], newPosishen, Quaternion.identity);*/

                    GameObject newCandy = Instantiate(
                        specialCandyPrefabs[prefabIndex],
                        newPosishen,
                        Quaternion.identity
                        );

                    newCandy.transform.SetParent(boardParent.transform);

                    // הגדרת אינדקסים
                    newCandy.GetComponent<candy>().setIndicies(bestCandy.x, bestCandy.y);
                    candyBoard[bestCandy.x, bestCandy.y] = new Node(true, newCandy);

                    // 🎨 אפקט DOTween
                    newCandy.transform.localScale = Vector3.zero;
                    newCandy.transform
                        .DOScale(1.2f, 0.3f) // גדל לגודל 120% ב־0.3 שניות
                        .SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            newCandy.transform.DOScale(1f, 0.2f); // חוזר לגודל רגיל
                        });

                    // להציב את הממתק תחת אובייקט האב
                    newCandy.transform.SetParent(boardParent.transform);

                    //דהגדר מיקומים
                    newCandy.GetComponent<candy>().setIndicies((int)bestCandy.x, (int)bestCandy.y);

                    //הגדר אותם לוח
                    candyBoard[bestCandy.x, bestCandy.y] = new Node(true, newCandy);
                }
            }
        }
    }

    private void Refill()
    {
        //לולאה שעוברת על הלוח וממלאת אותו
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (candyBoard[x, y].candy == null && candyBoard[x, y].isUsabal == true)
                {
                    //שולח הודעה על ניסיון מילוי
                    Debug.Log("the location x: " + x + "y:" + y + " is empty, attempting to refil it.");

                    //שולח למילוי הממתקים
                    RefillCandy(x, y);
                }
            }
        }
    }

    private Vector2Int GetBestPositionForSpecialCandy(MatchResults matchResults)
    {
        if (matchResults.connectedCandy.Count == 0)
            return Vector2Int.zero;

        // שלב 1: מוצא את המינימום Y
        int minY = matchResults.connectedCandy.Min(c => c.yIndex);

        // שלב 2: מסנן רק ממתקים בגובה הזה
        var filtered = matchResults.connectedCandy
            .Where(c => c.yIndex == minY)
            .ToList();

        // שלב 3: אם יש יותר מאחד, בוחר את הקרוב ביותר למיקום האחרון של השחקן
        if (filtered.Count > 1 && lastTouchedCandyPos.x >= 0)
        {
            filtered = filtered
                .OrderBy(c => Vector2Int.Distance(
                    new Vector2Int(c.xIndex, c.yIndex),
                    lastTouchedCandyPos
                ))
                .ToList();
        }

        return new Vector2Int(filtered[0].xIndex, filtered[0].yIndex);
    }


    private int GetSpecialPrefabIndex(MatchResults matchResults)
    {
        switch (matchResults.direction)
        {
            case MatchDirection.LongVertical:
                return 0;
            case MatchDirection.LongHorizontal:
                return 1;
            case MatchDirection.Super:
                return 2;  
        }
        return 3;
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
            if (candyBoard[x, y].candy == null && candyBoard[x, y].isUsabal)
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
            return matchCandy;
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
            return matchCandy;
        }

        //אם עבר משהו שהוא לא תוצאה בכלל
        return matchCandy;
    }

    //בדיקה אם מחובר
    MatchResults IsConnected(candy candy)
    {
        List<candy> connectedCandy = new();
        CandyType candyType = candy.candyType;

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
            // מודיע על התאמה של יותר מ-3
            Debug.Log("has Horizontal mached more then 3 frome type: " + connectedCandy[0].candyType);

            if (connectedCandy.Count == 4)
            {
                return new MatchResults
                {
                    connectedCandy = connectedCandy,
                    direction = MatchDirection.LongHorizontal,
                };
            }
            else if (connectedCandy.Count > 4)
            {
                return new MatchResults
                {
                    connectedCandy = connectedCandy,
                    direction = MatchDirection.Super,
                };
            }
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

            if (connectedCandy.Count == 4)
            {
                return new MatchResults
                {
                    connectedCandy = connectedCandy,
                    direction = MatchDirection.LongVertical,
                };
            }
            else if (connectedCandy.Count > 4)
            {
                return new MatchResults
                {
                    connectedCandy = connectedCandy,
                    direction = MatchDirection.Super,
                };
            }
        }

        return new MatchResults
        {
            connectedCandy = connectedCandy,
            direction = MatchDirection.None,
        };
    }

    //בדוק כיוון
    void CheckDirection(candy candy, Vector2Int direction, List<candy> connectedCandy)
    {
        CandyType candyType = candy.candyType;
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
        // שומר את המיקום האחרון שהשחקן נגע בו
        lastTouchedCandyPos = new Vector2Int(_candy.xIndex, _candy.yIndex);

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

            //selectedCandy = null;
        }
    }

    //מחליף את הממתק
    private void SwapCandy(candy _candy1, candy _candy2)
    {
        //אם זה לא אחד ליד השני
        if (!IsAdjacent(_candy1, _candy2))
        {
            selectedCandy = null;
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
        yield return new WaitForSeconds(0.4f);
        
        //בודק אם יש התאמה
        if (CheckBoard())
        {
            if(!CheckIfCandyIsSpecial(_candy1, _candy2))
            {
                //מתחיל קורוטינה שתטפל בתוצאות
                StartCoroutine(ProsesTurnOnMatchedBoard(true, delayBetweenMatches));
            }
        }
        else if(CheckIfCandyIsSpecial(_candy1, _candy2))
        {
            //StartCoroutine(ProsesTurnOnMatchedBoard(true, delayBetweenMatches));
        }
        //אם אין התאמה אז הם יחזרו לאחור
        else
        {
            DoSwap(_candy1, _candy2);
            //מעדכן שסיים מהלך ואפשר לבצע עוד אחד
            isProcessingMove = false;
        }

        selectedCandy = null;
    }

    private bool CheckIfCandyIsSpecial(candy _candy1, candy _candy2)
    {
        bool isSpecial = false;
        if (_candy1.isSpecial)
        {
            TriggerSpecialCandy(_candy1, _candy2);
            isSpecial = true;
        }
        if (_candy2.isSpecial)
        {
            TriggerSpecialCandy(_candy2, _candy1);
            isSpecial = true;
        }
        return isSpecial;
    }

    private void TriggerSpecialCandy(candy mainCandy, candy otherCandy)
    {
        activeCoroutines++;
        switch (mainCandy.candyType)
        {
            case CandyType.vertical:
                StartCoroutine(PerformVertical(mainCandy));
                break;
            case CandyType.horizontal:
                StartCoroutine(PerformHorizontal(mainCandy));
                break;
            case CandyType.super:
                StartCoroutine(PerformSuper(mainCandy, otherCandy));
                break;
            case CandyType.bomb:
                StartCoroutine(PerformBomb(mainCandy));
                break;
        }
    }
    IEnumerator PerformVertical(candy candy)
    {
        //רשימה של ממתקים לניקוד
        List<candy> _candyList = new();

        //מחכה ואז עושה אפקט
        yield return new WaitForSeconds(0.3f);
        GameObject toDestroy = candy.OnDestroyVFX();

        int maxDistance = Mathf.Max(candy.yIndex, height - candy.yIndex); // המרחק המרבי לכל כיוון

        for (int y = 0; y <= maxDistance; y++)
        {
            bool removedAny = false;

            // בדיקה ומחיקה כלפי מעלה
            int upperY = candy.yIndex + y;
            if (upperY < height && candyBoard[candy.xIndex, upperY].isUsabal && candyBoard[candy.xIndex, upperY].candy != null && y != 0)
            {
                candy targetCandy = candyBoard[candy.xIndex, upperY].candy.GetComponent<candy>();
/*                if (targetCandy != null && targetCandy.isSpecial)
                {
                    TriggerSpecialCandy(targetCandy, candy); // להפעלת שרשרת
                }
                else*/
                {
                    //מוסיף את הממתק לרשימה לניקוד
                    candyForScoring.Add(targetCandy);

                    Destroy(candyBoard[targetCandy.xIndex, targetCandy.yIndex].candy.gameObject);
                    candyBoard[targetCandy.xIndex, targetCandy.yIndex] = new Node(true, null);
                    removedAny = true;
                }
                //removedAny = DeletCandyBySpecial(targetCandy, candy);
            }

            // בדיקה ומחיקה כלפי מטה
            int lowerY = candy.yIndex - y;
            if (lowerY >= 0 && candyBoard[candy.xIndex, lowerY].isUsabal && candyBoard[candy.xIndex, lowerY].candy != null)
            {
                candy targetCandy = candyBoard[candy.xIndex, lowerY].candy.GetComponent<candy>();
/*                if (targetCandy != null && targetCandy.isSpecial && y != 0)
                {
                    TriggerSpecialCandy(targetCandy, candy); // להפעלת שרשרת
                }
                else*/
                {
                    //מוסיף את הממתק לרשימה לניקוד
                    candyForScoring.Add(targetCandy);

                    Destroy(candyBoard[targetCandy.xIndex, targetCandy.yIndex].candy.gameObject);
                    candyBoard[targetCandy.xIndex, targetCandy.yIndex] = new Node(true, null);
                    removedAny = true;
                }
                //removedAny = DeletCandyBySpecial(targetCandy, candy);
            }

            // אם נמחק לפחות אחד - מחכים לפני שממשיכים לשלב הבא
            if (removedAny)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        //משנה ניקוד וכמות מהלכים
        GameManager.Instance.ProcessTurn(_candyList, false);

        // מחכים שהאפקט יסתיים ואז מוחקים אותו
        yield return new WaitForSeconds(0.1f);
        Destroy(toDestroy);

        // מחכים שיראה את המסך ריק לרגע וארז ממלאים
        yield return new WaitForSeconds(0.3f);
        activeCoroutines--;
        if (activeCoroutines == 0)
            StartCoroutine(ProsesTurnOnMatchedBoard(true, delayBetweenMatches));
    }

    private bool DeletCandyBySpecial(candy targetCandy, candy callingCandy)
    {
        if (targetCandy != null && targetCandy.isSpecial)
        {
            TriggerSpecialCandy(targetCandy, callingCandy); // להפעלת שרשרת
        }
        else
        {
            //מוסיף את הממתק לרשימה לניקוד
            candyForScoring.Add(targetCandy);

            Destroy(candyBoard[targetCandy.xIndex, targetCandy.yIndex].candy.gameObject);
            candyBoard[targetCandy.xIndex, targetCandy.yIndex] = new Node(true, null);
            return true;
        }
        return false;
    }


    IEnumerator PerformHorizontal(candy candy)
    {
        //רשימה של ממתקים לניקוד
        List<candy> _candyList = new();

        //מחכה ואז עושה אפקט
        yield return new WaitForSeconds(0.3f);
        GameObject toDestroy = candy.OnDestroyVFX();

        int maxDistance = Mathf.Max(candy.xIndex, width - candy.xIndex); // המרחק המרבי לכל כיוון בציר ה-X

        for (int x = 0; x <= maxDistance; x++)
        {
            bool removedAny = false;

            // בדיקה ומחיקה כלפי ימין
            int rightX = candy.xIndex + x;
            if (rightX < width && candyBoard[rightX, candy.yIndex].isUsabal && candyBoard[rightX, candy.yIndex].candy != null)
            {
                //מוסיף את הממתק לרשימה לניקוד
                _candyList.Add(candyBoard[rightX, candy.yIndex].candy.GetComponent<candy>());

                Destroy(candyBoard[rightX, candy.yIndex].candy.gameObject);
                candyBoard[rightX, candy.yIndex] = new Node(true, null);
                removedAny = true;
            }

            // בדיקה ומחיקה כלפי שמאל
            int leftX = candy.xIndex - x;
            if (leftX >= 0 && candyBoard[leftX, candy.yIndex].isUsabal && candyBoard[leftX, candy.yIndex].candy != null)
            {
                //מוסיף את הממתק לרשימה לניקוד
                _candyList.Add(candyBoard[leftX, candy.yIndex].candy.GetComponent<candy>());

                Destroy(candyBoard[leftX, candy.yIndex].candy.gameObject);
                candyBoard[leftX, candy.yIndex] = new Node(true, null);
                removedAny = true;
            }

            // אם נמחק לפחות אחד - מחכים לפני שממשיכים לשלב הבא
            if (removedAny)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        //משנה ניקוד וכמות מהלכים
        GameManager.Instance.ProcessTurn(_candyList, false);

        // מחכים שהאפקט יסתיים ואז מוחקים אותו
        yield return new WaitForSeconds(0.1f); // זמן מותאם לפי משך האנימציה
        Destroy(toDestroy);


        // מחכים שיראה את המסך ריק לרגע וארז ממלאים
        yield return new WaitForSeconds(0.3f);
        activeCoroutines--;
        if (activeCoroutines == 0)
            StartCoroutine(ProsesTurnOnMatchedBoard(true, delayBetweenMatches));
    }

    IEnumerator PerformSuper(candy candy1, candy candy2)
    {
        //רשימה של ממתקים לניקוד
        List<candy> _candyList = new();

        GameObject efect = candy1.OnDestroyVFX();
        // פעולה לממתק סופר
        // מעבר על כל הלוח כדי למצוא את כל הממתקים עם אותו צבע
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (candyBoard[x, y].isUsabal)
                {
                    candy currentCandy = candyBoard[x, y]?.candy?.GetComponent<candy>();

                    if (currentCandy != null && currentCandy.candyType == candy2.candyType)
                    {
                        //מוסיף את הממתק לרשימה לניקוד
                        _candyList.Add(currentCandy);

                        Destroy(candyBoard[x, y].candy.gameObject);
                        candyBoard[x, y] = new Node(true, null);
                        //candyToRemove.Add(currentCandy);
                    }
                }
            }
        }
        //מוסיף את הממתק לרשימה לניקוד
        _candyList.Add(candyBoard[candy1.xIndex, candy1.yIndex].candy.GetComponent<candy>());

        Destroy(candyBoard[candy1.xIndex, candy1.yIndex].candy.gameObject);
        candyBoard[candy1.xIndex, candy1.yIndex] = new Node(true, null);
        //candyToRemove.Add(candy1);

        //משנה ניקוד וכמות מהלכים
        GameManager.Instance.ProcessTurn(_candyList, false);

        yield return new WaitForSeconds(0.1f);
        Destroy(efect);

        // מחכים שיראה את המסך ריק לרגע וארז ממלאים
        yield return new WaitForSeconds(0.3f);
        activeCoroutines--;
        if (activeCoroutines == 0)
            StartCoroutine(ProsesTurnOnMatchedBoard(true, delayBetweenMatches));

        yield break;
    }
    IEnumerator PerformBomb(candy candy)
    {
        //רשימה של ממתקים לניקוד
        List<candy> _candyList = new();

        GameObject efect = candy.OnDestroyVFX();
        int explosionRadius = 1; //  רדיוס 1 לכל כיוון יוצר אזור 3x3

        for (int dx = -explosionRadius; dx <= explosionRadius; dx++)
        {
            for (int dy = -explosionRadius; dy <= explosionRadius; dy++)
            {
                int newX = candy.xIndex + dx;
                int newY = candy.yIndex + dy;

                // בדיקה שהאינדקסים תקפים בתוך גבולות הלוח
                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    candy currentCandy = candyBoard[newX, newY]?.candy?.GetComponent<candy>();

                    if (currentCandy != null)
                    {
                        //מוסיף את הממתק לרשימה לניקוד
                        _candyList.Add(currentCandy);

                        Destroy(candyBoard[newX, newY].candy.gameObject);
                        candyBoard[newX, newY] = new Node(true, null);
                        //candyToRemove.Add(currentCandy);
                    }
                }
            }
        }

        //משנה ניקוד וכמות מהלכים
        GameManager.Instance.ProcessTurn(_candyList, false);

        yield return new WaitForSeconds(0.1f);
        Destroy(efect);

        // מחכים שיראה את המסך ריק לרגע וארז ממלאים
        yield return new WaitForSeconds(0.3f);
        activeCoroutines--;
        if (activeCoroutines == 0)
            StartCoroutine(ProsesTurnOnMatchedBoard(true, delayBetweenMatches));

        yield break;
    }


    //בודק אם הם אחד ליד השני
    private bool IsAdjacent(candy _candy1, candy _candy2)
    {
        return Mathf.Abs(_candy1.xIndex - _candy2.xIndex) + Mathf.Abs(_candy1.yIndex - _candy2.yIndex) == 1;
    }
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