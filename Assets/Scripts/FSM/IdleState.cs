using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State {
    private float startTime;
    private float waiteTime;
    private float randomNum;
    public IdleState(FSM fsmIn) : base (fsmIn){

    }
    public override void OnEnter() {
        fsm.body.velocity = Vector2.zero;
        fsm.animator.Play("Rogue_Idle");
        startTime = Time.time;
        waiteTime = Random.Range(0, 4);
    }
    public override void OnUpdate() {
        if(fsm.playerLAArea.bounds.Intersects(fsm.rogueCollider.bounds) || fsm.playerHAArea.bounds.Intersects(fsm.rogueCollider.bounds)) {//����
            fsm.ChangeState(StateType.Hurt);
        }
        if (Time.time - startTime >= waiteTime) {//����ʱ����
            randomNum = Random.value;
            Debug.Log(randomNum);
            if (fsm.attackLenth.bounds.Intersects(fsm.playerColldiers[0].bounds) || fsm.attackLenth.bounds.Intersects(fsm.playerColldiers[1].bounds)) {//��ս����
                fsm.ChangeState(StateType.Attack);
            }
            else {
                if (randomNum > 0.3f) {
                    fsm.ChangeState(StateType.Run);
                }
                else {
                    fsm.ChangeState(StateType.Throw);
                }
            }
            if(!fsm.throwLenth.bounds.Intersects(fsm.playerColldiers[0].bounds) && !fsm.throwLenth.bounds.Intersects(fsm.playerColldiers[1].bounds)) {//Զ�̹���
            }
        }
    }
    public override void OnFixedUpdate() {

    }
    public override void OnExit() {

    }
}
