using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.VFX;

public class candy : MonoBehaviour
{

    public CandyType candyType;
    public bool isSpecial = false;

    public int xIndex;
    public int yIndex;

    public bool isMatched;
    private Vector2 current;
    private Vector2 targetPause;

    //���� �����
    public GameObject VFXOnDestroyPrefab;

    [SerializeField]
    private float duration = 0.5f;

    public bool isMoving;

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


    //��� ����� �������
    public void MoveToTarget(Vector2 target)
    {
        StartCoroutine(MoveCoroutine(target));
    }

    //�������� ����
    private IEnumerator MoveCoroutine(Vector2 target)
    {
        Debug.Log($"Swapping candy {this.name} at {transform.position} with candy at {target}");
        //���� ��� ��
        isMoving = true;

/*        //������ ������ ���� �� ������ ��� ��� ��������� �� ���� ���
        float duration = 0.2f;*/

        //���� ����� ������
        Vector2 startPosition = transform.position;

        //���� �� ���� ���� ���
        float elasperTime = 0f;

        //���� �� ��� �� �� ���� ������ ��� ��� ���� ���� �� ����� �����
        while (elasperTime < duration)
        {
            //���� ����� 
            float t = elasperTime / duration;

            //�� ���
            transform.position = Vector2.Lerp(startPosition, target, t);

            //���� �� ���� ����
            elasperTime += Time.deltaTime;

            //���� ��������� ��� �� �������
            yield return null;
        }
        
        //����� ��� �� ����� �������
        transform.position = target;

        //���� ����� �����
        isMoving = false;
        Debug.Log($"Swapping candy {this.name} at {transform.position} with candy at {target}");
    }

    public GameObject OnDestroyVFX()
    {
        if (VFXOnDestroyPrefab != null)
        {
            // ����� ���� �������
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
    random,
    Red,
    Blue,
    Green,
    Purple,
    White,
    redSqwer,
    horizontal,
    vertical,
    bomb,
    super
}
