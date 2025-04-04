using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private float deley;
    public void OnChangeSeneDeley(int scene)
    {
        StartCoroutine(DeleyAndThenLode(scene));
    }

    IEnumerator DeleyAndThenLode(int scene)
    {
        yield return new WaitForSeconds(deley);

        SceneManager.LoadScene(scene);
    }
}
