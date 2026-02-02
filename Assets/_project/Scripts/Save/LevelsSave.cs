using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsSave", menuName = "ScriptableObject/LevelSave", order = 0)]
public class LevelsSave : ScriptableObject
{
    public List<int> starsLevels;
}

public class LevelsSaveData
{
    public List<int> starsLevels;
}