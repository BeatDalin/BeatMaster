using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [Header("Prefab Buttons")]
    [SerializeField]
    private Button _characterBtn;
    [SerializeField]
    private Button _itemBtn;
    [SerializeField] 
    private Button _paidItemBtn;

    [Header("Panels")]
    [SerializeField]
    private GameObject[] _popupPanel; //panels 0:_purchasePanel, 1:_noMoneyPanel, 2: _lockedPanel

    [Header("Product Info")] 
    [SerializeField]
    private Sprite[] _moneySprite;
    [SerializeField] 
    private Text _productTitle;
    [SerializeField] 
    private Text _productDescription;
    

    [Header("Popup Buttons")]
    [SerializeField]
    private Button[] _popupBtn; //buttons 0:_purchaseBtn, 1:_closeBtn

    [Header("Text")]
    [SerializeField]
    private Text _playerCoin;

    [Header("Objects")]
    [SerializeField]
    private GameObject _ifPurchased;
    [SerializeField]
    private GameObject[] _selectArea;
    [SerializeField]
    private GameObject _charContent;
    [SerializeField]
    private GameObject _itemContent;
    [SerializeField] 
    private GameObject _paidItemContent;

    [SerializeField]
    private Anim _anim;

    [SerializeField]
    private ChangeCharSprite _changeChar;

    [Header("Toggles")]
    [SerializeField]
    private Toggle[] _toggles; //0: charBtn, 1:itemBtn, 2:paidItemBtn

    //private Animator _animator;

    private Button[] _character;
    private Button[] _item;
    private Button[] _paidItem;

    [Header("Data")]
    [SerializeField] private StoreData _storeData;
    [SerializeField] private PlayerData _playerData;


    [Header("DOTween Animations")] 
    [SerializeField] private DOTweenAnimation[] _popUpDOTweens;
    [SerializeField] private DOTweenAnimation[] _toggleDOTweens;
    [SerializeField] private DOTweenAnimation[] _selectAreaDOTweens;

    [SerializeField] private Button _closeStoreBtn;

    // readonly string
    private readonly string _equip = "Equip";
    private readonly string _unEquip = "UnEquip";
    private readonly string _equipped = "Equipped";
    private readonly string _buy = "Buy";
    
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
        SetPaidItemBtn();

        ShowStoreList(0);
        _toggles[0].onValueChanged.AddListener(delegate { ShowStoreList(0); });
        _toggles[1].onValueChanged.AddListener(delegate { ShowStoreList(1); });
        _toggles[2].onValueChanged.AddListener(delegate { ShowStoreList(2); });

        ClosePanel();
    }

    private void OnEnable()
    {
        for (int i = 0; i < 3; i++)
        {
            _toggles[i].isOn = i == 0;
        }
        ShowStoreList(0);
    }

    private void SetCharBtn()
    {
        _character = new Button[_storeData.characterData.Length];
        for (int i = 0; i < _storeData.characterData.Length; i++)
        {
            int index = i;

            _character[index] = Instantiate(_characterBtn, _charContent.transform);
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
        _closeStoreBtn.interactable = false;
        SoundManager.instance.PlaySFX("Touch");
        _popupPanel[2].SetActive(true);
        _popUpDOTweens[2].DORestart();
        _popupPanel[2].transform.GetChild(0).GetChild(1).GetComponent<Text>().text =
            $"Clear {_storeData.characterData[index].unlockStage + 1} - {_storeData.characterData[index].unlockLevel + 1} stage to unlock";
    }

    private void SetItemBtn()
    {
        _item = new Button[_storeData.itemData.Length];
        for (int i = 0; i < _storeData.itemData.Length - 1; i++)
        {
            int index = i;

            _item[index] = Instantiate(_itemBtn, _itemContent.transform);
            _item[index].transform.GetChild(0).GetComponent<Image>().sprite =
                _changeChar.ChangeItemSprite((StoreData.ItemName)index);
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

    private void SetPaidItemBtn()
    {
        _paidItem = new Button[_storeData.paidItemCount];
        for (int i = 0; i < _storeData.paidItemCount; i++)
        {
            int index = i;

            _paidItem[index] = Instantiate(_paidItemBtn, _paidItemContent.transform);
            _paidItem[index].transform.GetChild(0).GetComponent<Image>().sprite =
                _changeChar.ChangePaidItemSprite((StoreData.PaidItemName)index);
            _paidItem[index].transform.GetChild(1).GetChild(0).GetComponent<Text>().text =
                _storeData.paidItemData[index].price.ToString();
            if (_storeData.paidItemData[index].isPurchased)
            {
                _paidItem[index].transform.GetChild(1).gameObject.SetActive(false);
            }
            
            _paidItem[index].transform.GetChild(2).gameObject.SetActive(false);
            _paidItem[index].onClick.AddListener(() =>
                SetPaidItemPopup(_storeData.paidItemData[index].paidItemName));
        }
    }

    private void ChangeCharacterInItemScroll()
    {
        _selectArea[1].transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite =
            _changeChar.ChangeCharacterSprite(_playerData.playerChar);
    }

    // 선택한 탭에 따라 캐릭터/아이템 리스트를 보여준다.
    private void ShowStoreList(int toggleNum)
    {
        if (!_selectArea[toggleNum].activeSelf)
        {
            SoundManager.instance.PlaySFX("Touch");
            _toggleDOTweens[toggleNum].DORestart();
            _selectAreaDOTweens[toggleNum].DORestart();
            for (int i = 0; i < _toggles.Length; i++)
            {
                _selectArea[i].SetActive(i == toggleNum);
            }
            _playerData = DataCenter.Instance.GetPlayerData();
            InitStorePopup();
            ChangeCharacterInItemScroll();
        }
    }

    private void InitStorePopup()
    {
        _closeStoreBtn.interactable = true;
        SoundManager.instance.PlaySFX("Touch");
        _storeData = DataCenter.Instance.GetStoreData();
        _playerData = DataCenter.Instance.GetPlayerData();
        _ifPurchased.transform.GetChild(0).GetComponent<Button>().interactable = true;
        _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = _buy;
        _popupBtn[0].onClick.RemoveAllListeners();
        _changeChar.ChangeItemInItemScroll(_playerData);
        _ifPurchased.SetActive(true);

        for (int i = 0; i < _popupPanel.Length; i++)
        {
            _popupPanel[i].SetActive(false);
        }
        for (int i = 0; i < _storeData.characterData.Length; i++)
        {
            if (_storeData.characterData[i].isPurchased)
            {
                _character[i].transform.GetChild(0).gameObject.SetActive(false);
                _character[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < _storeData.itemData.Length - 1; i++) // empty 제외하기 위해 Length - 1
        {
            if (_storeData.itemData[i].isPurchased)
            {
                int index = i;
                _item[index].onClick.RemoveAllListeners();
                _item[index].onClick.AddListener(() =>
                    SetItemPopup(_storeData.itemData[index].itemPart, _storeData.itemData[index].itemName));
                _item[i].transform.GetChild(1).gameObject.SetActive(false);
                _item[i].transform.GetChild(2).gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < _storeData.paidItemData.Length; i++)
        {
            if (_storeData.paidItemData[i].isPurchased)
            {
                _paidItem[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    #region Character

    // 해금되었을 때만 호출
    private void SetPurchasePopup(int charNum)
    {
        _popupPanel[0].transform.GetChild(0).GetChild(1).gameObject.SetActive(false); // Item image

        _popupPanel[0].transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite =
            ((Image)_character[charNum].targetGraphic).sprite; // Square sprite
        _popupPanel[0].transform.GetChild(0).GetChild(0).gameObject.SetActive(true); // Square
        _productTitle.text = _storeData.characterData[charNum].characterName.ToString(); // CharacterName
        _productDescription.text = _storeData.characterData[charNum].characterDescription; // CharacterDescription
        
        // 구매 안 한 상태일 때 구매하기 버튼 노출
        if (!_storeData.characterData[charNum].isPurchased && _storeData.characterData[charNum].isUnlocked)
        {
            _ifPurchased.transform.GetChild(1).GetChild(0).GetComponent<Text>().text =
                _storeData.characterData[charNum].price.ToString(); // price text
            
            _ifPurchased.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite =
                _moneySprite[_storeData.characterData[charNum].isPaidItem ? 1 : 0]; // Money sprite
            
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(true); // SetActive price text, purchaseBtn
            
            _popupBtn[0].onClick.AddListener(delegate { PurchaseCharacter(charNum); });
        }

        // 구매했는데 미장착 상태일 때 장착하기 버튼
        else if (_storeData.characterData[charNum].isPurchased &&
                 charNum != DataCenter.Instance.GetPlayerData().playerChar)
        {
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(false);
            _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = _equip;

            _popupBtn[0].onClick.AddListener(delegate { EquipCharacter(charNum); });
        }

        // 구매했는데 장착 상태일 때
        else
        {
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(false);
            _ifPurchased.transform.GetChild(0).GetComponent<Button>().interactable = false;
            _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = _equipped;
        }

        _closeStoreBtn.interactable = false;
        SoundManager.instance.PlaySFX("Touch");
        _popupPanel[0].SetActive(true);
        _popUpDOTweens[0].DORestart();
        _anim.ChangeCharacterAnim(charNum);
    }

    private void PurchaseCharacter(int charNum)
    {
        int price = _storeData.characterData[charNum].price;
        if (price > DataCenter.Instance.GetPlayerData().playerItem)
        {
            _closeStoreBtn.interactable = false;
            SoundManager.instance.PlaySFX("Touch");
            _popupPanel[1].SetActive(true);
            _popUpDOTweens[1].DORestart();
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
        _popupPanel[0].transform.GetChild(0).GetChild(0).gameObject.SetActive(false); // Character image
        _popupPanel[0].transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite =
            _item[itemNum].transform.GetChild(0).GetComponent<Image>().sprite; // Item Sprite
        _popupPanel[0].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        _productTitle.text = _storeData.itemData[itemNum].itemName.ToString(); // Item Name
        _productDescription.text = _storeData.itemData[itemNum].itemDescription; // Item Description
        
        // 구매 안 한 상태일 때 구매하기 버튼 노출
        if (!_storeData.itemData[itemNum].isPurchased && _storeData.itemData[itemNum].isUnlocked)
        {
            _ifPurchased.transform.GetChild(1).GetChild(0).GetComponent<Text>().text =
                _storeData.itemData[itemNum].price.ToString(); // price text
            
            _ifPurchased.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite =
                _moneySprite[_storeData.itemData[itemNum].isPaidItem ? 1 : 0]; // Money sprite
            
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(true); // SetActive price text, purchaseBtn

            _popupBtn[0].onClick.AddListener(delegate { PurchaseItem(itemPart, itemName); });
        }

        // 구매했는데 미장착 상태일 때 장착하기 버튼
        else if (_storeData.itemData[itemNum].isPurchased &&
                 itemNum != DataCenter.Instance.GetPlayerData().itemData[(int)itemPart])
        {
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(false);
            _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = _equip;

            _popupBtn[0].onClick.AddListener(delegate { EquipItem(itemPart, itemName); });
        }

        // 구매했는데 장착 상태일 때
        else
        {
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(false);
            _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = _unEquip;

            _popupBtn[0].onClick.AddListener(delegate { EquipItem(itemPart, (StoreData.ItemName)(-1)); });
        }

        _closeStoreBtn.interactable = false;
        SoundManager.instance.PlaySFX("Touch");
        _popupPanel[0].SetActive(true);
        _popUpDOTweens[0].DORestart();
    }

    private void PurchaseItem(StoreData.ItemPart itemPart, StoreData.ItemName itemName)
    {
        int itemNum = (int)itemName;
        int price = _storeData.itemData[itemNum].price;

        if (price > DataCenter.Instance.GetPlayerData().playerItem)
        {
            _closeStoreBtn.interactable = false;
            _popupPanel[1].SetActive(true);
            _popUpDOTweens[1].DORestart();
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

    #region PaidItem
    
    private void SetPaidItemPopup(StoreData.PaidItemName paidItemName)
    {
        int itemNum = (int)paidItemName;
        _popupPanel[0].transform.GetChild(0).GetChild(0).gameObject.SetActive(false); // Character image 
        _popupPanel[0].transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite =
            _paidItem[itemNum].transform.GetChild(0).GetComponent<Image>().sprite; // Item Sprite
        _popupPanel[0].transform.GetChild(0).GetChild(1).gameObject.SetActive(true); // Item 
        _productTitle.text = _storeData.paidItemData[itemNum].paidItemName.ToString(); // paid Item Name
        _productDescription.text = _storeData.paidItemData[itemNum].paidItemDescription; // paid Item Description

        // 구매 안 한 상태일 때 구매하기 버튼 노출
        if (!_storeData.paidItemData[itemNum].isPurchased)
        {
            _ifPurchased.transform.GetChild(1).GetChild(0).GetComponent<Text>().text =
                _storeData.paidItemData[itemNum].price.ToString(); // price text
            
            _ifPurchased.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite =
                _moneySprite[1]; // Money sprite
            
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(true); // SetActive price text, purchaseBtn

            
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(true);

            _popupBtn[0].onClick.AddListener(delegate { PurchasePaidItem(paidItemName); });
        }
        
        // 구매했을 때.. 
        else
        {
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(false);
            _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "purchased";
        }
        
        _closeStoreBtn.interactable = false;
        SoundManager.instance.PlaySFX("Touch");
        _popupPanel[0].SetActive(true);
        _popUpDOTweens[0].DORestart();
    }
    
    private void PurchasePaidItem(StoreData.PaidItemName paidItemName)
    {
        int itemNum = (int)paidItemName;
        // int price = _storeData.paidItemData[itemNum].price;
        
        // To do : IAP 연결..

        if (_storeData.paidItemData[itemNum].packageCharacterNum[0] != 0) // package에 character가 있을 경우
        {
            for (int i = 0; i < _storeData.paidItemData[itemNum].packageCharacterNum.Length; i++)
            {
                _storeData.characterData[_storeData.paidItemData[itemNum].packageCharacterNum[i]].isPurchased = true;
                _storeData.characterData[_storeData.paidItemData[itemNum].packageCharacterNum[i]].isUnlocked = true;
            }
        }

        if (_storeData.paidItemData[itemNum].packageItemName[0] != (StoreData.ItemName)99) // package에 item이 있을 경우
        {
            for (int i = 0; i < _storeData.paidItemData[itemNum].packageItemName.Length; i++)
            {
                _storeData.itemData[(int)_storeData.paidItemData[itemNum].packageItemName[i]].isPurchased = true;
                _storeData.itemData[(int)_storeData.paidItemData[itemNum].packageItemName[i]].isUnlocked = true;
            }
        }
        
        DataCenter.Instance.UpdatePaidItemPurchaseData(paidItemName);

        UpdatePlayersDataInScene();

        InitStorePopup();
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