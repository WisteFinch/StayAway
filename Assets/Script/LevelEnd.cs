using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    public class LevelEnd : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Pony"))
            {
                collision.gameObject.GetComponent<GameLogic>().FlowGameOver(0);
            }
        }
    }
}