using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public LayerMask groundLayer, enemyLayer;
    public float speed, jumpForce;
    [Header("�����")]
    public int lightPauseFrame;
    public int heavyPauseFrame;
    [Header("���")]
    public float dashSpeed;
    public float dashTime;
    public float slamSpeed;

    private Rigidbody2D body;
    private Animator animator;
    private Collider2D usualCollider, crouchCollider;
    private Transform footPoint, frontPoint;
    private AnimatorStateInfo animeInfo;
    private int jumpCount, finalJumpCount = 2;
    private bool jumpPressed = false;
    private int runningID, isRunId, jumpId, jumpingID, isIdleID, lightAttackID, haveyAttackID, comboID, isAttackID, isCrouchID, dashID, isDashID, HurtID, isHurtID;
    private int parryStanceID, isParryStanceID, parryID, isParryID;
    //isAttack��animator��Ϊbool�ͣ���Ϊ��ֹ��������������������ϵı�Ƿ���
    //isHurtID��animator��Ϊbool�ͣ���Ϊ��ֹ���˶���������������ϵı�Ƿ���

    private int finalMaxCombo = 2, combo = 0;
    private float timeCount, interval = 2f;
    private bool uponGround, isAttack = false, isSlam = false, isCrouch = false, isDash = false, canDash = true, isHurt = false, isParryStance = false, isParry = false;
    //isAttack����������ͨ����ʱ���ƶ����룬Ϊ�˱�֤�ָ��붯����������������animator�е�isAttack���á�isSlamAttack�����������乥����ҡʱ����Ծ��
    //��isDash���ڴ������״̬��Ϊ��ʱ�����������ƶ���ص����룬canDash��������Dashing�Ĵ���Ƶ�ʣ�����isDash���á�
    private string attackName;
    void Awake() {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        usualCollider = GetComponents<BoxCollider2D>()[0];
        crouchCollider = GetComponents<BoxCollider2D>()[1];
        footPoint = transform.Find("FootPoint");
        frontPoint = transform.Find("FrontPoint");

        runningID = Animator.StringToHash("Running");
        isRunId = Animator.StringToHash("isRun");
        isIdleID = Animator.StringToHash("isIdle");
        jumpId = Animator.StringToHash("Jump");
        jumpingID = Animator.StringToHash("Jumping");
        lightAttackID = Animator.StringToHash("LightAttack");
        haveyAttackID = Animator.StringToHash("HaveyAttack");
        comboID = Animator.StringToHash("Combo");
        isAttackID = Animator.StringToHash("isAttack");
        isCrouchID = Animator.StringToHash("isCrouch");
        dashID = Animator.StringToHash("Dash");
        isDashID = Animator.StringToHash("isDash");
        HurtID = Animator.StringToHash("Hurt");
        isHurtID = Animator.StringToHash("isHurt");
        parryStanceID = Animator.StringToHash("ParryyStance");
        isParryStanceID = Animator.StringToHash("isParryStance");
        parryID = Animator.StringToHash("Parry");
        isParryID = Animator.StringToHash("isParry");
    }
    void Update() {
        uponGround = Physics2D.OverlapCircle(footPoint.position, 0.1f, groundLayer);
        GetInput();
        Animation();
        Attack();
        Crouch();
    }
    void FixedUpdate() {
        Movement();
    }
    private void GetInput() {//��Update�е��������еؽ�������
        if (Input.GetButtonDown("Jump") && jumpCount > 0 && !isSlam && !isHurt && !isParryStance) {
            jumpPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (!isDash && canDash && !isSlam && !isHurt && !isParry) {
                isDash = true;
                isParryStance = false;
                animator.SetTrigger(dashID);
                canDash = false;
                StartCoroutine(Dash(dashTime));
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && !isCrouch && !isHurt) {
            isParryStance = true;
            animator.SetTrigger(parryStanceID);
        }
        if (Input.GetKeyUp(KeyCode.E)) {
            isParryStance = false;
        }
    }
    private IEnumerator Dash(float dashTime) {
        yield return new WaitForSeconds(dashTime);
        isDash = false;
        yield return new WaitForSeconds(0.4f);
        canDash = true;//��̽����󾭹�һ�����ݵ�CD�������ٴγ��
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Enemy")) {//����˽���������֡����
            if (attackName == "Light") {
                AttackSense.instance.FrameFreeze(lightPauseFrame);
            }
            if (attackName == "Heavy") {
                AttackSense.instance.FrameFreeze(heavyPauseFrame);
            }
        }
        if (collision.CompareTag("AttackArea") && !isDash) {//�ܻ�
            if (isParryStance) {//����
                transform.localScale = collision.bounds.center.x >= usualCollider.bounds.center.x ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
                isParry = true;
                animator.SetTrigger(parryID);
                Invoke(nameof(ParryOver), 5f / 14f);
                return;
            }
            //����
            isHurt = true;
            animator.SetTrigger(HurtID);
        }
    }
    public void HurtOver() {//�����˶�������ʱ������
        isHurt = false;
        isParryStance = false;
    }
    public void ParryOver() {
        isParry = false;
        isParryStance = false;
    }
    private void Movement() {
        float move = Input.GetAxisRaw("Horizontal");//-1, 0, 1
        if (isDash) {//����ƶ���������ȼ�
            body.velocity = new Vector2(dashSpeed * transform.localScale.x, body.velocity.y);
            AfterimagePool.instance.TakeFromPool();
            return;
        }
        if (move != 0) {
            transform.localScale = new Vector3(move, 1, 1);//ת��
        }
        if (isParry) {//�����ɹ�������ʱ���ƶ�
            body.velocity = new Vector2(-transform.localScale.x * 5, body.velocity.y);
            return;
        }
        if (isHurt || isParryStance) {//����ʱ�����ʱ��x�᲻���ƶ�����Ҫ�������Ȼ����
            body.velocity = new Vector2(0, body.velocity.y);
            return;
        }
        if (isCrouch) {//�¶�״̬�µ��ƶ�
            body.velocity = new Vector2(move * speed * Time.fixedDeltaTime * 0.15f, body.velocity.y);
            return;
        }
        if (!isAttack) {//��ͨ״̬�µ��ƶ�
            body.velocity = new Vector2(move * speed * Time.fixedDeltaTime, body.velocity.y);
        }
        else {
            if (isSlam) {//���乥��ʱ���³��
                body.velocity = new Vector2(0, -slamSpeed);
            }
            else {//���в��򵱲����ƶ����������ʱ��Vector2.zero����velocity���ⷢ��Ԥ������ƶ�
                body.velocity = Vector2.zero;
            }
        }
        if (uponGround && body.velocity.y <= 0) {
            jumpCount = finalJumpCount;//���ʱ������Ծ��صĲ���������Ҫ���������ʱOverlapCircle��⵽����
        }
        if (jumpPressed && jumpCount > 0) {//��ع�����ҡʱjumpPressed����Ϊtrue
            jumpPressed = false;
            isAttack = false;//���̽����ƶ�����Ծ�������
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            animator.SetTrigger(jumpId);
            jumpCount--;
        }
    }
    private void Crouch() {
        if (Input.GetButtonDown("Crouch")) {
            isCrouch = true;
            animator.SetBool(isCrouchID, true);
            usualCollider.enabled = false;
            crouchCollider.enabled = true;
        }
        if (Input.GetButtonUp("Crouch")) {
            isCrouch = false;
            animator.SetBool(isCrouchID, false);
            crouchCollider.enabled = false;
            usualCollider.enabled = true;
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
        animator.SetBool(isParryStanceID, isParryStance);
        animator.SetBool(isDashID, isDash);
        animator.SetBool(isHurtID, isHurt);
        animator.SetBool(isParryID, isParry);
    }
    private Coroutine tmpCoroutine = null;//����Attack��ʹ��
    private void Attack() {
        if (!animator.GetBool(isAttackID) && !isHurt && !isDash && !isParryStance) {
            if (uponGround) {
                if (Input.GetMouseButton(0)) {//����ṥ��
                    if (tmpCoroutine != null) {
                        StopCoroutine(tmpCoroutine);
                    }
                    tmpCoroutine = StartCoroutine(WaitForAttackOver(4f / 14f));
                    //�ر�Э��ȷ�����ʱ����isAttack���ᱻ��Ϊfalse������Э�̵���ʱʹisAttackΪfalse
                    body.velocity = Vector2.zero;
                    isAttack = true;
                    timeCount = interval;
                    combo++;
                    if (combo > finalMaxCombo) {//comboΪ1ʱ����һ�ι���������Ϊ2ʱ���Ŷ��ι�������
                        combo = 1;
                    }
                    animator.SetBool(isAttackID, true);
                    animator.SetTrigger(lightAttackID);
                    body.MovePosition(frontPoint.position);
                    animator.SetInteger(comboID, combo);
                    attackName = "Light";
                }
                if (Input.GetMouseButton(1)) {//�Ҽ��ع���
                    if (tmpCoroutine != null) {
                        StopCoroutine(tmpCoroutine);
                    }
                    tmpCoroutine = StartCoroutine(WaitForAttackOver(6f / 14f));
                    body.velocity = Vector2.zero;
                    isAttack = true;
                    animator.SetBool(isAttackID, true);
                    animator.SetTrigger(haveyAttackID);
                    attackName = "Heavy";
                }
            }
            else {//�ڿ���
                if (Input.GetMouseButton(0)) {//���й���
                    animator.SetBool(isAttackID, true);
                    animator.SetTrigger(lightAttackID);
                    attackName = "Light";
                }
                if (Input.GetMouseButton(1)) {//���乥��
                    if (tmpCoroutine != null) {
                        StopCoroutine(tmpCoroutine);
                    }
                    tmpCoroutine = StartCoroutine(WaitForAttackOver(6f / 14f));
                    body.velocity = Vector2.zero;
                    isAttack = true;
                    isSlam = true;
                    animator.SetBool(isAttackID, true);
                    animator.SetTrigger(haveyAttackID);
                    attackName = "";
                }
            }
        }//if (!animator.GetBool(isAttackID))
        if (timeCount != 0) {
            timeCount -= Time.deltaTime;
            if (timeCount <= 0) {
                timeCount = 2f;
                combo = 0;
            }
        }
    }
    private IEnumerator WaitForAttackOver(float time) {
        yield return new WaitForSeconds(time);
        isAttack = false;
        isSlam = false;
        animator.SetBool(isAttackID, false);//ȷ����ʹ������������ϣ�animator�е�isAttackҲ����ȷ�ر�����
    }
    public void AttackOver() {//��ÿ���������������ʱ����
        animator.SetBool(isAttackID, false);
    }
}
