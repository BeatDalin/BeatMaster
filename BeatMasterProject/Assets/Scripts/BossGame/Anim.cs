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
    Die
}

public class Anim : MonoBehaviour
{
    [SerializeField]
    private GameObject _hammer;
    private Animator _anim;

    private RhythmGameController gameController;
    private LaneController laneController;

    // Start is called before the first frame update
    void Start()
    {
        _anim=GetComponentInChildren<Animator>();
        StatusJudge(Status.Run);
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

            case Status.Die:
                Die();
                break;
        }
    }


    public void Attack()
    {
        GameObject hammerClone = Instantiate(_hammer, _hammer.transform.position, _hammer.transform.rotation);
        _anim.SetTrigger("Attack");
        hammerClone.GetComponent<Animator>().SetTrigger("Attack");

    }

    public void Jump()
    {
        //anim.SetBool("Attack", false);
    }

    public void Die()
    {
        _anim.SetTrigger("Die");
        _anim.SetBool("Run", false);
    }

    public void Run()
    {
        _anim.SetBool("Run", true);
    }

    public void Idle()
    {
        _anim.SetBool("Attack", false);
        _anim.SetBool("Run", false);
    }
}
