using UnityEngine;
using UnityEngine.UI;

public class ButtonCycler : MonoBehaviour
{
    public GameObject[] buttonsToShow;
    private int currentIndex = 0;

    public void ShowNextButton()
    {
        // כבה את כולם
        foreach (GameObject btn in buttonsToShow)
        {
            btn.SetActive(false);
        }

        // הדלק רק את הנוכחי
        if (buttonsToShow.Length > 0)
        {
            buttonsToShow[currentIndex].SetActive(true);
            currentIndex = (currentIndex + 1) % buttonsToShow.Length; // מעגלי
        }
    }
}