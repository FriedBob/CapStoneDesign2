using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 점수와 스테이지를 관리함
public class GameManager : MonoBehaviour
{
    [SerializeField] public int totalPoint;
    [SerializeField] public int stagePoint;
    [SerializeField] public int stageIndex;
    [SerializeField] Player player;
    [SerializeField] Vector2 resetPos;

    // Game UI
    public Image[] UIHealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartBtn;

    public GameObject[] Stages;     // stage들을 관리함

    void Start(){
       // player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        resetPos = new Vector2(player.transform.position.x, player.transform.position.y);
    }
    
    void Update(){
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        // Change Stage
        if(stageIndex < Stages.Length-1){

        Stages[stageIndex].SetActive(false);
        stageIndex++;
        Stages[stageIndex].SetActive(true);

        totalPoint += stagePoint;
        stagePoint = 0;

        UIStage.text = "STAGE " + (stageIndex+1);
        }
        else{       // 게임 클리어
            Time.timeScale = 0; // 시간을 멈춤
            Debug.Log("게임 클리어!");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(stagePoint<0)
            stagePoint = 0;

        if(player.transform.position.y < -30){
            player.transform.position = new Vector2(resetPos.x, resetPos.y);
        }
    }

    public void Restart(){
        SceneManager.LoadScene(0);
    }
}
