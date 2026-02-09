using UnityEngine;

public abstract class SeatRuleSO : ScriptableObject
{
    public abstract bool IsSatisfied(Dino dino, Seat seat);
}
