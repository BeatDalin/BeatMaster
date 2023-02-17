using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get; private set; }
    public CharacterStatus playerStatus = CharacterStatus.Idle;

    private float _hp = 100;
    private float Damage { get { return _hp; } set { _hp -= value > _hp ? 0 : value; } }
    private Anim _animation;
    private float _speed;
    private float _colorValue = 1f;
    private Vector4 _color;
    private Vector4 _hairColor;
    private SPUM_SpriteList _spumSpriteList;

    [SerializeField] private BossGameUI _bossGameUI;

    private void Awake()
    {
        Instance = this;
        _animation = GetComponent<Anim>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        /*        _spumSpriteList = GetComponentInChildren<SPUM_SpriteList>();
                _hairColor = (Color.white - _spumSpriteList._itemList[0].color) / 10;*/
    }

    public void ChangeStatus(CharacterStatus status)
    {
        if (status == playerStatus)
            return;
        if (playerStatus == CharacterStatus.FastIdle && status == CharacterStatus.Jump)
        {
            status = CharacterStatus.FastIdle;
        }
        if(playerStatus == CharacterStatus.FastIdle)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y-0.13f, gameObject.transform.position.z);
        }
        else if(status== CharacterStatus.FastIdle)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+ 0.13f, gameObject.transform.position.z);
        }

        playerStatus = status;
        _animation.StatusJudge(playerStatus);
    }

    public void DecreaseHP()
    {
        Damage = 10;
        ChangeCharacterColor();

        if (_hp == 0)
        {
            //ChangeStatus(CharacterStatus.Die);
            _bossGameUI.ShowFinalPanel();
        }
    }

    private void ChangeCharacterColor()
    {
        _colorValue -= 0.1f;
        _color = Vector4.one * _colorValue;
        _color.w = 1;

        for (var i = 0; i < _spumSpriteList._itemList.Count; i++)
        {
            if (i == 0)
            {
                Vector4 temp = _spumSpriteList._itemList[0].color;
                temp -= _hairColor;
                _spumSpriteList._itemList[0].color = temp;
            }
            else
            {
                _spumSpriteList._itemList[i].color = _color;
            }
        }

        for (var i = 0; i < _spumSpriteList._bodyList.Count; i++)
        {
            _spumSpriteList._bodyList[i].color = _color;
        }
    }
}