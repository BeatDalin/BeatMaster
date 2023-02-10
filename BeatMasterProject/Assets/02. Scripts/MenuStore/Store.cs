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
    private GameObject[] _popupPanel; //panels 0:_purchasePanel, 1:_noMoneyPanel
    [SerializeField]
    private Button[] _popupBtn; //buttons 0:_purchaseBtn, 1:_closeBtn
    [SerializeField]
    private Text _playerCoin;
    [SerializeField]
    private GameObject _ifPurchased;

    //private int _currentChar;
    [SerializeField]
    private Dropdown _dropdown;
    [SerializeField]
    private Anim _anim;
    //private Animator _animator;
    /*    [SerializeField]
        private AnimationClip[] _anims;*/
    /*    [SerializeField]*/
    //private Data _gameData;

    //private StoreData _currentStoreData;

    private int[] _price;
    //구매 여부 판단 변수 
    private bool[] _isPurchased;
    private bool[] _isUnlocked;
    //private PlayerData _playerDatas; //0: playerStage, 1: playerLv, 2:playerItem(coin) 3:playerChar

    private void Awake()
    {
        DataCenter.Instance.LoadData();
        //_currentStoreData = DataCenter.Instance.GetStoreData();
        //각 변수에 맞는 데이터들 받아오기
    }

    private void Start()
    {
        //_animator = _popupPanel[0].transform.GetChild(0).GetComponent<Animator>();
        _isPurchased = new bool[DataCenter.Instance.GetStoreData().characterData.Length];
        _isUnlocked = new bool[DataCenter.Instance.GetStoreData().characterData.Length];
        _price = new int[DataCenter.Instance.GetStoreData().characterData.Length];

        for (int i = 0; i < DataCenter.Instance.GetStoreData().characterData.Length; i++)
        {
            _isPurchased[i] = DataCenter.Instance.GetStoreData().characterData[i].isPurchased;
            _isUnlocked[i] = DataCenter.Instance.GetStoreData().characterData[i].isUnlocked;
            _price[i] = DataCenter.Instance.GetStoreData().characterData[i].price;
        }
        //_playerChar = _gameData.playerChar;

        UpdatePlayersDataInScene();
        for (int i = 0; i < _character.Length; i++)
        {
            if (!_isUnlocked[i])
            {
                continue;
            }
            
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

    // 해금되었을 때만 호출
    private void SetPurchasePopup(int charNum)
    {
        _popupPanel[0].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ((Image)_character[charNum].targetGraphic).sprite;
        _popupPanel[0].SetActive(true);
        
        // 구매 안 한 상태일 때 구매하기 버튼 노출
        if (!_isPurchased[charNum] && _isUnlocked[charNum])
        {
            //_currentChar = charNum;
            _ifPurchased.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = _price[charNum].ToString();
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(true);

            UnityAction UA;
            UA = new UnityAction(() => PurchaseCharacter(charNum));
            _popupBtn[0].onClick.AddListener(UA);
        }
        
        // 구매했는데 미장착 상태일 때 장착하기 버튼
        else if (_isPurchased[charNum] && charNum != DataCenter.Instance.GetPlayerData().playerChar)
        {
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(false);
            _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "장착하기";
            
            UnityAction UA;
            UA = new UnityAction(() => EquipCharacter(charNum));
            _popupBtn[0].onClick.AddListener(UA);
        }
            
        // 구매했는데 장착 상태일 때
        else
        {
            _ifPurchased.SetActive(false);
        }
        _anim.ChangeCharacterAnim(charNum);
    }

    private void PurchaseCharacter(int charNum)
    {
        int price = _price[charNum];
        if (price > DataCenter.Instance.GetPlayerData().playerItem)
        {
            _popupPanel[1].SetActive(true);
            return;
        }

        DataCenter.Instance.UpdateStorePurchaseData(charNum);

        UpdatePlayersDataInScene();
        
        SetPurchasePopup(charNum);
    }

    private void EquipCharacter(int charNum)
    {
        DataCenter.Instance.UpdatePlayerCharData(charNum);
    }

    private void UpdatePlayersDataInScene()
    {
        _playerCoin.text = DataCenter.Instance.GetPlayerData().playerItem.ToString();
        for (int i = 0; i < DataCenter.Instance.GetStoreData().characterData.Length; i++)
        {
            _isPurchased[i] = DataCenter.Instance.GetStoreData().characterData[i].isPurchased;
            _isUnlocked[i] = DataCenter.Instance.GetStoreData().characterData[i].isUnlocked;
        }
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
        //_animator.Play(Enum.GetName(typeof(CharacterStatus), op));
        PlayerStatus.Instance.ChangeStatus((CharacterStatus)op);
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
