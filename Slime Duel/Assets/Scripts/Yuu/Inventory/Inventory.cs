using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public static readonly Inventory Instance = new Inventory();
    
    private List<Skin> skins = new List<Skin>();

    public void AddSkin(Skin skin)
    {
        skins.Add(skin);
    }

    public void RemoveSkin(Skin skin)
    {
        skins.Remove(skin);
    }
    
    public IReadOnlyList<Skin> Skins => skins;
}
