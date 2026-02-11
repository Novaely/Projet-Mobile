using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Forbidden Row")]
public class ForbiddenRowSO : SeatRuleSO
{
    public SeatRow forbiddenRow;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        return seat.seatRow != forbiddenRow;
    }
}