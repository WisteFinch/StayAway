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

        [Header("预制体")]
        public GameObject MagicWaterBall;

        public void EmitMagic(StayAwayGame.Magic magic, GameObject target = null)
        {
            // 获取鼠标在相机中（世界中）的位置，转换为屏幕坐标；
            var screenPosition = Camera.main.WorldToScreenPoint(this.transform.position);
            // 获取鼠标在场景中坐标
            var mousePositionOnScreen = UnityEngine.Input.mousePosition;
            // 让场景中的Z=鼠标坐标的Z
            mousePositionOnScreen.z = screenPosition.z;
            // 将相机中的坐标转化为世界坐标，再转化为相对自己的单位向量
            Vector2 mouseVector = (Camera.main.ScreenToWorldPoint(mousePositionOnScreen) - this.transform.position).normalized;

            // 遍历敌人
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float minDist = float.MaxValue;
            if (enemies.Length != 0)
            {
                foreach (GameObject enemy in enemies)
                {
                    // 敌人相对自己的单位向量
                    Vector2 enemyVector = (enemy.transform.position - this.transform.position).normalized;
                    // 与敌人距离
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
            
            // 发射魔法
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