using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 5f; // Puedes ajustar esto en cada prefab

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        if (transform.position.z < -10f)
        {
            Destroy(gameObject);
        }
    }
}
