using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Forbidden Seat Type")]
public class ForbiddenSeatTypeRuleSO : SeatRuleSO
{
    [Tooltip("Le type de siège sur lequel le dino REFUSE d'être assis")]
    public SeatType forbiddenSeatType;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        return seat.seatType != forbiddenSeatType;
    }
}