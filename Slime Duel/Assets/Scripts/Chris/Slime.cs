using UnityEngine;

public enum SlimeClass
{
    Guerrier,
    Mage,
    Assassin,
    Clerc,
    Druide
}

public class Slime : MonoBehaviour
{
    [Header("Choix de la classe")]
    public SlimeClass classe;

    [Header("Bonus de stats (ajoutés aux stats de base)")]
    public int bonusPV;
    public int bonusMana;
    public int bonusAgi;
    public int bonusFor;
    public int bonusInt;
    public int bonusDef;

    // Stats finales (runtime, pas visible dans l’inspector)
    [HideInInspector] public int PV;
    [HideInInspector] public int Mana;
    [HideInInspector] public int Agi;
    [HideInInspector] public int For;
    [HideInInspector] public int Int;
    [HideInInspector] public int Def;

    void Start()
    {
        InitialiserStats();
        Debug.Log($"{gameObject.name} ({classe}) → PV:{PV} Mana:{Mana} Agi:{Agi} For:{For} Int:{Int} Def:{Def}");
    }

    void InitialiserStats()
    {
        // Stats de base selon la classe
        switch (classe)
        {
            case SlimeClass.Guerrier:
                PV = 70; Mana = 10; Agi = 8; For = 15; Int = 5; Def = 10;
                break;
            case SlimeClass.Mage:
                PV = 50; Mana = 40; Agi = 9; For = 4; Int = 16; Def = 4;
                break;
            case SlimeClass.Assassin:
                PV = 55; Mana = 20; Agi = 16; For = 12; Int = 6; Def = 5;
                break;
            case SlimeClass.Clerc:
                PV = 60; Mana = 35; Agi = 8; For = 7; Int = 15; Def = 7;
                break;
            case SlimeClass.Druide:
                PV = 65; Mana = 30; Agi = 10; For = 8; Int = 12; Def = 8;
                break;
        }

        // Ajoute les bonus du joueur
        PV += bonusPV;
        Mana += bonusMana;
        Agi += bonusAgi;
        For += bonusFor;
        Int += bonusInt;
        Def += bonusDef;
    }
}