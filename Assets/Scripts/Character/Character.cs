using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Character : MonoBehaviour
{
    public float moveSpeed;
    CharacterAnimator animator;

    public float OffsetY { get; private set; } = 0.3f;
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
        SetPositionAndSnapToTile(transform.position);
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;
        
        transform.position = pos;
    }

    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);

        var targetPos = transform.position;

        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        var ledge = CheckForLedge(targetPos);
   
        if(ledge != null)
        {
            if (ledge.TryToJump(this, moveVec))
                yield break;
        }

        if(!IsPathClear(targetPos))
            yield break;
      
        if(animator.IsSurfing && Physics2D.OverlapCircle(targetPos,0.3f,GameLayers.i.WaterLayer) == null)
            animator.IsSurfing = false;

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


    private bool IsPathClear(Vector3 targetPos) 
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        // 자신의위치보다 1보다 커야한다 왜냐하면 자신의위치에서시작하면 바로충돌이 되기때문에.
        /*
        Physics2D.BoxCast()는 박스(사각형)를 특정 방향으로 이동시키면서 충돌 검사를 수행하는 함수입니다.
        즉, 캐릭터가 이동할 경로에 장애물이 있는지 체크하는 역할을 합니다.
         */

        var collisionLayer = GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer;

        if (!animator.IsSurfing)
            collisionLayer = collisionLayer | GameLayers.i.WaterLayer;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, collisionLayer)) 
            return false;

        return true;
    }

    public void LookTowards(Vector3 targetPos)
    {
        /*
         xdiff, ydiff를 사용해서 현재 위치와 목표 위치의 차이를 계산
        if (xdiff == 0 || ydiff == 0)를 통해 4방향(좌/우/상/하)만 이동 가능하도록 제한
        MoveX, MoveY를 Mathf.Clamp(..., -1, 1)로 제한하여 딱 1칸씩 이동하는 값만 설정
         */

        // 타일맵이기때문에 타일맵의 개수를 알고싶다
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        // 캐릭터는 4방향으로 볼수있다
        if (xdiff == 0 || ydiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
            Debug.Log("Error in Look Toward : You can't ask the character to look diagnolly");

            
    }

    Ledge CheckForLedge(Vector3 targetPos)
    {
       var collider = Physics2D.OverlapCircle(targetPos, 0.15f, GameLayers.i.LedgeLayer);
       return collider?.GetComponent<Ledge>();
    }

    bool IsWalkable(Vector3 targetPos)
    {
        // OverlapCirlce = targetPos위치에 raidus만큼의 원을만들어 충돌을 검사한다 세번째 레이어를 설정할수있다. 실패시 null반환
        // 보통은 플레이어를 타일맵의 정중앙 0.5,0.5단위로 두는게 일반적이지만
        // 조금더 나은느낌을 주기위해 정중앙보다 약간위 0.5,0.8단위로 두면 더 좋은 게임경험을 만들수있다.
        // 이렇게하면 오버랩서클의 반지름을 더작게 0.2로 줄였다.. (기존 0.5 0.5 위치의 반지름 0.3)
        if (Physics2D.OverlapCircle(targetPos , 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
            return false;

        return true;
    }

   
}
