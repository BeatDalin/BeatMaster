using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class ComboSystem : MonoBehaviour
{
    [SerializeField] private int _increasingAmount = 10000;
    [SerializeField] private GameObject _comboTextPrefab; 
    [SerializeField] private ushort _combo;
    
    
    private int _currentAmount;
    private Transform _characterTransform;

    // 확인용 SerializeField
    private float _timer;

    private void Awake()
    {
        _characterTransform = FindObjectOfType<CharacterMovement>().transform;
    }

    void Start()
    {
        _currentAmount = _increasingAmount;
    }

    public void IncreaseCombo()
    {
        _combo++;
        ShowCombo();
    }

    public void IncreaseComboInProcess(int startSample)
    {
        int currentSampleTime = SoundManager.instance.playingKoreo.GetLatestSampleTime();
        if (_currentAmount + startSample < currentSampleTime)
        {
            IncreaseCombo();
            _currentAmount += _increasingAmount;
        }
    }
    
    public void ResetCombo()
    {
        _combo = 0;
        ResetCurrentAmount();
        // 콤보가 끊기는 연출
    }

    public void ResetCurrentAmount()
    {
        _currentAmount = _increasingAmount;
    }

    private void ShowCombo()
    {
        // 콤보 UI를 보여줌
        GameObject comboGo = Instantiate(_comboTextPrefab, _characterTransform.position, Quaternion.Euler(Vector3.zero), transform);
        ComboText comboText = comboGo.GetComponent<ComboText>(); 
        comboText.SetText(_combo);
    }

    
    
}
