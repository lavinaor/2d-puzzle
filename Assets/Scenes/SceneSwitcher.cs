using UnityEngine;
using UnityEngine.SceneManagement; // חובה בשביל לטעון סצנות

public class SceneSwitcher : MonoBehaviour
{
    // שמות הסצנות שאתה רוצה לעבור אליהן
    public string sceneName;

    // הפונקציה שנקראת כשיש לחיצה על הכפתור
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
