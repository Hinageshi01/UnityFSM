using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowState : IState {
    private FSM fsm;
    private AnimatorStateInfo animeInfo;

    public ThrowState(FSM fsmIn) {
        this.fsm = fsmIn;
    }
    public void OnEnter() {
        fsm.body.velocity = Vector2.zero;
        fsm.animator.Play("Rogue_Throw");
        fsm.gameObject.transform.Find("Shurikens").Find("Shuriken").gameObject.SetActive(true);
        //Shuriken的控制在Shuriken的脚本中完成
    }
    public void OnUpdate() {
        animeInfo = fsm.animator.GetCurrentAnimatorStateInfo(0);
        if (animeInfo.normalizedTime >= 0.99f) {
            fsm.ChangeState(StateType.Idle);
        }
    }
    public void OnFixedUpdate() {

    }
    public void OnExit() {
        fsm.idleStartTime = Time.time;
    }
}
