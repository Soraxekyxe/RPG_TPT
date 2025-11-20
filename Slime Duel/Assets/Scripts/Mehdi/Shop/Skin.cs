using UnityEngine;
[CreateAssetMenu(menuName = "SlimeGame/Skins")]

public class Skin : ScriptableObject
{
    [SerializeField] public Sprite Visual;
    [SerializeField] public int ID;
    public enum SkinRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    public SkinRarity MySkinRarity = SkinRarity.Common;
    public Sprite Icon => Visual;
}