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

    // IEnumerator �Լ��� ������ �ٽý����ϱ����� ���������� �����ϱ����� �������̽�
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
