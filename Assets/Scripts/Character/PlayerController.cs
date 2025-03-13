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
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

       
    private Character character;
    private Vector2 input;

    public string Name
    {
        get => name;
    }

    public Sprite Sprite
    {
        get => sprite;
    }

    public Character Character => character;

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
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);
    
       foreach(var collider in colliders)
       {
           var trigerable = collider.GetComponent<IPlayerTriggerable>();
           if(trigerable != null)
            {
                trigerable.OnPlayerTriggered(this);
                break;
           }
      }
    }

}
