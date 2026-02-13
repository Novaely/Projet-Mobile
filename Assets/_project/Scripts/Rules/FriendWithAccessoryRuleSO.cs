using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Friend With Accessory")]
public class FriendWithAccessoryRuleSO : SeatRuleSO
{
    [Tooltip("Le tag de l'accessoire que le voisin doit porter")]
    public string targetAccessory; 

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null)
            {
                if (neighbor.occupant.profile != null && 
                    neighbor.occupant.profile.accessoryTag == targetAccessory)
                {
                    return true;
                }
            }
        }
        return false;
    }
}