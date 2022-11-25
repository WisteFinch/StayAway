using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace StayAwayGameScript
{
    public class MagicEmitter : MonoBehaviour
    {

        public float AngleDetection = 15;
        public float SpawnDistance = 0f;
        public float MaxDistance = 10f;

        [Header("Ԥ����")]
        public GameObject MagicWaterBall;

        public void EmitMagic(StayAwayGame.Magic magic, GameObject target = null)
        {
            // ��ȡ���������У������У���λ�ã�ת��Ϊ��Ļ���ꣻ
            var screenPosition = Camera.main.WorldToScreenPoint(this.transform.position);
            // ��ȡ����ڳ���������
            var mousePositionOnScreen = UnityEngine.Input.mousePosition;
            // �ó����е�Z=��������Z
            mousePositionOnScreen.z = screenPosition.z;
            // ������е�����ת��Ϊ�������꣬��ת��Ϊ����Լ��ĵ�λ����
            Vector2 mouseVector = (Camera.main.ScreenToWorldPoint(mousePositionOnScreen) - this.transform.position).normalized;

            // ��������
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float minDist = float.MaxValue;
            if (enemies.Length != 0)
            {
                foreach (GameObject enemy in enemies)
                {
                    // ��������Լ��ĵ�λ����
                    Vector2 enemyVector = (enemy.transform.position - this.transform.position).normalized;
                    // ����˾���
                    var dist = Vector2.Distance(this.transform.position, enemy.transform.position);
                    if(dist > this.MaxDistance)
                    {
                        continue;
                    }
                    if (dist < minDist && Vector2.Angle(mouseVector, enemyVector) <= this.AngleDetection)
                    {
                        minDist = dist;
                        target = enemy;
                    }
                }
            }
            
            // ����ħ��
            if(target != null)
            {
                GameObject instance;

                if(magic == StayAwayGame.Magic.MagicWaterBall)
                {
                    instance = Instantiate(this.MagicWaterBall);
                    instance.GetComponent<MagicController>().StartTracking(target, (Vector2)this.transform.position + mouseVector * this.SpawnDistance, mouseVector);
                }
            }
        }
    }
}