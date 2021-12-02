using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour
{
    [SerializeField]public GameManager gameManager;

    [SerializeField] Rigidbody2D rigid;
   // [SerializeField] float h;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] bool controllable;         // 플레이어 조작권한 여부

    // Sound
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioFinish;
    public AudioClip audioDie;

    AudioSource audioSource;
    
     // Sound를 상황마다 재생하기 위한 것
    void PlaySound(string action){
        switch(action){
            case "JUMP":
                audioSource.clip = audioJump;
                audioSource.Play();
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                audioSource.Play();
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                audioSource.Play();
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                audioSource.Play();
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                audioSource.Play();
                break;
            case "DIE":
                audioSource.clip = audioDie;
                audioSource.Play();
                break;
            default:
                break;
        }
    }

    void Awake()
    {
        controllable = true;
        maxSpeed = 10;
        jumpPower = 30;
     //  h = 0;
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    // Sound AudioSource 를 담을 변수
        audioSource = GetComponent<AudioSource>();
        
    }
    
    void Update()       // 단발적인 입력은 update에
    {
        // JUMP
        if(Input.GetButtonDown("Jump") && !animator.GetBool("isJumping") && controllable){
            animator.SetBool("isJumping",true);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        // Jump 효과음 재생
            PlaySound("JUMP");
        }


        if(Input.GetButtonUp("Horizontal")){
            // stop speed
            rigid.velocity = new Vector2(3f * rigid.velocity.normalized.x, rigid.velocity.y);
        }

        // 방향전환
        if(Input.GetButton("Horizontal") && controllable){
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        // 걷기 애니메이션
        if(Math.Abs(rigid.velocity.x) <= 0.5f){
            animator.SetBool("isWalking", false);
        }else{
            animator.SetBool("isWalking",true);
        }
    }

    void FixedUpdate()
    {
        Debug.Log(rigid.velocity.y);

        float h = Input.GetAxisRaw("Horizontal");
        if(controllable)        // 조작권한이 있을때만 이동가능
             rigid.AddForce(Vector2.right * h * rigid.gravityScale, ForceMode2D.Impulse); // gravity의 영향으로 경사를 못올라가는것을 상쇄
            //Debug.Log(rigid.velocity);
        

        if(rigid.velocity.x > maxSpeed)         //Right
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if(rigid.velocity.x < maxSpeed*(-1))    //Left
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        // 레이케스트
        // Landing Platform
         //Debug.DrawRay(rigid.position, Vector3.down, new Color(0,1,0)); // 그냥 시각적으로 scene에 ray가 보이도록 그려주는것
        // Gizmos.DrawWireCube(rigid.position, new Vector3(1, 0.05f,0));
        // 레이캐스트 히트
       // RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));   // rayHit에 맞은 오브젝트에 대한 정보, 4번째인자는 레이어마스크 = 이것에 대한 충돌만 감지하겠다
        RaycastHit2D rayHit = Physics2D.BoxCast(new Vector2(rigid.position.x, rigid.position.y), new Vector2(0.8f, 1f), 0f, Vector2.down, 0.1f, LayerMask.GetMask("Platform"));

       if(rigid.velocity.y <= 0.2){
            if(rayHit.collider != null){ // 안맞으면 null 맞으면 collider 정보가 들어가있다
            // if(rayHit.distance < 0.5f){ // 거리가 0.5f 미만이면 ( 중앙에서 ray가 시작하므로 0.5 , 캐릭터크기는 1픽셀이다, 단 콜라이더 기준)
                   // Debug.Log(rayHit.collider.name);
                    animator.SetBool("isJumping",false);
              }//else animator.SetBool("isJumping",true);
        }

    }

// enemy와 충돌이벤트
    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Enemy"){
            //Debug.Log("플레이어가 맞았습니다");                                                            // 플랫폼에 정확한 좌표로 떨어지는게 아니기에 차이를 줘야함
            if(rigid.velocity.y <0 && transform.position.y - collision.transform.position.y >= 0.3f){              // 몬스터 위에있음 + 아래 낙하중 = 밟음
                OnAttack(collision.transform);
                // Point
                
            }
            else
                OnDamaged(collision.transform.position);

        }
    }

// 데미지를 받았을 때
    void OnDamaged(Vector2 targetPos){
        StartCoroutine("ControlTaken",0.3f);   // 지정한 함수를 수행함 (IEnumerator 한정)
        rigid.velocity = new Vector2(0,rigid.velocity.y);       // 운동값 초기화
        gameObject.layer = 10;                                  // 자기자신의 레이어를 10번으로 바꿈 (PlayerDamaged)
        spriteRenderer.color = new Color(1,1,1,0.4f);           // 4번째거는 alpha값

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;  // 맞은 위치가 어느방향인가를 판별
        Debug.Log(transform.position.x - targetPos.x);
        rigid.AddForce(new Vector2(dirc*10,8), ForceMode2D.Impulse);  // 맞고 튕겨나가는것 구현
        PlaySound("DAMAGED");   // damage sound

        animator.SetTrigger("doDamaged");   //animator의 doDamaged 트리거를 ON 함
        Invoke("OffDamaged", 3);        // 3초후 무적풀림
    }

// 몬스터를 공격
    void OnAttack(Transform enemy){
        //Point
        gameManager.stagePoint += 50;
        //Enemy Die
        Monster enemyMove = enemy.GetComponent<Monster>();  // 객체.GetComponent<스크립트>();
        enemyMove.OnDamaged();

        //reaction
        PlaySound("ATTACK");
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up*15, ForceMode2D.Impulse);
    }

    IEnumerator ControlTaken(float ntime){           // 플레이어 컨트롤 강탈 + 돌려줌 -> 추가로 코루틴으로 딜레이주기
        controllable = false;
        //Debug.Log("delaying");
        
        yield return new WaitForSeconds((float)ntime);    //3초 세고 다시 여기서부터 이함수를 수행
        //Debug.Log("End Delay");
        controllable = true;
    }

    void OffDamaged(){
        gameObject.layer = 9;
        spriteRenderer.color = new Color(1,1,1,1);
    }

    // 아이템처리, gamemange
    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Item"){
            // Point
            bool isBronze = other.gameObject.name.Contains("Bronze");       // 해당 문자열이 있으면 true
            bool isSilver = other.gameObject.name.Contains("Silver");
            bool isGold = other.gameObject.name.Contains("Gold");

            if(isBronze)
                gameManager.stagePoint += 50;
            else if(isSilver)
                gameManager.stagePoint += 100;
            else if(isGold)
                gameManager.stagePoint += 300;

            // Decative Item
            PlaySound("ITEM");
            other.gameObject.SetActive(false);
        }
        else if(other.gameObject.tag == "Finish"){
            // next stage
            PlaySound("FINISH");
            gameManager.NextStage();

        }
        
    }
}
