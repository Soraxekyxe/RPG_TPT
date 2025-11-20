using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SavedData
{
    public int[] IDs_Skins_Unlocked;
    public int[] IDs_Skins_locked;
    // coins
    public SavedData(Gacha gacha, Inventaire inventaire)
    {
        IDs_Skins_Unlocked = gacha.ID_Dump().ToArray();
        IDs_Skins_locked = inventaire.ID_Dump().ToArray();
    }
}