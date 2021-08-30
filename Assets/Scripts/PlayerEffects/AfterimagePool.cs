using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterimagePool : MonoBehaviour
{
    public static AfterimagePool instance;
    public GameObject imagePrefab;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        FillPoll(10);
    }
    public void FillPoll(int num) {
        for (int i = 0; i < num; i++) {
            GameObject newImage = Instantiate(imagePrefab);
            newImage.transform.SetParent(transform);
            ReturnToPool(newImage);
        }
    }
    public void ReturnToPool(GameObject objectIn) {//�ڶ�������ű��е���
        objectIn.SetActive(false);
        pool.Enqueue(objectIn);
    }
    public void TakeFromPool() {//PlayerΪ���״̬ʱ��FixedUpdate�з�������
        if (pool.Count <= 0) {//����ز�����ʱ�����ɼ���Ԥ�Ƽ�
            FillPoll(5);
        }
        GameObject crtImage = pool.Dequeue();
        crtImage.SetActive(true);
    }
}
