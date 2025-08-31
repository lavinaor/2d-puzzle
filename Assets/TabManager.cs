using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Tab
{
    public Button tabButton;       // הכפתור של הטאב
    public GameObject contentPanel; // הפאנל של התוכן
}

public class TabManager : MonoBehaviour
{
    [SerializeField] private List<Tab> tabs;
    private int currentIndex = -1;

    private void Start()
    {
        // חיבור אוטומטי של הכפתורים לפונקציית החלפת טאב
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i; // חובה לשמור משתנה לוקאלי לסגירה נכונה בלולאה
            tabs[i].tabButton.onClick.AddListener(() => SwitchTab(index));
        }

        // ברירת מחדל - להפעיל את הטאב הראשון
        if (tabs.Count > 0)
            SwitchTab(0);
    }

    public void SwitchTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        // כיבוי כל הפאנלים
        for (int i = 0; i < tabs.Count; i++)
        {
            tabs[i].contentPanel.SetActive(i == index);
        }

        currentIndex = index;
    }

    // פונקציות עזר שאפשר להשתמש בהן בעתיד
    public void AddTab(Tab newTab)
    {
        tabs.Add(newTab);
        int index = tabs.Count - 1;
        newTab.tabButton.onClick.AddListener(() => SwitchTab(index));
    }

    public void RemoveTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;
        tabs.RemoveAt(index);
    }
}
