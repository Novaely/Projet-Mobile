using UnityEngine;

[CreateAssetMenu(menuName = "Rules/T-Rex Needs Herbivore")]
public class TRexNeedsHerbivoreSO : SeatRuleSO
{
    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null && neighbor.occupant.diet == DietType.Herbivore)
                return true;
        }
        return false;
    }
}
