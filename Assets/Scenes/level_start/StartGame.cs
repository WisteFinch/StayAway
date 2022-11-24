using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelStart
{
    public class StartGame : MonoBehaviour
    {
        void Update()
        {
            if(UnityEngine.Input.anyKeyDown)
            {
                SceneManager.LoadScene("level_guide");
            }    
        }
    }
}