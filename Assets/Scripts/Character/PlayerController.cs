using System;
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

    // ������Ʈ�ѷ����� state�������̿��� ���¸��ٲٷ��� 
    // �÷��̾���Ʈ�ѷ����� ������Ʈ�ѷ��� �����ؾߴµ� �̹� ������Ʈ�ѷ����� �÷��̾���Ʈ�ѷ��� ������ ���¿����� ������Ʈ���ϹǷ�
    // �÷��̾���� ������Ʈ�ѷ��� �����ϸ� ��ȯ������ �Ͼ�� �̸� �ذ��ϱ�����
    // �̺�Ʈ�� �����ϴ� ����� �������������̿��Ѵ�.

    /*
     ������ �����̶�?
    ������ ������� ��ü�� ���°� �ٲ� ��, �� ��ȭ�� �ٸ� ��ü���� �ڵ����� �˷��ִ� ���

    ��ü(Subject): � ���� �ϰ� �ִ� ��ü (��: ���ӿ��� ������ ����ϴ� Player)
    ������(Observer): ��ü�� ��ȭ�� ���� �����ϴ� ��ü (��: ������ ȭ�鿡 ǥ���ϴ� ScoreBoard)
    ��ü(���ΰ�)�� ���°� �ٲ��, �� ��ȭ�� �ڵ����� ���������� �˷��ֱ� ������ �������� ��ȭ�� �˰� �ǰ�, �׿� �°� ������ �մϴ�. ��, ���� ��ȭ�� ���� �� �ٸ� ��ü���� �ڵ����� �˸��� �޴� �ý����Դϴ�.
     */
    public event Action onEncountered;
    public event Action<Collider2D> OnEnterTrainersView;


    private Character character;
    private Vector2 input;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    // ������Ʈ�ѷ�Ŭ���� ���� �����ϰ�, ����Ƽ ������Ʈ�� �ڵ�ȣ������ʰ� �̸�����
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            // GetAxis �� 0~ +-1���� �������������� 
            // GetAXisRaw�� 0~ +=1�� �ٷ�����
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0.0f) input.y = 0.0f;

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }
        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    public void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        CheckForEncounters();
        CheckIfInTraninersView();
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.GrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                //false�μ����ؾ� encounter�ɶ� �ִϸ��̼�ȭ����������.
                character.Animator.IsMoving = false;
                onEncountered();
            }
        }
    }

    private void CheckIfInTraninersView()
    {
        // ������ �����ϱ����� ��ȯ�ϴ� �ݶ��̴������� ��ü��ã�ƿ���
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);

        if (collider != null)
        {
            character.Animator.IsMoving = false;
            OnEnterTrainersView?.Invoke(collider);
        }
    }
}
