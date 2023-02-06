using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpritesForm", fileName = "Level")]
public class ChangingSprites : ScriptableObject
{
    [SerializeField] private Material[] _changingMaterials;
    public Material[] ChangingMaterials { get => _changingMaterials; private set => _changingMaterials = value; }
    [SerializeField] private GameObject[] _changingEnemies;
    public GameObject[] ChangingEnemies { get => _changingEnemies; private set => _changingEnemies = value; }
    [SerializeField] private ParticleSystem[] _changingParticles;
    public ParticleSystem[] ChangingParticles { get => _changingParticles; private set => _changingParticles = value; }
}
