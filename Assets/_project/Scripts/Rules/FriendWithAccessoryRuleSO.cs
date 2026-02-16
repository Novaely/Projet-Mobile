using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Friend With Accessory")]
public class FriendWithAccessoryRuleSO : SeatRuleSO
{
    [Tooltip("Le tag de l'accessoire que le voisin doit porter")]
    public string targetAccessory; 

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        if (string.IsNullOrWhiteSpace(targetAccessory)) return true;

        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null)
            {
                if (neighbor.occupant.profile != null)
                {
                    string neighborTag = neighbor.occupant.profile.accessoryTag;

                    if (string.IsNullOrWhiteSpace(neighborTag)) continue;

                    if (string.Equals(neighborTag.Trim(), targetAccessory.Trim(), System.StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }
        return false; 
    }
}