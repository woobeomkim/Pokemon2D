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
            // �����Ѽӵ��� �����϶� ���
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // yield �ڷ�ƾ�� ����ϱ����� ������
            // �������ӽ��� ���������ӿ� �����̵�
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

        // �ڽ�����ġ���� 1���� Ŀ���Ѵ� �ֳ��ϸ� �ڽ�����ġ���������ϸ� �ٷ��浹�� �Ǳ⶧����.
        /*
        Physics2D.BoxCast()�� �ڽ�(�簢��)�� Ư�� �������� �̵���Ű�鼭 �浹 �˻縦 �����ϴ� �Լ��Դϴ�.
        ��, ĳ���Ͱ� �̵��� ��ο� ��ֹ��� �ִ��� üũ�ϴ� ������ �մϴ�.
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
         xdiff, ydiff�� ����ؼ� ���� ��ġ�� ��ǥ ��ġ�� ���̸� ���
        if (xdiff == 0 || ydiff == 0)�� ���� 4����(��/��/��/��)�� �̵� �����ϵ��� ����
        MoveX, MoveY�� Mathf.Clamp(..., -1, 1)�� �����Ͽ� �� 1ĭ�� �̵��ϴ� ���� ����
         */

        // Ÿ�ϸ��̱⶧���� Ÿ�ϸ��� ������ �˰�ʹ�
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        // ĳ���ʹ� 4�������� �����ִ�
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
        // OverlapCirlce = targetPos��ġ�� raidus��ŭ�� ��������� �浹�� �˻��Ѵ� ����° ���̾ �����Ҽ��ִ�. ���н� null��ȯ
        // ������ �÷��̾ Ÿ�ϸ��� ���߾� 0.5,0.5������ �δ°� �Ϲ���������
        // ���ݴ� ���������� �ֱ����� ���߾Ӻ��� �ణ�� 0.5,0.8������ �θ� �� ���� ���Ӱ����� ������ִ�.
        // �̷����ϸ� ��������Ŭ�� �������� ���۰� 0.2�� �ٿ���.. (���� 0.5 0.5 ��ġ�� ������ 0.3)
        if (Physics2D.OverlapCircle(targetPos , 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
            return false;

        return true;
    }

   
}
