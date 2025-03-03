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

    private bool isMoving;
    private Animator animator;
    private Vector2 input;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
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
            if(Random.Range(1,101) <=10)
            {
                Debug.Log("야생의 포켓몬이 등장했다!");
            }
        }
    }
}
