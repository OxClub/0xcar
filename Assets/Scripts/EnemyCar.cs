using UnityEngine;

public class EnemyCar : MonoBehaviour
{
    public float speed = 4f;
    public int lane;
    private float despawnY = -8f;

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y < despawnY) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            GameManager.Instance.GameOver();
    }
}
