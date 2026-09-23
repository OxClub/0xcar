using UnityEngine;
using UnityEngine.EventSystems;

public class TouchControls : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ControlType { Accelerate, Brake, Left, Right }
    public ControlType controlType;

    private CarController car;

    void Start()
    {
        car = FindObjectOfType<CarController>();
    }

    public void OnPointerDown(PointerEventData eventData) => SetState(true);
    public void OnPointerUp(PointerEventData eventData) => SetState(false);

    private void SetState(bool pressed)
    {
        if (car == null) return;
        switch (controlType)
        {
            case ControlType.Accelerate: car.accelerate = pressed; break;
            case ControlType.Brake:      car.brake = pressed; break;
            case ControlType.Left:       if (pressed) car.moveLeft = true; break;
            case ControlType.Right:      if (pressed) car.moveRight = true; break;
        }
    }
}
