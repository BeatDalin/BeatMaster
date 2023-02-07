using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField]
    private Button[] _character;
    //private Sprite[] _sprite;
    [SerializeField]
    private int[] _price;
    private int _coin;
    [SerializeField]
    private GameObject[] _popupPanel; //panels 0:_purchasePanel, 1:_noMoneyPanel
    [SerializeField]
    private Button[] _popupBtn; //buttons 0:_purchaseBtn, 1:_closeBtn
    [SerializeField]
    private Text _playerCoin;
    [SerializeField]
    private Data _gameData;

    //구매 여부 판단 변수 
    [SerializeField]
    private bool[] _isPurchased;
    [SerializeField]
    private GameObject _ifPurchased;
    //해금여부
    //private bool[] _isUnlock;

    //private int _currentChar;
    [SerializeField]
    private Dropdown _dropdown;
    private Animator _animator;
    [SerializeField]
    private AnimationClip[] _anims;

    [SerializeField]
    private RuntimeAnimatorController[] _animatorController;
    private void Start()
    {
        _animator = _popupPanel[0].transform.GetChild(0).GetComponent<Animator>();
        _coin = 1000;
        UpdatePlayersCoinData();
        for (int i = 0; i < _character.Length; i++)
        {
            int index = i;
            _character[index].onClick.AddListener(() => SetPurchasePopup(index));
        }
        _dropdown.onValueChanged.AddListener(delegate { ChangeDropdownValue(_dropdown); });
        ClosePanel();
    }


    private void InitStorePopup()
    {
        foreach (var button in _popupBtn)
        {
            button.onClick.RemoveAllListeners();
        }
        _ifPurchased.SetActive(true);
        _popupPanel[0].SetActive(false);
        //_currentChar = -1;
        _dropdown.value = 0;
        ClosePanel();
    }

    private void SetPurchasePopup(int charNum)
    {
        _popupPanel[0].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ((Image)_character[charNum].targetGraphic).sprite;
        _popupPanel[0].SetActive(true);
        if (!_isPurchased[charNum])
        {
            //_currentChar = charNum;
            _ifPurchased.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = _price[charNum].ToString();
            UnityAction UA;
            UA = new UnityAction(() => PurchaseCharacter(charNum));
            _popupBtn[0].onClick.AddListener(UA);
        }
        else
        {
            _ifPurchased.SetActive(false);
        }
        _animator.runtimeAnimatorController = _animatorController[charNum];
    }

    private void PurchaseCharacter(int charNum)
    {
        int price = _price[charNum];
        if (price > _coin)
        {
            _popupPanel[1].SetActive(true);
            return;
        }

        _coin -= price;
        _isPurchased[charNum] = true;

        UpdatePlayersCoinInScene();
    }

    private void UpdatePlayersCoinData()
    {
        //_coin = _gameData.playerItem;
        _playerCoin.text = _coin.ToString();
    }

    private void UpdatePlayersCoinInScene()
    {
        _playerCoin.text = _coin.ToString();
        _gameData.playerItem = _coin;
    }

    private void ClosePanel()
    {
        foreach (var button in _popupBtn)
        {
            button.onClick.AddListener(() => InitStorePopup());
        }
    }

    private void ChangeDropdownValue(Dropdown select)
    {
        int op = select.value;
        _animator.Play(Enum.GetName(typeof(Status), op));
    }

    #region ifFix
    /*
     *     private void OnEnable()
        {
            SetAnimationLoop(false);
        }

        private void OnDisable()
        {
            SetAnimationLoop(true);
        }
     * 
     * 
     * private void SetAnimationLoop(bool isLoop)
        {
            foreach(var _anim in _anims)
            {
                _anim.wrapMode = isLoop?WrapMode.Default : WrapMode.Loop;
            }
        }*/
    #endregion
}
