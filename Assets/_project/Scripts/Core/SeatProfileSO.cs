using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DinoSystem/Seat Profile")]
public class SeatProfileSO : ScriptableObject
{
    public Seat seat;
    public List<SeatRuleSO> rules = new List<SeatRuleSO>();
}
