using StayAwayGameScript;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameLevelScript
{
    public class LevelGuideScript : MonoBehaviour
    {
        void Start()
        {
            GameManager.Instance.InitPlayer();
        }
    }
}