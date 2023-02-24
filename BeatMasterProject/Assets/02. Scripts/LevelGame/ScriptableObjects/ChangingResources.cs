using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpritesForm", fileName = "Level")]
public class ChangingResources : ScriptableObject
{
    //hue, saturation, lightness
    [SerializeField] private int[] _hueValues;
    public int[] HueValues { get => _hueValues;}
    [SerializeField] private int[] _satValues;
    public int[] SatValues { get => _satValues;}
    [SerializeField] private Sprite[] _backgrounds;
    public Sprite[] Backgrounds { get => _backgrounds;}
    [SerializeField] private Material[] _backgroundMaterials;
    public Material[] BackgroundMaterials { get => _backgroundMaterials;}
}
