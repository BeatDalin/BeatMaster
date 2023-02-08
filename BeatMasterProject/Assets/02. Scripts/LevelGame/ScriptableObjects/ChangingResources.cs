using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "SpritesForm", fileName = "Level")]
public class ChangingResources : ScriptableObject
{
    [SerializeField] private Material[] _changingMaterials;
    public Material[] ChangingMaterials { get => _changingMaterials; private set => _changingMaterials = value; }
    [SerializeField] private Monster[] _changingMonsters;
    public Monster[] ChangingMonsters { get => _changingMonsters; private set => _changingMonsters = value; }
    [SerializeField] private ParticleSystem[] _changingParticles;
    public ParticleSystem[] ChangingParticles { get => _changingParticles; private set => _changingParticles = value; }
}
