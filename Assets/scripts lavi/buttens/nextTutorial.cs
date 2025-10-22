using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextTutorial : MonoBehaviour
{
    [SerializeField]
    private float deley;
    public void OnChangeSeneDeley(string scene)
    {
        StartCoroutine(DeleyAndThenLode(scene));
    }

    IEnumerator DeleyAndThenLode(string scene)
    {
        yield return new WaitForSeconds(deley);

        SceneManager.LoadScene(scene);
    }
}
