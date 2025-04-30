using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.VFX;

public class candy : MonoBehaviour
{
    //סוג הממתק
    public CandyType candyType;

    //האם הוא ממתק מיוחד
    public bool isSpecial = false;

    //אפקט בזמן הריסה
    public GameObject VFXOnDestroyPrefab;

    //אורף אפקט
    [SerializeField]
    private float duration = 0.5f;

    //המיקום שלו בלוח לפי צירים
    [HideInInspector]
    public int xIndex;
    [HideInInspector]
    public int yIndex;

    //אם הוא תפוס במטרה לעזור לבדיקה 
    [HideInInspector]
    public bool isMatched;

    //בזמן תזוזה זה פעיל
    [HideInInspector]
    public bool isMoving;

    private void Start()
    {
        Sprite sprite = CandySkinManager.Instance.GetCandySpriteByType(candyType);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    public candy(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    public void setIndicies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }


    //זוז למטרה מסויימת
    public void MoveToTarget(Vector2 target)
    {
        StartCoroutine(MoveCoroutine(target));
    }

    //קורוטינת הזזה
    private IEnumerator MoveCoroutine(Vector2 target)
    {
        Debug.Log($"Swapping candy {this.name} at {transform.position} with candy at {target}");
        //מסמן שזה זז
        isMoving = true;

/*        //אפשרות לשינוי הזמן של ההחלפה לפי זמן האנימצייה או משהו אחר
        float duration = 0.2f;*/

        //שומר מיקום התחלתי
        Vector2 startPosition = transform.position;

        //מיצר את הזמן שעבר כבר
        float elasperTime = 0f;

        //פועל כל עוד זה זז לזמן התזוזה ככה שזה נותן אפקט של תזוזה איטית
        while (elasperTime < duration)
        {
            //מחשב תזוזה 
            float t = elasperTime / duration;

            //זז מעט
            transform.position = Vector2.Lerp(startPosition, target, t);

            //מוסף את הזמן שלקח
            elasperTime += Time.deltaTime;

            //חובה לקורוטינה אבל לא רלוונטי
            yield return null;
        }
        
        //מוודא שזה זז בצורה מדוייקט
        transform.position = target;

        //מסמן שסיים תזוזה
        isMoving = false;
        Debug.Log($"Swapping candy {this.name} at {transform.position} with candy at {target}");
    }

    public GameObject OnDestroyVFX()
    {
        if (VFXOnDestroyPrefab != null)
        {
            // יצירת אפקט ויזואלי
            GameObject effect = Instantiate(VFXOnDestroyPrefab, this.transform.position, Quaternion.identity);
            VisualEffect VFXOnDestroy = effect.GetComponent<VisualEffect>();
            VFXOnDestroy.Play();
            return effect;
        }
        return null;
    }
}

public enum CandyType
{
    Red,
    Blue,
    Green,
    Purple,
    White,
    redSqwer,
    horizontal,
    vertical,
    bomb,
    super,
    random
}
