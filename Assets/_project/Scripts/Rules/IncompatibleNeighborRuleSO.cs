using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Incompatible Neighbor")]
public class IncompatibleNeighborRuleSO : SeatRuleSO
{
    [Tooltip("Le nom de l'espèce qu'il déteste (ex: T-Rex)")]
    public string hatedDinoName;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null)
            {
                // On vérifie le nom au lieu de la couleur
                if (neighbor.occupant.dinoName == hatedDinoName)
                    return false; 
            }
        }
        return true;
    }
}