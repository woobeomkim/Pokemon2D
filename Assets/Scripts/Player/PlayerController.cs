using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
   TilemapCollider -> 각 타일마다 충돌체를 만들어줌 ( 성능부하가큼)
   CompositeCollider -> 각 충돌체를 하나의 큰충돌체로 만들어 성능부하를 줄임
   Rigidbody static설정 -> 물리계산기능을꺼서 충돌할수있는 역할로만만든다.
 */
public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask grassLayer;

    // 게임컨트롤러에서 state패턴을이용해 상태를바꾸려면 
    // 플레이어컨트롤러에서 게임컨트롤러를 참조해야는데 이미 게임컨트롤러에서 플레이어컨트롤러를 참조해 상태에따라 업데이트를하므로
    // 플레이어에에서 게임컨트롤러를 참조하면 순환참조가 일어난다 이를 해결하기위해
    // 이벤트를 구독하는 방식인 옵저버패턴을이용한다.

    /*
     옵저버 패턴이란?
    옵저버 패턴은어떤 객체의 상태가 바뀔 때, 그 변화를 다른 객체에게 자동으로 알려주는 방식

    주체(Subject): 어떤 일을 하고 있는 객체 (예: 게임에서 점수를 기록하는 Player)
    옵저버(Observer): 주체의 변화를 보고 반응하는 객체 (예: 점수를 화면에 표시하는 ScoreBoard)
    주체(주인공)가 상태가 바뀌면, 그 변화를 자동으로 옵저버에게 알려주기 때문에 옵저버는 변화를 알게 되고, 그에 맞게 반응을 합니다. 즉, 상태 변화가 있을 때 다른 객체들이 자동으로 알림을 받는 시스템입니다.
     */
    public event Action onEncountered;

    private bool isMoving;
    private Animator animator;
    private Vector2 input;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // 게임컨트롤러클래스 에서 관리하고, 유니티 업데이트로 자동호출되지않게 이름변경
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            // GetAxis 는 0~ +-1까지 점진적으로증가 
            // GetAXisRaw는 0~ +=1로 바로증가
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0.0f) input.y = 0.0f;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                
                var targetPos = transform.position;

                targetPos.x += input.x;
                targetPos.y += input.y;

                if(IsWalkable(targetPos)) 
                    StartCoroutine(Move(targetPos));
            }
        }

        animator.SetBool("isMoving",isMoving);
    }

    // IEnumerator 함수가 멈춘후 다시실행하기위한 상태정보를 저장하기위한 인터페이스
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        { 
            // 일정한속도로 움직일때 사용
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // yield 코루틴을 사용하기위한 연산자
            // 한프레임쉬고 다음프레임에 실행이됨
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        CheckForEncounters();
    }

    bool IsWalkable(Vector3 targetPos)
    {
        // OverlapCirlce = targetPos위치에 raidus만큼의 원을만들어 충돌을 검사한다 세번째 레이어를 설정할수있다. 실패시 null반환
        // 보통은 플레이어를 타일맵의 정중앙 0.5,0.5단위로 두는게 일반적이지만
        // 조금더 나은느낌을 주기위해 정중앙보다 약간위 0.5,0.8단위로 두면 더 좋은 게임경험을 만들수있다.
        // 이렇게하면 오버랩서클의 반지름을 더작게 0.2로 줄였다.. (기존 0.5 0.5 위치의 반지름 0.3)
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null)
            return false;

        return true;
    }

    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position,0.2f,grassLayer)!=null)
        {
            if(UnityEngine.Random.Range(1,101) <=10)
            {
                //false로설정해야 encounter될때 애니메이션화가되지않음.
                animator.SetBool("isMoving", false);
                onEncountered();
            }
        }
    }
}
