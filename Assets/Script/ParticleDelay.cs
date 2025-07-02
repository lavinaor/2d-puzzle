using UnityEngine;
using System.Collections;

public class ParticleDelay : MonoBehaviour
{
    public ParticleSystem particleToStart;
    public float delaySeconds = 2f;

    private void Start()
    {
        if (particleToStart == null)
        {
            Debug.LogWarning("No particle system assigned.");
            return;
        }

        particleToStart.Stop();
        StartCoroutine(StartAfterDelay());
    }

    private IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);
        particleToStart.Play();
    }
}
