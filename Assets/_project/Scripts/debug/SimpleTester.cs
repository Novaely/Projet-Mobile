using UnityEngine;

public class SimpleTester : MonoBehaviour
{
    public ConditionManager manager;
    public Dino dinoATester;
    public Seat siegeVise;

    [ContextMenu("Essayer de Poser")]
    public void TestPlace()
    {
        Color couleurFeedback = manager.GetHighlightColor(dinoATester, siegeVise);
        
        var renderer = siegeVise.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = couleurFeedback;
        }

        if (manager.CanPlace(dinoATester, siegeVise))
        {
            Debug.Log("<color=green>SUCCÈS</color>");
        }
        else
        {
            Debug.Log("<color=red>ÉCHEC</color>");
        }
    }
    
    [ContextMenu("Reset Couleur")]
    public void ResetColor()
    {
        var renderer = siegeVise.GetComponent<SpriteRenderer>();
        if(renderer) renderer.color = new Color(1f,1f,1f,0.3f);
    }
}
