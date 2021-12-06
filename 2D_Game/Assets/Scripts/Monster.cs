using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Monster : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Animator animator;
    [SerializeField] public int nextMove;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] BoxCollider2D collider2d;
    [SerializeField] bool isLive;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        collider2d = GetComponent<BoxCollider2D>();
        isLive = true;
        Think();

        Invoke("Think", 5); //5초뒤에 Think 함수 호출
    }

    
    void FixedUpdate()
    {
        // 기본이동
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y );

        // Platform Check
        Vector2 frontVec = new Vector2((float)rigid.position.x + (float)nextMove/2, rigid.position.y); // 자기보다 nexMove/2 픽셀만큼 앞에 그려지게
        Debug.DrawRay(frontVec, Vector3.down, new Color(0,1,0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 2, LayerMask.GetMask("Platform"));
        if(rayHit.collider == null && isLive){        // 몬스터 앞에 그려진 레이가 바닥에 충돌하지 않을경우 진행경로를 바꾸게
            Turn();
        }
    }

    void Update() {
        
    }

    // 재귀함수 + 딜레이주기
    void Think(){
        nextMove = UnityEngine.Random.Range(-1, 2);

        Invoke("Think", 5);

        // Sprite 애니메이션
        animator.SetInteger("WalkSpeed", nextMove);

        if(nextMove != 0)   // 몬스터의 방향 (flipX)
            spriteRenderer.flipX = (nextMove == 1);
    }

    // 방향전환 함수
    void Turn(){
        nextMove *= -1;
        spriteRenderer.flipX = nextMove==1;

        CancelInvoke();
        Invoke("Think", 5);
    }

    public void OnDamaged(){
        CancelInvoke();
        isLive = false;
        nextMove = 0;
        animator.SetInteger("WalkSpeed", nextMove);
        
        // 몬스터가 죽었을때 하는 행동들
        // 투명화
        spriteRenderer.color = new Color(1,1,1,0.4f);
        // 스프라이트 y축 플립
        spriteRenderer.flipY = true;
        // collider disabled
        collider2d.enabled = false;
        // Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // Destroy
        Invoke("DeActive",2);

    }
    void DeActive(){
        gameObject.SetActive(false);
    }
}
