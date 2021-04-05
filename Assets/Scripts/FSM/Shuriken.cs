using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public float speed;

    private Transform originalParent;
    private Vector2 originalPosition;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private float startTime;
    private float direction;
    private void OnEnable() {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalParent = transform.parent;
        originalPosition = transform.localPosition;

        startTime = Time.time;
        transform.parent = null;
        spriteRenderer.color = new Color(1, 1, 1, 0);
        direction = originalParent.parent.localScale.x;
    }
    void FixedUpdate() {
        if (Time.time - startTime >= (4f / 14f)) {//�ȴ�Throw������ǰҡ����������ʾ���ƶ����｣
            spriteRenderer.color = new Color(1, 1, 1, 1);
            body.velocity = new Vector2(direction * speed * Time.fixedDeltaTime, 0);
        }
        if (Time.time - startTime >= 2f) {//��������
            transform.parent = originalParent;
            transform.localPosition = originalPosition;
            gameObject.SetActive(false);
        }
    }
}
