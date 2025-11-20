using System;
using UnityEngine;

[Serializable]
public struct Stats
{
    public int PV, Mana, Agi, For, Int, Def;
    public static Stats Zero => new Stats();
}

public enum SlimeClass { Guerrier, Mage, Assassin, Clerc, Druide }
public enum DamageKind { Physical, Magical, True } // True = ignore DEF
public enum TargetMode { Self, AllySingle, AllyAll, EnemySingle, EnemyAll }