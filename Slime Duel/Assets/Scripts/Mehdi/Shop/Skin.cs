using System.Net.Mime;
using UnityEngine;

public class Skin : MonoBehaviour
{
    [SerializeField] private Sprite Visual;
    [SerializeField] [Header("0 = commun, rare = 1, epic = 2, legendaire = 3")] public int rarity = 0; // commun = 0, rare = 1, epic = 2, legendaire = 3
}
