using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DicoDino", menuName = "Scriptable Objects/DicoDino")]
public class DicoDinoDatabase : ScriptableObject
{
    [field: SerializeField] public List<DicoDino> Dictionnary { get; private set; }
}

[Serializable]
public struct DicoDino
{
    public string DinoWord;
    public string KeyUniversalTraduction;
    public int UnlockedAtLevel;
}