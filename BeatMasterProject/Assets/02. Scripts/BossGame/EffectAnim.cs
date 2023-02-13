using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnim : Anim
{
    [SerializeField] private Animator _jumpAnim;
    protected override void Awake()
    {
        base.Awake();
        effectAnim = animators[1];
    }
    // Start is called before the first frame update
    protected override void Attack()
    {
        base.Attack();
    }

    protected override void Jump()
    {
        base.Jump();
        ShowJumpEffect();
    }

    protected override void Damage()
    {
        base.Damage();
    }

    protected override void Run()
    {
        base.Run();
    }

    protected override void Idle()
    {
        base.Idle();
    }


    private void ShowJumpEffect()
    {
        _jumpAnim.transform.position = transform.position; // set position of 
        _jumpAnim.Play("JumpDustEffect");
    }
    public void SetEffectBool(bool isLong)
    {
        effectAnim.SetBool("isLongNote", isLong);
    }
}
