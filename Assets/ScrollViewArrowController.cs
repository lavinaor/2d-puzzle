using UnityEngine;
using UnityEngine.UI;

public class ScrollViewArrowController : MonoBehaviour
{
    public ScrollRect scrollRect;                // ה-ScrollRect שלך
    public RectTransform content;               // התוכן שמוחזק בתוך ה-ScrollView
    public HorizontalLayoutGroup layoutGroup;   // ה-Horizontal Layout Group
    public int currentIndex = 0;                // האינדקס הנוכחי
    public float scrollDuration = 0.2f;         // כמה זמן האנימציה תיקח

    private int totalItems;
    private RectTransform[] items;
    private bool isScrolling = false;
    private float scrollTimer = 0f;
    private float startScrollPos;
    private float targetScrollPos;

    void Start()
    {
        // שומרים את כל האייטמים בתוך מערך
        totalItems = content.childCount;
        items = new RectTransform[totalItems];
        for (int i = 0; i < totalItems; i++)
        {
            items[i] = content.GetChild(i).GetComponent<RectTransform>();
        }
    }

    public void ScrollLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ScrollToIndex(currentIndex);
        }
    }

    public void ScrollRight()
    {
        if (currentIndex < totalItems - 1)
        {
            currentIndex++;
            ScrollToIndex(currentIndex);
        }
    }

    private void ScrollToIndex(int index)
    {
        if (isScrolling) return;

        float totalWidth = content.rect.width - scrollRect.viewport.rect.width; // הרוחב שניתן לגלול
        float itemWidth = items[0].rect.width + layoutGroup.spacing;           // רוחב אייטם + ספייסינג
        float targetPixelPos = index * itemWidth;                               // לאיזה פיקסל להגיע
        float normalizedPos = Mathf.Clamp01(targetPixelPos / totalWidth);       // המרה ל-Normalized Position

        // הכנה לאנימציה
        startScrollPos = scrollRect.horizontalNormalizedPosition;
        targetScrollPos = normalizedPos;
        scrollTimer = 0f;
        isScrolling = true;
    }

    void Update()
    {
        if (isScrolling)
        {
            scrollTimer += Time.deltaTime / scrollDuration;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startScrollPos, targetScrollPos, scrollTimer);

            if (scrollTimer >= 1f)
            {
                isScrolling = false;
            }
        }
    }
}
