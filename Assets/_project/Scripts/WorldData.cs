using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/WorldData")]
public class WorldData : ScriptableObject
{
    public List<World> Worlds;

    [Serializable]
    public class World
    {
        public string label;
        public int levelInThisWorld;
        public int numberStarNeed;
    }
}
