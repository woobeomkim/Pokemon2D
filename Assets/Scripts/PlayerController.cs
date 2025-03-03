using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
   TilemapCollider -> �� Ÿ�ϸ��� �浹ü�� ������� ( ���ɺ��ϰ�ŭ)
   CompositeCollider -> �� �浹ü�� �ϳ��� ū�浹ü�� ����� ���ɺ��ϸ� ����
   Rigidbody static���� -> ��������������� �浹�Ҽ��ִ� ���ҷθ������.
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
            // GetAxis �� 0~ +-1���� �������������� 
            // GetAXisRaw�� 0~ +=1�� �ٷ�����
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

    // IEnumerator �Լ��� ������ �ٽý����ϱ����� ���������� �����ϱ����� �������̽�
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        { 
            // �����Ѽӵ��� �����϶� ���
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // yield �ڷ�ƾ�� ����ϱ����� ������
            // �������ӽ��� ���������ӿ� �����̵�
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        CheckForEncounters();
    }

    bool IsWalkable(Vector3 targetPos)
    {
        // OverlapCirlce = targetPos��ġ�� raidus��ŭ�� ��������� �浹�� �˻��Ѵ� ����° ���̾ �����Ҽ��ִ�. ���н� null��ȯ
        // ������ �÷��̾ Ÿ�ϸ��� ���߾� 0.5,0.5������ �δ°� �Ϲ���������
        // ���ݴ� ���������� �ֱ����� ���߾Ӻ��� �ణ�� 0.5,0.8������ �θ� �� ���� ���Ӱ����� ������ִ�.
        // �̷����ϸ� ��������Ŭ�� �������� ���۰� 0.2�� �ٿ���.. (���� 0.5 0.5 ��ġ�� ������ 0.3)
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
                Debug.Log("�߻��� ���ϸ��� �����ߴ�!");
            }
        }
    }
}
