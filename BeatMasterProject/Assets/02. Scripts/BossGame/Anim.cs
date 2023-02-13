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
    //Die
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

    protected Animator[] _animators;
    protected Animator _anim;

    protected Animator _effectAnim;

    private RuntimeAnimatorController _charAnimator;

    private Dictionary<string, RuntimeAnimatorController> _animatorDic = new Dictionary<string, RuntimeAnimatorController>();

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _animators = GetComponentsInChildren<Animator>();
        _anim = _animators[0];
    }

    protected virtual RuntimeAnimatorController GetAnimatorDic(string _key)
    {
        if (_animatorDic.TryGetValue(_key, out _charAnimator))
        {
            return _animatorDic[_key];
        }
        _charAnimator = Resources.Load<RuntimeAnimatorController>("Animator/" + _key);
        _animatorDic.Add(_key, _charAnimator);
        return _charAnimator;
    }

    public void ChangeCharacterAnim(int charNum)
    {
        _anim.runtimeAnimatorController = GetAnimatorDic(Enum.GetName(typeof(CharacterNum), charNum));
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

                /*case CharacterStatus.Die:
                    Die();
                    break;*/
        }
    }


    protected virtual void Attack()
    {
        /*GameObject hammerClone = Instantiate(_hammer, _hammer.transform.position, _hammer.transform.rotation);
        _anim.Play("2_Attack_Normal", -1, 0f);
        hammerClone.GetComponent<Animator>().SetTrigger("Attack");*/
        _anim.Play("Attack");
    }

    protected virtual void Jump()
    {
        //anim.SetBool("Attack", false);
        /*        _anim.SetTrigger("Jump");
                _anim.SetBool("Run", true);
                _anim.SetBool("Idle", false);*/
        _anim.Play("Jump");
    }

    protected virtual void Damage()
    {
        /*        _anim.SetTrigger("Die");
                _anim.SetBool("Run", false);
                _anim.SetBool("Idle", false);*/
        _anim.Play("Damage");
    }

    protected virtual void Die()
    {
        _anim.SetTrigger("Die");
        _anim.SetBool("Run", false);
        _anim.SetBool("Idle", false);
    }

    protected virtual void Run()
    {
        /*_anim.SetBool("Run", true);
        _anim.SetBool("Idle", false);*/
        _anim.Play("Run");
    }

    protected virtual void Idle()
    {
        /*_anim.SetBool("Run", false);
        _anim.SetBool("Idle", true);*/
        _anim.Play("Idle");
    }
}