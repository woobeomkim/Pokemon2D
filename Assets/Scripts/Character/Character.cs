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

    bool IsWalkable(Vector3 targetPos)
    {
        // OverlapCirlce = targetPos��ġ�� raidus��ŭ�� ��������� �浹�� �˻��Ѵ� ����° ���̾ �����Ҽ��ִ�. ���н� null��ȯ
        // ������ �÷��̾ Ÿ�ϸ��� ���߾� 0.5,0.5������ �δ°� �Ϲ���������
        // ���ݴ� ���������� �ֱ����� ���߾Ӻ��� �ణ�� 0.5,0.8������ �θ� �� ���� ���Ӱ����� ������ִ�.
        // �̷����ϸ� ��������Ŭ�� �������� ���۰� 0.2�� �ٿ���.. (���� 0.5 0.5 ��ġ�� ������ 0.3)
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
            return false;

        return true;
    }

   
}
