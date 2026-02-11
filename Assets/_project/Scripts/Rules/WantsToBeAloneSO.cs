using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Wants To Be Alone")]
public class WantsToBeAloneSO : SeatRuleSO
{
    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null)
                return false;
        }
        return true;
    }
}
