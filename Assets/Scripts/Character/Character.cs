using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Character : MonoBehaviour
{
    public float moveSpeed;
    CharacterAnimator animator;

    public bool IsMoving
    {
        get; private set;
    }

    public CharacterAnimator Animator
    {
        get => animator;
    }
    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);

        var targetPos = transform.position;

        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

   
        if(!IsWalkable(targetPos))
            yield break;
        else
       IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            // 일정한속도로 움직일때 사용
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // yield 코루틴을 사용하기위한 연산자
            // 한프레임쉬고 다음프레임에 실행이됨
            yield return null;
        }

        transform.position = targetPos;
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    bool IsWalkable(Vector3 targetPos)
    {
        // OverlapCirlce = targetPos위치에 raidus만큼의 원을만들어 충돌을 검사한다 세번째 레이어를 설정할수있다. 실패시 null반환
        // 보통은 플레이어를 타일맵의 정중앙 0.5,0.5단위로 두는게 일반적이지만
        // 조금더 나은느낌을 주기위해 정중앙보다 약간위 0.5,0.8단위로 두면 더 좋은 게임경험을 만들수있다.
        // 이렇게하면 오버랩서클의 반지름을 더작게 0.2로 줄였다.. (기존 0.5 0.5 위치의 반지름 0.3)
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
            return false;

        return true;
    }

   
}
