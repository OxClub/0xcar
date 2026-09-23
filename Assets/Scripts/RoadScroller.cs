using UnityEngine;

public class RoadScroller : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float tileHeight = 10f;
    private CarController car;
    private Vector3 startPos;

    void Start()
    {
        car = FindObjectOfType<CarController>();
        startPos = transform.position;
    }

    void Update()
    {
        float speed = car != null ? car.GetSpeed() : scrollSpeed;
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y <= startPos.y - tileHeight)
            transform.position = startPos;
    }
}
