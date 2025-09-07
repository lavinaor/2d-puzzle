using UnityEngine;
using UnityEngine.UI;

public class ScrollViewArrowController : MonoBehaviour
{
    public ScrollRect scrollRect;                // �-ScrollRect ���
    public RectTransform content;               // ����� ������ ���� �-ScrollView
    public HorizontalLayoutGroup layoutGroup;   // �-Horizontal Layout Group
    public int currentIndex = 0;                // ������� ������
    public float scrollDuration = 0.2f;         // ��� ��� �������� ����

    private int totalItems;
    private RectTransform[] items;
    private bool isScrolling = false;
    private float scrollTimer = 0f;
    private float startScrollPos;
    private float targetScrollPos;

    void Start()
    {
        // ������ �� �� �������� ���� ����
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

        float totalWidth = content.rect.width - scrollRect.viewport.rect.width; // ����� ����� �����
        float itemWidth = items[0].rect.width + layoutGroup.spacing;           // ���� ����� + ��������
        float targetPixelPos = index * itemWidth;                               // ����� ����� �����
        float normalizedPos = Mathf.Clamp01(targetPixelPos / totalWidth);       // ���� �-Normalized Position

        // ���� ��������
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
