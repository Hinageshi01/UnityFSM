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

    private Rigidbody2D body;
    private Animator animator;
    private int jumpCount;
    private bool jumpPressed = false;
    private int runningID, isRunId, jumpId, jumpingID, isIdleID, lightAttackID, haveyAttackID, comboID, isAttackID;
    //isAttack��animator��Ϊbool�ͣ���Ϊ��ֹ��������������������ϵı�Ƿ�

    private int combo = 0;
    private float timeCount, interval = 2f;
    private bool uponGround, isAttack = false, isSlamAttack = false, canPlayJumpEffect = true;
    //isAttack����������ͨ����ʱ���ƶ����룬isSlamAttack�����������乥����ҡʱ����Ծ��canPlayJumpEffect���ڷ�ֹ��Ծ��������ڳ�ʼ��֮ǰ������
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
        uponGround = Physics2D.OverlapCircle(footPoint.position, 0.1f, groundLayer);
        if (Input.GetButtonDown("Jump") && jumpCount > 0 && !isSlamAttack) {
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
        if (body.velocity.y <= 0 && (uponGround)) {
            jumpCount = finalJumpCount;//���ʱ������Ծ��صĲ���������Ҫ���������ʱOverlapCircle��⵽����
        }
        if (jumpPressed && jumpCount > 0 && !isSlamAttack) {//����ع�����ҡ�ڵ���Ծ
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
        animator.SetBool(isIdleID, uponGround);
    }
    private Coroutine tmpCoroutine = null;
    private void Attack() {
        if (!animator.GetBool(isAttackID)) {
            if(uponGround) {
                if (Input.GetMouseButton(0)) {//����ṥ��
                    if (tmpCoroutine != null) {
                        StopCoroutine(tmpCoroutine);
                    }
                    tmpCoroutine = StartCoroutine(setIsAttackFalse(4f / 14f));
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
                if (Input.GetMouseButton(1)) {//�Ҽ��ع���
                    if (tmpCoroutine != null) {
                        StopCoroutine(tmpCoroutine);
                    }
                    tmpCoroutine = StartCoroutine(setIsAttackFalse(6f / 14f));
                    body.velocity = Vector2.zero;
                    animator.SetBool(isAttackID, true);
                    isAttack = true;
                    animator.SetTrigger(haveyAttackID);
                    attackName = "Heavy";
                }
            }
            else {
                if (Input.GetMouseButton(0)) {//���й���
                    animator.SetBool(isAttackID, true);
                    animator.SetTrigger(lightAttackID);
                    attackName = "Light";
                }
                if (Input.GetMouseButton(1)) {//���乥��
                    if (tmpCoroutine != null) {
                        StopCoroutine(tmpCoroutine);
                    }
                    tmpCoroutine = StartCoroutine(setIsAttackFalse(9f / 14f));
                    body.velocity = Vector2.zero;
                    animator.SetBool(isAttackID, true);
                    isAttack = true;
                    isSlamAttack = true;
                    animator.SetTrigger(haveyAttackID);
                    attackName = "";
                }
            }
        }
        if (timeCount != 0) {
            timeCount -= Time.deltaTime;
            if (timeCount <= 0) {
                timeCount = 2f;
                combo = 0;
            }
        }
    }
    IEnumerator setIsAttackFalse(float time) {
        yield return new WaitForSeconds(time);
        isAttack = false;
        isSlamAttack = false;
    }
    public void AttackOver() {//��ÿ���������������ʱ����
        animator.SetBool(isAttackID, false);
    }

    public void OnJump() {
        AudioManager.PlayJumpClip();
        if (canPlayJumpEffect) {
            canPlayJumpEffect = false;
            GameObject effect = transform.Find("Effects").Find("JumpDust").gameObject;
            StartCoroutine(PlayEffect(effect, 3f / 14f));
        }
    }
    public void OnLanding() {
        AudioManager.PlayLandingClip();
        GameObject effect = transform.Find("Effects").Find("LandingDust").gameObject;
        StartCoroutine(PlayEffect(effect, 2f / 14f));
    }
    public void OnRunSop() {
        GameObject effect = transform.Find("Effects").Find("RunStopDust").gameObject;
        StartCoroutine(PlayEffect(effect, 3f / 14f));
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
    public void OnAirSlamLanding() {
        AudioManager.PlayAirSlamLandingClip();
        GameObject effect = transform.Find("Effects").Find("AirSlamDust").gameObject;
        StartCoroutine(PlayEffect(effect, 5f / 14f));
    }
    public void DrawSwordAudio() {
        AudioManager.PlayDrawSwordClip();
    }
    private IEnumerator PlayEffect(GameObject effectIn, float time) {//�ڶ�Ӧ��Ҫ��Ч�Ķ����б�����
        effectIn.SetActive(true);
        effectIn.transform.SetParent(null);
        yield return new WaitForSecondsRealtime(time);//����һ��x/14��x���ö�������֡��
        effectIn.transform.SetParent(effectIn.GetComponent<Effect>().originalParent);//��ʼ��������Effect�ű��е�OnEnable�б���ֵ
        effectIn.transform.localPosition = effectIn.GetComponent<Effect>().originalPosition;
        effectIn.SetActive(false);
        canPlayJumpEffect = true;
    }
}
