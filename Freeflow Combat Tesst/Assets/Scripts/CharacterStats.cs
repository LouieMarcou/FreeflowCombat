using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/Characters", order = 1)]
public class CharacterStats : ScriptableObject
{
    public float Health;
    public float Damage;
}
