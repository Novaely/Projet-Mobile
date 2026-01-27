using UnityEngine;

public class EmplacementController : MonoBehaviour
{
    public GameObject _storage;
    private void OnDrawGizmosSelected()
    {
        if (_storage == null)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Vector3 TopLeft = transform.position - transform.right / 2 - transform.up / 2;
        Vector3 TopRight = transform.position + transform.right / 2 - transform.up / 2;
        Vector3 BotRight = transform.position + transform.right / 2 + transform.up / 2;
        Vector3 BotLeft = transform.position - transform.right / 2 + transform.up / 2;


        Gizmos.DrawLine(TopLeft, TopRight);
        Gizmos.DrawLine(TopRight, BotRight);
        Gizmos.DrawLine(BotRight, BotLeft);
        Gizmos.DrawLine(BotLeft, TopLeft);
    }
}