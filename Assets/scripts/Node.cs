using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    //כדי לבדוק אם זה אפשרי למילוי
    public bool isUsabal;

    public GameObject candy;

    public Node(bool _isUsabal, GameObject _candy)
    {
        this.isUsabal = _isUsabal;
        this.candy = _candy;
    }
}
