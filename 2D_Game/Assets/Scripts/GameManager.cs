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

    [SerializeField] AudioSource audiosource;   

    // Game UI
    public Image[] UIHealth;
    public Text UIPoint;
    public Text UIStage;
    public Text ClearScore;
    public GameObject RestartBtn;

    public GameObject[] Stages;     // stage들을 관리함
    public GameObject ClearScreen;
    public GameObject Canvas; 
    
    // 메인 카메라와 서브 카메라
    public Camera MainC;
    public Camera SubC;
    public GameObject SubCamera;

    public AudioClip audioDie;
    

    public void PlaySound(string action){
        switch(action){
            case "DIE":
                audiosource.clip = audioDie;
                audiosource.Play();
                break;
            default:
                break;
        }
    }

    void Awake(){
       // player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        resetPos = new Vector2(player.transform.position.x, player.transform.position.y);
        audiosource = GetComponent<AudioSource>();
        
    }
    
    void Update(){
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    // 다음 스테이지로 이동
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
        player.transform.position = resetPos;
        }
        else{       // 게임 클리어
            //Time.timeScale = 0; // 시간을 멈춤
            Debug.Log("게임 클리어!");
            player.gameObject.SetActive(false);
            SubCamera.SetActive(true);
            SubOn();
            ClearScore.text = (totalPoint + stagePoint).ToString();
            Stages[stageIndex].SetActive(false);
            Canvas.SetActive(false);
            ClearScreen.SetActive(true);

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    // 추락 판정
        if(player.transform.position.y < -10){
            //player.transform.position = new Vector2(resetPos.x, resetPos.y);
            SemiRestart();
        }
    }

    // 게임 완전 재시작 (스테이지 all 리셋)
    public void Restart(){
        SceneManager.LoadScene("Scene 1");
    }

    // 진행 중인 스테이지 재시작 (해당 점수 스테이지 리셋)
    public void SemiRestart(){
        stagePoint -= 500;
        player.transform.position = new Vector2(resetPos.x, resetPos.y);
        //stagePoint = 0;
        
    }

    // 메인 카메라 온
    void MainOn(){
        MainC.enabled = true;
        SubC.enabled = false;
    }
    // 서브 카메라 온
    void SubOn(){
        MainC.enabled = false;
        SubC.enabled = true;

    }

    
}
