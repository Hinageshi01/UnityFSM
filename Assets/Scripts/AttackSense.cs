using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSense : MonoBehaviour
{
    public static AttackSense instance;

    private bool isShake;
    private void Awake() {
        instance = this;
    }
    public void FrameFreeze(int pauseFrame) {//��PlayerControler�е���
        StartCoroutine(PauseOnAttack(pauseFrame));
    }
    IEnumerator PauseOnAttack(int pauseFrame) {
        float pauseTime = pauseFrame / 60f;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1;
    }
    //��Ļ�����Ĺ�����Cinemachineʵ��
}
