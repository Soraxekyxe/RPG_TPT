using UnityEngine;

public class ItemRuntime
{
    public BaseItemSO def;

    // flags / counters pour les effets
    public bool flagA, flagB;
    public int stacksA, stacksB;

    // ✅ CONSTRUCTEUR UNIQUE
    public ItemRuntime(BaseItemSO def)
    {
        this.def = def;
    }
}