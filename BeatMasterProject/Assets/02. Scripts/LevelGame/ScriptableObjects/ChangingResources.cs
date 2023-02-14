using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "SpritesForm", fileName = "Level")]
public class ChangingResources : ScriptableObject
{
    //hue, saturation, lightness
    [SerializeField] private int[] _hueValues;
    public int[] HueValues { get => _hueValues; private set => _hueValues = value; }
    [SerializeField] private int[] _satValues;
    public int[] SatValues { get => _satValues; private set => _satValues = value; }
}
