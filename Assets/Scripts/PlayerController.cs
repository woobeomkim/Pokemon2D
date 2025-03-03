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
    public bool isMoving;
    Animator animator;

    Vector2 input;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                
                Vector2 targetPos = transform.position;

                targetPos.x += input.x;
                targetPos.y += input.y;

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
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
    }

}
