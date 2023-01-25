using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayterStatus : MonoBehaviour
{
    public static PlayterStatus instance { get; private set; }
    public Status playerStatus = Status.Run;

    private float _hp=100;
    private float _damage
    {
        get { return _hp; }
        set
        {
            _hp -= value > _hp ? 0 : value;
        }
    }
    private Anim _animation;
    private float _speed;
    private float _colorValue=1f;
    private Vector4 _color;
    private Vector4 _hairColor;
    private SPUM_SpriteList _spumSpriteList;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    private void Start()
    {
        _animation=GetComponent<Anim>();
        _spumSpriteList=GetComponentInChildren<SPUM_SpriteList>();
        _hairColor = (Color.white - _spumSpriteList._itemList[0].color) / 10;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DecreaseHP();
        }
    }

    public void ChangeStatus(Status status)
    {
        playerStatus = status;
        _animation.StatusJudge(playerStatus);
    }

    private void DecreaseHP()
    {
        _damage = 10;
        ChangeCharacterColor();

        if (_hp <= 0)
        {
            ChangeStatus(Status.Die );
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
