using System;
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

    // 게임컨트롤러클래스 에서 관리하고, 유니티 업데이트로 자동호출되지않게 이름변경
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            // GetAxis 는 0~ +-1까지 점진적으로증가 
            // GetAXisRaw는 0~ +=1로 바로증가
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
