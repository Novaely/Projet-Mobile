using UnityEngine;
using System.Collections.Generic;

public class Seat : MonoBehaviour
{
    [Header("Configuration")]
    public SeatType seatType = SeatType.Normal;

    [Header("État")]
    public Dino occupant;

    [Header("Voisins")]
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
}
