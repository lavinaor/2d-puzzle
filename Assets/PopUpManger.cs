using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UIManager;

public class PopUpManger : MonoBehaviour
{
    //שומר את כל הui
    [SerializeField]
    private PopUp[] popUps = new PopUp[0];

    //שומר על הנוככי שדלוק
    [SerializeField]
    private PopUpType currentPopUpType = PopUpType.NoneUI;

    //שומר את המספר של המסך הראשי
    [SerializeField]
    private int MainManuNum;

    // מסדר את זה בתחילת המשחק
    void Start()
    {
        //מפעיל את הפעולה שמשנה
        ChangeActiveUI();
    }

    //משנה את המצב לפי הסוג שנכנס
    public void ChangeUIState(int newState)
    {
        //לחיצה על אותו הדבר מכבה אותו
        if (currentPopUpType == (PopUpType)newState)
        {
            currentPopUpType = PopUpType.NoneUI;
        }
        else
        {
            //שומר את זה כנוככי
            currentPopUpType = (PopUpType)newState;
        }

        //מפעיל שינוי
        ChangeActiveUI();
    }

    // משנה את הUi הנוככי
    public void ChangeActiveUI()
    {
        //עובר על כל האופציות מחבה את הלא נכונים ומדליק את הנכון
        foreach (PopUp popUp in popUps)
        {
            //בודק אם נכון
            if (popUp.popUpType == currentPopUpType)
            {
                //מדליק את הנכון
                popUp.popUpGameObject.SetActive(true);

                //בודק אם יש זמן או לא
                if (popUp.isTimePase)
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
                popUp.popUpGameObject.SetActive(false);
            }

        }

        //אם אין אז תמשיך בלי כלום
        if (currentPopUpType == PopUpType.NoneUI) 
        {
            Time.timeScale = 1f;
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

    // מעביר לסצנה אחרת
    public void GoToNewScene(int sceneNum)
    {
        //טוען את הסצנה החדשה
        SceneManager.LoadScene(sceneNum);

        //מחזיר את הזמן לזוז
        Time.timeScale = 1f; // Resume the game
    }
}

//כיתה שמכילה את הסוג את האובייקט ואת האם הזמן שם זז או לא
[System.Serializable]
class PopUp
{
    //שומר על סוג הui
    public PopUpType popUpType = PopUpType.NoneUI;

    //שומר את האובייקט
    public GameObject popUpGameObject;

    //שומר האם הזמן זז 
    public bool isTimePase = false;
}

//סוגים של Ui
public enum PopUpType
{
    NoneUI = 0,
    settingsUI = 1,
    storUI = 2,
    confirmation = 3,
    endScreen = 4,
}
