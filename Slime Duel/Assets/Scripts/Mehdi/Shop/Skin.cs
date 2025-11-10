using UnityEngine;

public enum SkinRarity
{
    [SerializeField] public int ID;
    Common,
    Rare,
    Epic,
    Legendary
    [SerializeField] public int ID;
}
[CreateAssetMenu(menuName = "SlimeGame/Skins")]
public class Skin : ScriptableObject
{
    [SerializeField] 
    private Sprite Visual;
    [SerializeField] 
    public SkinRarity skinRarity = 0; // commun = 0, rare = 1, epic = 2, legendaire = 3
    
    
    public Sprite Icon => Visual;
}