using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField]
    private Button _char;

    [SerializeField]
    private Button _itemBtn;

    [SerializeField]
    private GameObject[] _popupPanel; //panels 0:_purchasePanel, 1:_noMoneyPanel, 2: _lockedPanel
    [SerializeField]
    private Button[] _popupBtn; //buttons 0:_purchaseBtn, 1:_closeBtn
    [SerializeField]
    private Text _playerCoin;
    [SerializeField]
    private GameObject _ifPurchased;
    [SerializeField]
    private Anim _anim;

    [SerializeField]
    private ChangeCharSprite _changeChar;

    [SerializeField] private Toggle[] _toggles; //0: charBtn, 1:itemBtn

    //private Animator _animator;

    [SerializeField]
    private GameObject[] _selectArea;

    [SerializeField]
    private GameObject _charContent;

    [SerializeField]
    private GameObject _itemContent;

    private Button[] _character;
    private Button[] _item;
    [SerializeField] private StoreData _storeData;
    [SerializeField] private PlayerData _playerData;

    private void Awake()
    {
        DataCenter.Instance.LoadData();
    }

    private void Start()
    {
        //_animator = _popupPanel[0].transform.GetChild(0).GetComponent<Animator>();
        UpdatePlayersDataInScene();
        _storeData = DataCenter.Instance.GetStoreData();
        _playerData = DataCenter.Instance.GetPlayerData();
        SetCharBtn();
        SetItemBtn();

        ShowStoreList(0);
        _toggles[0].onValueChanged.AddListener(delegate { ShowStoreList(0); });
        _toggles[1].onValueChanged.AddListener(delegate { ShowStoreList(1); });
        ClosePanel();
    }

    private void SetCharBtn()
    {
        _character = new Button[_storeData.characterData.Length];
        for (int i = 0; i < _storeData.characterData.Length; i++)
        {
            int index = i;
            
            _character[index] = Instantiate(_char, _charContent.transform);
            _character[index].transform.GetComponent<Image>().sprite = _changeChar.ChangeCharacterSprite(index);
            if (_storeData.characterData[i].isPurchased)
            {
                _character[index].transform.GetChild(0).gameObject.SetActive(false);
                _character[index].transform.GetChild(1).gameObject.SetActive(false);
            }
            _character[index].transform.GetChild(0).GetComponent<Text>().text =
                _storeData.characterData[i].price.ToString();
            if (_storeData.characterData[i].isUnlocked)
            {
                _character[index].onClick.AddListener(() => SetPurchasePopup(index));
            }
            else
            {
                _character[index].onClick.AddListener(() => SetTextInUnlockChar(index));
            }
        }
    }

    private void SetTextInUnlockChar(int index)
    {
        _popupPanel[2].SetActive(true);
        _popupPanel[2].transform.GetChild(0).GetChild(1).GetComponent<Text>().text =
                    (_storeData.characterData[index].unlockStage + 1) + " - " + (_storeData.characterData[index].unlockLevel + 1) + "을 깨고 와주세요!";
    }

    private void SetItemBtn()
    {
        _item = new Button[_storeData.itemData.Length];
        for (int i = 0; i < _storeData.itemData.Length - 1; i++)
        {
            int index = i;

            _item[index] = Instantiate(_itemBtn, _itemContent.transform);
            _item[index].transform.GetChild(0).GetComponent<Image>().sprite = _changeChar.ChangeItemSprite((StoreData.ItemName)index);
            _item[index].transform.GetChild(1).GetChild(0).GetComponent<Text>().text =
                _storeData.itemData[index].price.ToString();
            if (_storeData.itemData[index].isPurchased)
            {
                _item[index].transform.GetChild(1).gameObject.SetActive(false);
            }
            if (_storeData.itemData[index].isUnlocked)
            {
                _item[index].transform.GetChild(2).gameObject.SetActive(false);
                _item[index].onClick.AddListener(() =>
                    SetItemPopup(_storeData.itemData[index].itemPart, _storeData.itemData[index].itemName));
            }
        }
        _changeChar.ChangeItemInItemScroll(_playerData);
    }

    private void ChangeCharacterInItemScroll()
    {
        _selectArea[1].transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = _changeChar.ChangeCharacterSprite(_playerData.playerChar);
    }

    // 선택한 탭에 따라 캐릭터/아이템 리스트를 보여준다.
    private void ShowStoreList(int toggleNum)
    {
        _selectArea[toggleNum].SetActive(true);
        _selectArea[toggleNum - 1 < 0 ? 1 : 0].SetActive(false);
        _playerData = DataCenter.Instance.GetPlayerData();
        ChangeCharacterInItemScroll();
    }

    private void InitStorePopup()
    {
        _storeData = DataCenter.Instance.GetStoreData();
        _playerData = DataCenter.Instance.GetPlayerData();
        _ifPurchased.transform.GetChild(0).GetComponent<Button>().interactable = true;
        _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "구매하기";
        _popupBtn[0].onClick.RemoveAllListeners();
        _changeChar.ChangeItemInItemScroll(_playerData);
        _ifPurchased.SetActive(true);
        _popupPanel[0].SetActive(false);
        for (int i = 0; i < _storeData.characterData.Length; i++)
        {
            if (_storeData.characterData[i].isPurchased)
            {
                _character[i].transform.GetChild(0).gameObject.SetActive(false);
                _character[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < _storeData.itemData.Length - 1; i++)
        {
            if (_storeData.itemData[i].isPurchased)
            {
                _item[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    #region Character

    // 해금되었을 때만 호출
    private void SetPurchasePopup(int charNum)
    {
        _popupPanel[0].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);

        _popupPanel[0].transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = ((Image)_character[charNum].targetGraphic).sprite;
        _popupPanel[0].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        
        // 구매 안 한 상태일 때 구매하기 버튼 노출
        if (!_storeData.characterData[charNum].isPurchased && _storeData.characterData[charNum].isUnlocked)
        {
            _ifPurchased.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = _storeData.characterData[charNum].price.ToString();
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(true);

            _popupBtn[0].onClick.AddListener(delegate { PurchaseCharacter(charNum); });
        }

        // 구매했는데 미장착 상태일 때 장착하기 버튼
        else if (_storeData.characterData[charNum].isPurchased && charNum != DataCenter.Instance.GetPlayerData().playerChar)
        {
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(false);
            _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "장착하기";

            _popupBtn[0].onClick.AddListener(delegate { EquipCharacter(charNum); });
        }

        // 구매했는데 장착 상태일 때
        else
        {
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(false);
            _ifPurchased.transform.GetChild(0).GetComponent<Button>().interactable = false;
            _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "장착 중";
        }
        _popupPanel[0].SetActive(true);
        _anim.ChangeCharacterAnim(charNum);
    }

    private void PurchaseCharacter(int charNum)
    {
        int price = _storeData.characterData[charNum].price;
        if (price > DataCenter.Instance.GetPlayerData().playerItem)
        {
            _popupPanel[1].SetActive(true);
            return;
        }

        DataCenter.Instance.UpdateCharacterPurchaseData(charNum);

        UpdatePlayersDataInScene();

        InitStorePopup();
    }

    private void EquipCharacter(int charNum)
    {
        DataCenter.Instance.UpdatePlayerCharData(charNum);

        InitStorePopup();
    }

    #endregion

    #region Item

    private void SetItemPopup(StoreData.ItemPart itemPart, StoreData.ItemName itemName)
    {
        int itemNum = (int)itemName;
        _popupPanel[0].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        _popupPanel[0].transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = _item[itemNum].transform.GetChild(0).GetComponent<Image>().sprite;
        _popupPanel[0].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

        // 구매 안 한 상태일 때 구매하기 버튼 노출
        if (!_storeData.itemData[itemNum].isPurchased && _storeData.itemData[itemNum].isUnlocked)
        {
            _ifPurchased.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = _storeData.itemData[itemNum].price.ToString();
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(true);

            _popupBtn[0].onClick.AddListener(delegate { PurchaseItem(itemPart, itemName); });
        }

        // 구매했는데 미장착 상태일 때 장착하기 버튼
        else if (_storeData.itemData[itemNum].isPurchased && itemNum != DataCenter.Instance.GetPlayerData().itemData[(int)itemPart])
        {
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(false);
            _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "장착하기";

            _popupBtn[0].onClick.AddListener(delegate { EquipItem(itemPart, itemName); });
        }

        // 구매했는데 장착 상태일 때
        else
        {
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(false);
            _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "해제하기";

            _popupBtn[0].onClick.AddListener(delegate { EquipItem(itemPart, (StoreData.ItemName)(-1)); });
        }
        _popupPanel[0].SetActive(true);
    }

    private void PurchaseItem(StoreData.ItemPart itemPart, StoreData.ItemName itemName)
    {
        int itemNum = (int)itemName;
        int price = _storeData.itemData[itemNum].price;

        if (price > DataCenter.Instance.GetPlayerData().playerItem)
        {
            _popupPanel[1].SetActive(true);
            return;
        }

        DataCenter.Instance.UpdateItemPurchaseData(itemName);

        UpdatePlayersDataInScene();

        InitStorePopup();
    }

    private void EquipItem(StoreData.ItemPart itemPart, StoreData.ItemName itemName)
    {
        DataCenter.Instance.UpdatePlayerItemData(itemPart, itemName);

        InitStorePopup();
    }

    private void UpdatePlayersDataInScene()
    {
        _playerCoin.text = DataCenter.Instance.GetPlayerData().playerItem.ToString();
    }

    private void ClosePanel()
    {
        foreach (var button in _popupBtn)
        {
            button.onClick.AddListener(() => InitStorePopup());
        }
    }

    #endregion

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
