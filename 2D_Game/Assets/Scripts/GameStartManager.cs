using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStartManager : MonoBehaviour
{
    
    void Awake(){}
    void Upadate(){}

    // Game Start
    public void StartGame(){ 
        SceneManager.LoadScene("Scene 1");
    }
}
