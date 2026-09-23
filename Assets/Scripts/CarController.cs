using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 8f;
    public float maxSpeed = 20f;
    public float acceleration = 5f;
    public float maxSteerAngle = 15f;

    [Header("Lane Settings")]
    public float laneWidth = 2.2f;
    public int totalLanes = 3;
    private int currentLane = 1;

    [Header("References")]
    public Transform carBody;

    private float currentSpeed = 0f;
    private float targetX;
    private float startX;

    [HideInInspector] public bool accelerate;
    [HideInInspector] public bool brake;
    [HideInInspector] public bool moveLeft;
    [HideInInspector] public bool moveRight;

    void Start()
    {
        startX = transform.position.x;
        targetX = startX + (currentLane - 1) * laneWidth;
    }

    void Update()
    {
        HandleSpeed();
        HandleLaneInput();
        HandleSteering();
    }

    private void HandleSpeed()
    {
        if (accelerate)
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        else if (brake)
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * 2f * Time.deltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, forwardSpeed, acceleration * Time.deltaTime);

        transform.Translate(Vector3.up * currentSpeed * Time.deltaTime);
    }

    private void HandleLaneInput()
    {
        if (moveLeft && currentLane > 0)
        {
            currentLane--;
            moveLeft = false;
            UpdateTargetX();
        }
        if (moveRight && currentLane < totalLanes - 1)
        {
            currentLane++;
            moveRight = false;
            UpdateTargetX();
        }
    }

    private void UpdateTargetX()
    {
        targetX = startX + (currentLane - 1) * laneWidth;
    }

    private void HandleSteering()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * 10f);
        transform.position = pos;

        if (carBody != null)
        {
            float diff = targetX - transform.position.x;
            float tilt = Mathf.Clamp(diff * maxSteerAngle, -maxSteerAngle, maxSteerAngle);
            carBody.rotation = Quaternion.Euler(0, 0, tilt);
        }
    }

    public float GetSpeed() => currentSpeed;
    public float GetSpeedKmh() => currentSpeed * 10f;
}
