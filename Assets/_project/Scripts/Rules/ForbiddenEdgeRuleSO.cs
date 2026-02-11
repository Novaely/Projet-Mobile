using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Forbidden Edge")]
public class ForbiddenEdgeRuleSO : SeatRuleSO
{
    [Tooltip("La direction où le dino refuse d'être collé au bord (ex: refuse d'être tout devant)")]
    public NeighborDirection forbiddenEdge;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        switch (forbiddenEdge)
        {
            case NeighborDirection.Devant:
                if (seat.front == null) return false; 
                break;
            case NeighborDirection.Derriere:
                if (seat.back == null) return false;  
                break;
            case NeighborDirection.Gauche:
                if (seat.left == null) return false;
                break;
            case NeighborDirection.Droite:
                if (seat.right == null) return false;
                break;
        }
        
        return true;
    }
}