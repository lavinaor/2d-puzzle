using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyBoard : MonoBehaviour
{

    //מגדיר את גודל הלוח
    public int width = 6;
    public int height = 8;
    public int spacingDevidor = 2;

    // מגדיר רווחים ללוח
    public float spacingX;
    public float spacingY;

    //לקבל את הפריפבים של הממתקים
    public GameObject[] candyPrefabs;

    //לקבל את הלוח ואת האובייקט
    private Node[,] candyBoard;
    public GameObject candyBoardGO;

    //layoutarray
    

    // ליצור פבליק לבורד
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
                //מחשב מיקום
                Vector2 pos = new Vector2(x - spacingX, y - spacingY);

                //נותן מספר רנדומלי של איזה צבע ממתק
                int randomIndex = Random.Range(0, candyPrefabs.Length);

                //מיצר ממתק מהסוג הרנדומלי במיקום הנוככי ומגדיר אותו בקוד שלו
                GameObject candy =  Instantiate(candyPrefabs[randomIndex], pos / spacingDevidor, Quaternion.identity);
                //מגדיר אותו 
                candy.GetComponent<candy>().setIndicies(x / spacingDevidor, y / spacingDevidor);
                // מוסיף אותו למערך
                candyBoard[x,y] = new Node(true, candy);
            }
        }
    }
}
