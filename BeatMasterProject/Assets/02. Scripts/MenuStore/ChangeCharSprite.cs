using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCharSprite : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _characterItemPos; //0: Corgi, 1:Tri_Corgi_Notail, 2:Duck 

    private Sprite _charSprite;
    private Sprite _itemSprite;
    private Sprite _paidItemSprite;
    private Dictionary<string, Sprite> _spriteDic = new Dictionary<string, Sprite>();

    public Sprite ChangeCharacterSprite(int charNum)
    {
        string _key = Enum.GetName(typeof(CharacterNum), charNum);
        if (_spriteDic.TryGetValue(_key, out _charSprite))
        {
            return _spriteDic[_key];
        }
        _charSprite = Resources.Load<Sprite>("Sprite/" + _key);
        _spriteDic.Add(_key, _charSprite);
        return _charSprite;
    }

    public Sprite ChangeItemSprite(StoreData.ItemName itemName)
    {
        string _key = itemName.ToString();
        if (_spriteDic.TryGetValue(_key, out _charSprite))
        {
            return _spriteDic[_key];
        }
        _itemSprite = Resources.Load<Sprite>("Sprite/" + _key);
        _spriteDic.Add(_key, _itemSprite);
        return _itemSprite;
    }
    
    public Sprite ChangePaidItemSprite(StoreData.PaidItemName paidItemName)
    {
        string _key = paidItemName.ToString();
        if (_spriteDic.TryGetValue(_key, out _charSprite))
        {
            return _spriteDic[_key];
        }
        _paidItemSprite = Resources.Load<Sprite>("Sprite/" + _key);
        _spriteDic.Add(_key, _paidItemSprite);
        return _paidItemSprite;
    }

    public void ChangeItemInItemScroll(PlayerData _playerData)
    {
        for (int i = 0; i < _playerData.itemData.Length; i++)
        {
            if (_playerData.itemData[i] != -1)
            {
                _characterItemPos[_playerData.playerChar < 2 ? 0 : 1].transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = ChangeItemSprite((StoreData.ItemName)_playerData.itemData[i]);
                _characterItemPos[_playerData.playerChar < 2 ? 0 : 1].transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                _characterItemPos[_playerData.playerChar < 2 ? 0 : 1].transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        _characterItemPos[_playerData.playerChar < 2 ? 0 : 1].SetActive(true);
        _characterItemPos[_playerData.playerChar < 2 ? 1 : 0].SetActive(false);
    }
}
