using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetActiveDeley : MonoBehaviour
{
    [SerializeField]
    private float deley;

    [SerializeField]
    private bool mode;

    public void OnSetActiveDeley(GameObject scene)
    {
        StartCoroutine(DeleyAndThenSetActive(scene));
    }

    IEnumerator DeleyAndThenSetActive(GameObject scene)
    {
        yield return new WaitForSeconds(deley);

        scene.SetActive(mode);
    }
}
