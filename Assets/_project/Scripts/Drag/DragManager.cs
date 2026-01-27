using UnityEngine;
using UnityEngine.InputSystem;

public class DragManager : MonoBehaviour
{
    [SerializeField] InputActionReference _dragInputAction;
    [SerializeField] LayerMask layerMask;

    DinoController _currentItemSelected;
    EmplacementController _lastEmplacement;

    private void Start()
    {
        _dragInputAction.action.started += DragStarted;
        _dragInputAction.action.canceled += DragCanceled;
    }

    private void OnDestroy()
    {
        _dragInputAction.action.started -= DragStarted;
        _dragInputAction.action.canceled -= DragCanceled;
    }

    private void Update()
    {
        if (_currentItemSelected == null) { return; }

        _currentItemSelected.Displace();
    }

    void DragStarted(InputAction.CallbackContext context)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            // Call methods here
            Debug.Log("Raycast Hit -> " + hit.transform.name);
            if (hit.transform.TryGetComponent(out DinoController item))
            {
                _currentItemSelected = item;

                Vector2 mousePosPlacement = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                RaycastHit2D hitPlacement = Physics2D.Raycast(mousePosPlacement, Vector2.zero, 10000, layerMask);

                if (hitPlacement)
                {
                    _lastEmplacement = hitPlacement.transform.GetComponent<EmplacementController>();
                }
            }
        }
    }

    private void DragCanceled(InputAction.CallbackContext context)
    {
        if (_currentItemSelected == null) { return; }

        Vector2 mousePosPlacement = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hitPlacement = Physics2D.Raycast(mousePosPlacement, Vector2.zero,10000, layerMask);

        if (hitPlacement)
        {
            Debug.Log("Raycast Hit -> " + hitPlacement.transform.name);
            if (hitPlacement.transform.TryGetComponent(out EmplacementController item))
            {
                if (item._storage == null)
                {
                    item._storage = _currentItemSelected.gameObject;
                    _currentItemSelected.SetPosition(item.transform);
                    if (_lastEmplacement != null)
                    {
                        _lastEmplacement._storage = null;
                        _lastEmplacement = null;
                    }
                }
                else
                {
                    _currentItemSelected.ReturnToLastPosition();
                }
            }
        }
        else
        {
            _currentItemSelected.ReturnToLastPosition();
        }
        _currentItemSelected = null;
    }
}