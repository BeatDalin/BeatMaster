using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "SpritesForm", fileName = "Level")]
public class ChangingResources : ScriptableObject
{
    [SerializeField] private Material[] changingMaterials;
    public Material[] ChangingMaterials { get => changingMaterials; private set => changingMaterials = value; }
    [SerializeField] private Monster[] _changingMonsters;
    public Monster[] ChangingMonsters { get => _changingMonsters; private set => _changingMonsters = value; }
    [SerializeField] private ParticleSystem[] _changingParticles;
    public ParticleSystem[] ChangingParticles { get => _changingParticles; private set => _changingParticles = value; }
}
