using UnityEngine;

public class TestZoom : MonoBehaviour
{
    Camera _cam;

    [SerializeField] float _maxZoomValue;
    [SerializeField] float _minZoomValue;

    void Start()
    {
        TryGetComponent(out _cam);
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Positions précédentes
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevDistance = Vector2.Distance(touch0PrevPos, touch1PrevPos);
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            float zoomDelta = currentDistance - prevDistance;

            // zoomDelta > 0 => zoom in
            if (zoomDelta > 0)
            {
                ZoomIn();
            }
            // zoomDelta < 0 => zoom out
            else if (zoomDelta < 0)
            {
                ZoomOut();
            }
            Debug.Log("Zoom delta : " + zoomDelta);
        }
    }


    public void ZoomIn()
    {
        if (_cam.orthographicSize > _maxZoomValue)
        {
            _cam.orthographicSize -= 0.1f;
        }
    }

    public void ZoomOut()
    {
        if (_cam.orthographicSize < _minZoomValue)
        {
            _cam.orthographicSize += 0.1f;
        }
    }
}
