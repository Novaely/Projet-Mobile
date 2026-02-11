using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Must Be In Front Row")]
public class MustBeInFrontRowRuleSO : SeatRuleSO
{
    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        return seat.front == null; 
    }
}