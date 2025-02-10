using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class candy : MonoBehaviour
{

    public candyType candyType;

    public int xIndex;
    public int yIndex;

    public bool isMatched;
    private Vector2 current;
    private Vector2 targetPause;

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
}

public enum candyType
{
    Red,
    Blue,
    Green,
    Purple,
    White
}
