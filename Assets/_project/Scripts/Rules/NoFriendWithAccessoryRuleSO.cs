using UnityEngine;

[CreateAssetMenu(menuName = "DinoSystem/Rules/No Friend With Accessory")]
public class NoFriendWithAccessoryRuleSO : SeatRuleSO
{
    [Tooltip("Le tag de l'accessoire que le voisin ne doit PAS porter")]
    public string forbiddenAccessory;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        if (string.IsNullOrWhiteSpace(forbiddenAccessory)) return true;

        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null)
            {
                if (neighbor.occupant.profile != null)
                {
                    string neighborTag = neighbor.occupant.profile.accessoryTag;

                    if (string.IsNullOrWhiteSpace(neighborTag)) continue;

                    if (string.Equals(neighborTag.Trim(), forbiddenAccessory.Trim(), System.StringComparison.OrdinalIgnoreCase))
                    {
                        return false; 
                    }
                }
            }
        }
        return true; 
    }
}