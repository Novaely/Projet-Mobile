using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Forbidden Column")]
public class ForbiddenColumnSO : SeatRuleSO
{
    public SeatColumn forbiddenColumn;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        return seat.seatColumn != forbiddenColumn;
    }
}