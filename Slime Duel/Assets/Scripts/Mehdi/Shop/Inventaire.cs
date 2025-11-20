using System;
using UnityEngine;
using System.Collections.Generic;

public class Inventaire : MonoBehaviour
{
    [SerializeField] public Gacha Gacha_Machine;
    public List<Skin> Skin_Debloque = new List<Skin>();
    [SerializeField] private SkinLoadData DataLoader;

    void Start()
    {
        DataLoader.Load();
    }


    public void try_pull()
    {
        Skin unlocked_skin = Gacha_Machine.pull();
        Skin_Debloque.Add(unlocked_skin);
        Debug.Log("new skin: " + unlocked_skin.name);
        Debug.Log("current skins unlocked: " + Skin_Debloque.Count);
        SaveSystem.SaveAllData(Gacha_Machine, this);
        Debug.Log("sauvegarde terminé");
    }

    public void try_DeleteSavedData()
    {
        DataLoader.DeleteData();
    }

    public List<int> ID_Dump()
    {
        List<int> IDs_Skins_Unlocked = new List<int>{};
        foreach (var i in Skin_Debloque)
        {
            IDs_Skins_Unlocked.Add(i.ID);
        }

        return IDs_Skins_Unlocked;
    }
}