using System;
using SonicBloom.Koreo.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status
{
    Idle = 0,
    Run,
    Jump,
    Attack,
    Damage,
    Die
}

public class Anim : MonoBehaviour
{
    [SerializeField] private GameObject _hammer;
    private Animator _anim;

    // Start is called before the first frame update
    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
    }

    public void StatusJudge(Status crnStat)
    {
        switch (crnStat)
        {
            case Status.Idle:
                Idle();
                break;

            case Status.Run:
                Run();
                break;

            case Status.Jump:
                Jump();
                break;

            case Status.Attack:
                Attack();
                break;

            case Status.Damage:
                Damage();
                break;

            case Status.Die:
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
}