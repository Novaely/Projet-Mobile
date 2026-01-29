using UnityEngine;
using UnityEngine.InputSystem;

public class DinoController : MonoBehaviour
{
    [SerializeField]
    Transform _lastPosition;

    public Transform LastPosition { get { return _lastPosition; } }

    public void Displace()
    {
        Vector2 mousePos;
        if (Input.touchCount != 0)
        {
            Touch touch0 = Input.GetTouch(0);
            mousePos = Camera.main.ScreenToWorldPoint(touch0.position);
            transform.position = mousePos;
        }

    }

    public void ReturnToLastPosition()
    {
        transform.position = _lastPosition.position;
    }

    public void SetPosition(Transform value)
    {
        _lastPosition = value;
        transform.position = value.position;
    }
}