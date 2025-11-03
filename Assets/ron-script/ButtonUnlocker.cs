using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // חשוב בשביל טעינת סצנות

public class ButtonUnlocker : MonoBehaviour
{
    [SerializeField] private Button targetButton; // הכפתור שאתה רוצה להשבית/לפתוח
    [SerializeField] private TextMeshProUGUI starsText; // שדה טקסט להצגת הכוכבים
    [SerializeField] private int requiredStars = 20; // מספר הכוכבים הנדרש
    [SerializeField] private string sceneNameToLoad; // שם הסצנה שאליה עוברים בלחיצה

    void Start()
    {
        UpdateButtonAndText();

        // מוסיפים מאזין לכפתור
        if (targetButton != null)
            targetButton.onClick.AddListener(OnButtonClicked);
    }

    public void UpdateButtonAndText()
    {
        int currentStars = SaveManager.Instance.GetTotalStars();

        // עדכון הכפתור
        targetButton.interactable = currentStars >= requiredStars;

        // עדכון הטקסט
        if (starsText != null)
            starsText.text = currentStars + " / " + requiredStars + " Stars";
    }

    private void OnButtonClicked()
    {
        if (!string.IsNullOrEmpty(sceneNameToLoad))
            SceneManager.LoadScene(sceneNameToLoad); // טוען את הסצנה שהגדרת
    }
}
