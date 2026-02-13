using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Must Be Empty Direction")]
public class MustBeEmptyDirectionRuleSO : SeatRuleSO
{
    [Tooltip("La direction qui doit obligatoirement être vide")]
    public NeighborDirection directionToCheck;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        Seat targetSeat = null;

        switch (directionToCheck)
        {
            case NeighborDirection.Devant: targetSeat = seat.front; break;
            case NeighborDirection.Derriere: targetSeat = seat.back; break;
            case NeighborDirection.Gauche: targetSeat = seat.left; break;
            case NeighborDirection.Droite: targetSeat = seat.right; break;
        }

        if (targetSeat != null && targetSeat.occupant != null)
        {
            return false;
        }

        return true;
    }
}