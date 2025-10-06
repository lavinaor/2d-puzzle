using UnityEngine;

public class SimpleMeshDestroyer : MonoBehaviour
{
    public int fragmentsPerAxis = 2;
    public float explosionForce = 300f;
    public float explosionRadius = 2f;

    public void DestroyObject()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 startPos = transform.position - (originalScale / 2f);

        for (int x = 0; x < fragmentsPerAxis; x++)
        {
            for (int y = 0; y < fragmentsPerAxis; y++)
            {
                for (int z = 0; z < fragmentsPerAxis; z++)
                {
                    Vector3 cubeSize = originalScale / fragmentsPerAxis;
                    Vector3 cubePos = startPos + Vector3.Scale(cubeSize, new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));

                    GameObject fragment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    fragment.transform.position = cubePos;
                    fragment.transform.localScale = cubeSize;
                    fragment.AddComponent<Rigidbody>().mass = 0.2f;
                }
            }
        }

        // הוספת כוח פיצוץ לכל השברים
        foreach (var rb in FindObjectsOfType<Rigidbody>())
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        Destroy(gameObject); // הסתרת האובייקט המקורי
    }
}
