using System;
using SonicBloom.Koreo.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStatus
{
    Idle = 0,
    Run,
    Jump,
    Attack,
    Damage,
    Die
}

public enum CharacterNum
{
    Corgi,
    Tri_Corgi,
    Corgi_Notail,
    Tri_Corgi_Notail,
}

public class Anim : MonoBehaviour
{
    [SerializeField] private GameObject _hammer;

    [SerializeField] private Animator[] _animators;
    private Animator _anim;

    private Animator _effectAnim;
    [SerializeField] private Animator _jumpAnim;

    /*    [SerializeField]
        private RuntimeAnimatorController[] _animatorController;*/
    private RuntimeAnimatorController _value;
    Dictionary<string, RuntimeAnimatorController> AnimatorDic = new Dictionary<string, RuntimeAnimatorController>();
    // Start is called before the first frame update
    private void Awake()
    {
        _animators = GetComponentsInChildren<Animator>();
        _anim = _animators[0];
        if (_animators.Length > 1) //수정예정
        {
            _effectAnim = _animators[1];
        }
    }

    private RuntimeAnimatorController GetDic(string _key)
    {
        if (AnimatorDic.TryGetValue(_key, out _value))
            return AnimatorDic[_key];
        _value = Resources.Load<RuntimeAnimatorController>(_key);
        AnimatorDic.Add(_key, _value);
        return _value;
    }

    public void ChangeCharacterAnim(int charNum)
    {
        _anim.runtimeAnimatorController = GetDic(Enum.GetName(typeof(CharacterNum), charNum));
    }

    public void StatusJudge(CharacterStatus crnStat)
    {
        switch (crnStat)
        {
            case CharacterStatus.Idle:
                Idle();
                break;

            case CharacterStatus.Run:
                Run();
                break;

            case CharacterStatus.Jump:
                Jump();
                break;

            case CharacterStatus.Attack:
                Attack();
                break;

            case CharacterStatus.Damage:
                Damage();
                break;

            case CharacterStatus.Die:
                Die();
                break;
        }
    }


    public void Attack()
    {
        /*GameObject hammerClone = Instantiate(_hammer, _hammer.transform.position, _hammer.transform.rotation);
        _anim.Play("2_Attack_Normal", -1, 0f);
        hammerClone.GetComponent<Animator>().SetTrigger("Attack");*/
        _anim.Play("Attack");
    }

    public void Jump()
    {
        //anim.SetBool("Attack", false);
        /*        _anim.SetTrigger("Jump");
                _anim.SetBool("Run", true);
                _anim.SetBool("Idle", false);*/
        _anim.Play("Jump");
        if (_animators.Length > 1)
        {
            ShowJumpEffect();
        }
    }

    public void Damage()
    {
        /*        _anim.SetTrigger("Die");
                _anim.SetBool("Run", false);
                _anim.SetBool("Idle", false);*/
        _anim.Play("Damage");
    }

    public void Die()
    {
        _anim.SetTrigger("Die");
        _anim.SetBool("Run", false);
        _anim.SetBool("Idle", false);
    }

    public void Run()
    {
        /*_anim.SetBool("Run", true);
        _anim.SetBool("Idle", false);*/
        _anim.Play("Run");
    }

    public void Idle()
    {
        /*_anim.SetBool("Run", false);
        _anim.SetBool("Idle", true);*/
        _anim.Play("Idle");
    }


    private void ShowJumpEffect()
    {
        _jumpAnim.transform.position = transform.position; // set position of 
        _jumpAnim.Play("JumpDustEffect");
    }
    public void SetEffectBool(bool isLong)
    {
        _effectAnim.SetBool("isLongNote", isLong);
    }
}