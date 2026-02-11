using UnityEngine;
using System.Collections.Generic;

public class Seat : MonoBehaviour
{
    [Header("Configuration")]   
    public SeatType seatType = SeatType.Normal;
    

    public SeatColumn seatColumn = SeatColumn.Milieu;
    public SeatRow seatRow = SeatRow.Milieu;
    
    [Tooltip("Distance maximum pour détecter un voisin (doit être un peu plus grand que l'espacement de ta grille)")]
    public float detectRadius = 2.1f;

    [Header("État")]
    public Dino occupant; 

    [Header("Voisins (Détectés auto)")]
    public Seat front;
    public Seat back;
    public Seat left;
    public Seat right;

    [HideInInspector] public Seat[] neighbors; 

    private void Awake()
    {
        AutoDetectNeighbors();
    }

    private void Start()
    {
        var list = new List<Seat>();
        if (front) list.Add(front);
        if (back) list.Add(back);
        if (left) list.Add(left);
        if (right) list.Add(right);
        neighbors = list.ToArray();
    }

    private void AutoDetectNeighbors()
    {
        front = back = left = right = null;

        Seat[] allSeats = Object.FindObjectsByType<Seat>(FindObjectsSortMode.None);

        foreach (Seat other in allSeats)
        {
            if (other == this) continue;

            Vector3 dir = other.transform.position - transform.position;
            float dist = dir.magnitude;

            if (dist <= detectRadius)
            {
                Vector3 normDir = dir.normalized;

                if (Mathf.Abs(normDir.x) > Mathf.Abs(normDir.y))
                {
                    if (normDir.x > 0.5f) right = other; 
                    else if (normDir.x < -0.5f) left = other;  
                }
                else
                {
                    if (normDir.y > 0.5f) front = other;       
                    else if (normDir.y < -0.5f) back = other;  
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (occupant == null) Gizmos.color = Color.green; 
        else Gizmos.color = Color.red;   
        
        Vector3 TopLeft = transform.position - transform.right / 2 - transform.up / 2;
        Vector3 TopRight = transform.position + transform.right / 2 - transform.up / 2;
        Vector3 BotRight = transform.position + transform.right / 2 + transform.up / 2;
        Vector3 BotLeft = transform.position - transform.right / 2 + transform.up / 2;

        Gizmos.DrawLine(TopLeft, TopRight);
        Gizmos.DrawLine(TopRight, BotRight);
        Gizmos.DrawLine(BotRight, BotLeft);
        Gizmos.DrawLine(BotLeft, TopLeft);
        
        Gizmos.color = Color.cyan;
        if (front) Gizmos.DrawLine(transform.position, front.transform.position);
        if (back) Gizmos.DrawLine(transform.position, back.transform.position);
        if (left) Gizmos.DrawLine(transform.position, left.transform.position);
        if (right) Gizmos.DrawLine(transform.position, right.transform.position);
    }
}