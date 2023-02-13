using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField]
    private Button _char;

    [SerializeField]
    private GameObject[] _popupPanel; //panels 0:_purchasePanel, 1:_noMoneyPanel
    [SerializeField]
    private Button[] _popupBtn; //buttons 0:_purchaseBtn, 1:_closeBtn
    [SerializeField]
    private Text _playerCoin;
    [SerializeField]
    private GameObject _ifPurchased;
    [SerializeField]
    private Anim _anim;

    //private Animator _animator;

    [SerializeField]
    private GameObject _content;

    private Sprite _charSprite;
    private Dictionary<string, Sprite> _spriteDic = new Dictionary<string, Sprite>();
    private Button[] _character;
    private StoreData _storeData;


    private void Awake()
    {
        DataCenter.Instance.LoadData();
    }

    private void Start()
    {
        //_animator = _popupPanel[0].transform.GetChild(0).GetComponent<Animator>();

        UpdatePlayersDataInScene();
        _storeData = DataCenter.Instance.GetStoreData();

        _character = new Button[_storeData.characterData.Length];
        for (int i = 0; i < _storeData.characterData.Length; i++)
        {
            int index = i;

            _character[index] = Instantiate(_char, _content.transform);
            _character[index].transform.GetComponent<Image>().sprite = ChangeCharacterSprite(index);
            _character[index].transform.GetChild(0).GetComponent<Text>().text = _storeData.characterData[i].price.ToString();
            if (_storeData.characterData[i].isUnlocked)
            {
                _character[index].onClick.AddListener(() => SetPurchasePopup(index));
            }
        }
        ClosePanel();
    }

    private void InitStorePopup()
    {
        _storeData = DataCenter.Instance.GetStoreData();
        _ifPurchased.transform.GetChild(0).GetComponent<Button>().interactable = true;
        _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "구매하기";
        foreach (var button in _popupBtn)
        {
            button.onClick.RemoveAllListeners();
        }
        _ifPurchased.SetActive(true);
        _popupPanel[0].SetActive(false);
        ClosePanel();
    }

    // 해금되었을 때만 호출
    private void SetPurchasePopup(int charNum)
    {
        _popupPanel[0].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ((Image)_character[charNum].targetGraphic).sprite;
        _popupPanel[0].SetActive(true);

        // 구매 안 한 상태일 때 구매하기 버튼 노출
        if (!_storeData.characterData[charNum].isPurchased && _storeData.characterData[charNum].isUnlocked)
        {
            _ifPurchased.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = _storeData.characterData[charNum].price.ToString();
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(true);

            UnityAction UA;
            UA = new UnityAction(() => PurchaseCharacter(charNum));
            _popupBtn[0].onClick.AddListener(UA);
        }

        // 구매했는데 미장착 상태일 때 장착하기 버튼
        else if (_storeData.characterData[charNum].isPurchased && charNum != DataCenter.Instance.GetPlayerData().playerChar)
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
            _ifPurchased.transform.GetChild(1).gameObject.SetActive(false);
            _ifPurchased.transform.GetChild(0).GetComponent<Button>().interactable = false;
            _ifPurchased.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "장착 중";
        }
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
    }

    private void ClosePanel()
    {
        foreach (var button in _popupBtn)
        {
            button.onClick.AddListener(() => InitStorePopup());
        }
    }

    private Sprite ChangeCharacterSprite(int charNum)
    {
        string _key = Enum.GetName(typeof(CharacterNum), charNum);
        if (_spriteDic.TryGetValue(_key, out _charSprite))
            return _spriteDic[_key];
        _charSprite = Resources.Load<Sprite>("Sprite/" + _key);
        _spriteDic.Add(_key, _charSprite);
        return _charSprite;
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
