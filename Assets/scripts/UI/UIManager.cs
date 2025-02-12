using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //שומר את כל הui
    [SerializeField]
    private UI[] UIAray = new UI[0];

    //שומר על הנוככי שדלוק
    [SerializeField]
    private UIType currentUIType = UIType.NoneUI;

    //שומר את המספר של המסך הראשי
    [SerializeField]
    private int MainManuNum;

    //כיתה שמכילה את הסוג את האובייקט ואת האם הזמן שם זז או לא
    [System.Serializable]
    class UI
    {
        //שומר על סוג הui
        public UIType UIType = UIType.NoneUI;

        //שומר את האובייקט
        public GameObject UIGameObject;

        //שומר האם הזמן זז 
        public bool isTimePase = false;
    }

    //סוגים של Ui
    public enum UIType
    {
        NoneUI = 0,
        settingsUI = 1,
        loseUI = 2,
        winUI = 3
    }

    // מסדר את זה בתחילת המשחק
    void Start()
    {
        //מפעיל את הפעולה שמשנה
        ChangeActiveUI();
    }

    //משנה את המצב לפי הסוג שנכנס
    public void ChangeUIState(int newState)
    {
        //שומר את זה כנוככי
        currentUIType = (UIType)newState;

        //מפעיל שינוי
        ChangeActiveUI();
    }

    // משנה את הUi הנוככי
    public void ChangeActiveUI()
    {
        //עובר על כל האופציות מחבה את הלא נכונים ומדליק את הנכון
        foreach (UI ui in UIAray)
        {
            //בודק אם נכון
            if (ui.UIType == currentUIType)
            {
                //מדליק את הנכון
                ui.UIGameObject.SetActive(true);

                //בודק אם יש זמן או לא
                if (ui.isTimePase)
                {
                    Time.timeScale = 0f;
                }
                else
                {
                    Time.timeScale = 1f;
                }
            }
            else
            {
                //מחבה את הלא פעיל
                ui.UIGameObject.SetActive(false);
            }
        }
    }

    // מתחיל מחדש את הסצנה 
    public void RestartLevel()
    {
        //מאתר את הסצנה הנוככית ומתחיל אותה מחדש
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        //משנה את הזמן ללוז במידה והוא לא 
        Time.timeScale = 1f;
    }

    //סוגר את המשחק
    public void ExitGame()
    {
        //עובד ככה שביוניטי זה יכבה את המשחק ומחות ליוניטי זה יכבה את האפליקציה וכאלה
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }

    // מחזיר למסך ההתחלה
    public void GoToMainManu()
    {
        //טוען את מסך ההתחלה לפי מה שהוכנס
        SceneManager.LoadScene(MainManuNum);
        
        //מחזיר את הזמן לזוז
        Time.timeScale = 1f; // Resume the game
    }
}
