using UnityEngine;
using System.Collections.Generic;

public class Seat : MonoBehaviour
{
    [Header("Configuration")]   
    public SeatType seatType = SeatType.Normal;

    [Tooltip("COCHER pour les sièges de la file d'attente (Spawn) !")]
    public bool isSpawnSeat = false; 

    public SeatColumn seatColumn = SeatColumn.Milieu;
    public SeatRow seatRow = SeatRow.Milieu;
    
    [Header("État")]
    public Dino occupant; 

    [Header("Voisins (Manuel)")]
    public Seat front;
    public Seat back;
    public Seat left;
    public Seat right;

    [HideInInspector] public Seat[] neighbors; 

    private void Start()
    {
        var list = new List<Seat>();
        if (front) list.Add(front);
        if (back) list.Add(back);
        if (left) list.Add(left);
        if (right) list.Add(right);
        neighbors = list.ToArray();
    }

    private void OnDrawGizmosSelected()
    {
        if (occupant == null) Gizmos.color = Color.green; 
        else Gizmos.color = Color.red;   
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        
        Gizmos.color = Color.cyan;
        if (front) Gizmos.DrawLine(transform.position, front.transform.position);
        if (back) Gizmos.DrawLine(transform.position, back.transform.position);
        if (left) Gizmos.DrawLine(transform.position, left.transform.position);
        if (right) Gizmos.DrawLine(transform.position, right.transform.position);
    }
}