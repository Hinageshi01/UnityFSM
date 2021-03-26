using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform footPoint, frontPoint;
    public Collider2D usualCollider, crouchCollider;
    public LayerMask groundLayer, enemyLayer;
    public float speed, jumpForce, attackMove;
    public int finalJumpCount, finalMaxCombo;
    [Header("�����")]
    public int lightPauseFrame;
    public int heavyPauseFrame;
    //public float shakeDuration, lightShakeStrength, heavyShakeStrength;

    private Rigidbody2D body;
    private Animator animator;
    private int jumpCount;
    private bool jumpPressed = false;
    private int runningID, isRunId, jumpId, jumpingID, isIdleID, lightAttackID, haveyAttackID, comboID, isAttackID;
    //isAttack��animator��Ϊbool�ͣ���Ϊ��ֹ��������������������ϵı�Ƿ�

    private int combo = 0;
    private float timeCount, interval = 2f;
    private bool isAttack = false;
    private string attackName;
    void Awake() {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        runningID = Animator.StringToHash("Running");
        isRunId = Animator.StringToHash("isRun");
        isIdleID = Animator.StringToHash("isIdle");
        jumpId = Animator.StringToHash("Jump");
        jumpingID = Animator.StringToHash("Jumping");
        lightAttackID = Animator.StringToHash("LightAttack");
        haveyAttackID = Animator.StringToHash("HaveyAttack");
        comboID = Animator.StringToHash("Combo");
        isAttackID = Animator.StringToHash("isAttack");
    }
    void Update() {
        if (Input.GetButtonDown("Jump") && jumpCount > 0) {
            jumpPressed = true;
        }
        Animation();
        Attack();
    }
    void FixedUpdate() {
        Movement();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Enemy")) {
            if (transform.localScale.x > 0) {
                collision.GetComponent<Enemy>().Hurt(Vector2.right);
            }
            else {
                collision.GetComponent<Enemy>().Hurt(Vector2.left);
            }
            if (attackName == "Light") {
                AttackSense.instance.FrameFreeze(lightPauseFrame);
            }
            if (attackName == "Heavy") {
                AttackSense.instance.FrameFreeze(heavyPauseFrame);
            }
        }
    }
    private void Movement() {
        float move = Input.GetAxisRaw("Horizontal");//-1, 0, 1
        if (!isAttack) {
            body.velocity = new Vector2(move * speed * Time.fixedDeltaTime, body.velocity.y);//�ǹ���״̬�µ��ƶ�
        }
        if (move != 0) {
            transform.localScale = new Vector3(move, 1, 1);//ת��
        }
        if (body.velocity.y <= 0 && (Physics2D.OverlapCircle(footPoint.position, 0.1f, groundLayer) || Physics2D.OverlapCircle(footPoint.position, 0.1f, enemyLayer))) {
            jumpCount = finalJumpCount;//��غͲȵ���ʱ������Ծ��صĲ���������Ҫ���������ʱOverlapCircle��⵽����
        }
        if (jumpPressed && jumpCount > 0) {//��Ծ
            jumpPressed = false;
            isAttack = false;
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            animator.SetTrigger(jumpId);
            jumpCount--;
        }
    }
    private void Animation() {
        float flMove = Input.GetAxis("Horizontal");//-1f ~ 1f
        float ySpeed = body.velocity.y;
        if (Input.GetAxisRaw("Horizontal") != 0) {
            animator.SetBool(isRunId, true);
        }
        else {
            animator.SetBool(isRunId, false);
        }
        animator.SetFloat(runningID, Mathf.Abs(flMove));
        animator.SetFloat(jumpingID, ySpeed);
        animator.SetBool(isIdleID, Physics2D.OverlapCircle(footPoint.position, 0.1f, groundLayer));
        if(Physics2D.OverlapCircle(footPoint.position, 0.1f, enemyLayer)) {
            animator.SetBool(isIdleID, true);
        }
    }
    Coroutine tmpCoroutine = null;
    private void Attack() {
        if (Input.GetMouseButton(0) && !animator.GetBool(isAttackID)) {//����ṥ��
            if (tmpCoroutine != null) {
                StopCoroutine(tmpCoroutine);
            }
            tmpCoroutine = StartCoroutine(setIsAttackFalse());
            //����Э�̵���ʱʹisAttackΪfalse���ر�Э��ȷ�����ʱ����isAttack���ᱻ��Ϊfalse
            body.velocity = Vector2.zero;
            animator.SetBool(isAttackID, true);
            isAttack = true;
            combo++;
            if (combo > finalMaxCombo) {//comboΪ1ʱ����һ�ι�����Ϊ2ʱ���ж��ι���
                combo = 1;
            }
            timeCount = interval;
            animator.SetTrigger(lightAttackID);
            animator.SetInteger(comboID, combo);
            body.MovePosition(frontPoint.position);
            attackName = "Light";
        }
        if (Input.GetMouseButton(1) && !animator.GetBool(isAttackID)) {//�Ҽ��ع���
            if (tmpCoroutine != null) {
                StopCoroutine(tmpCoroutine);
            }
            tmpCoroutine = StartCoroutine(setIsAttackFalse());
            body.velocity = Vector2.zero;
            animator.SetBool(isAttackID, true);
            isAttack = true;
            animator.SetTrigger(haveyAttackID);
            attackName = "Heavy";
        }
        if (timeCount != 0) {
            timeCount -= Time.deltaTime;
            if (timeCount <= 0) {
                timeCount = 2f;
                combo = 0;
            }
        }
    }
    IEnumerator setIsAttackFalse() {
        yield return new WaitForSeconds(0.2857f);//4/14s����һ�ι�����������ʱ��
        isAttack = false;
    }
    public void AttackOver() {//��ÿ���������������ʱ����
        animator.SetBool(isAttackID, false);
    }

    public void JumpAudio() {
        AudioManager.PlayJumpClip();
    }
    public void LandingAudio() {
        AudioManager.PlayLandingClip();
    }
    public void FootStepAudio() {
        AudioManager.PlayFootStepClip();
    }
    public void SheathSwordAudio() {
        AudioManager.PlaySheathSwordClip();
    }
    public void SwordSwooshAudio() {
        AudioManager.PlaySwordSwooshClip();
    }
}
