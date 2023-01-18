using UnityEngine;
using UnityEngine.InputSystem;

namespace StayAwayGameLevelEnd
{
    public class LevelEnd : MonoBehaviour
    {
        void Update()
        {
            if (Keyboard.current.anyKey.isPressed)
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