using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class PopupBlocker : MonoBehaviour, IPointerClickHandler
{
    void Awake()
    {
        // מוסיף Collider2D אם אין
        if (GetComponent<Collider2D>() == null)
            gameObject.AddComponent<BoxCollider2D>();
    }

    // קליק על הפופ-אפ → חוסם הכל מאחור
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("לחיצה על הפופ-אפ → חוסם כל מה שמתחת");
        // אין שום פעולה נוספת → הלחיצה לא תעבור הלאה
    }
}
