using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StayAwayGameLevelEnd
{
    public class LevelEnd : MonoBehaviour
    {
        void Update()
        {
            if (UnityEngine.Input.anyKeyDown)
            {

                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif

            }
        }
    }
}