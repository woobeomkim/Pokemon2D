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

    // 게임컨트롤러에서 state패턴을이용해 상태를바꾸려면 
    // 플레이어컨트롤러에서 게임컨트롤러를 참조해야는데 이미 게임컨트롤러에서 플레이어컨트롤러를 참조해 상태에따라 업데이트를하므로
    // 플레이어에에서 게임컨트롤러를 참조하면 순환참조가 일어난다 이를 해결하기위해
    // 이벤트를 구독하는 방식인 옵저버패턴을이용한다.

    /*
     옵저버 패턴이란?
    옵저버 패턴은어떤 객체의 상태가 바뀔 때, 그 변화를 다른 객체에게 자동으로 알려주는 방식

    주체(Subject): 어떤 일을 하고 있는 객체 (예: 게임에서 점수를 기록하는 Player)
    옵저버(Observer): 주체의 변화를 보고 반응하는 객체 (예: 점수를 화면에 표시하는 ScoreBoard)
    주체(주인공)가 상태가 바뀌면, 그 변화를 자동으로 옵저버에게 알려주기 때문에 옵저버는 변화를 알게 되고, 그에 맞게 반응을 합니다. 즉, 상태 변화가 있을 때 다른 객체들이 자동으로 알림을 받는 시스템입니다.
     */
    public event Action onEncountered;
    public event Action<Collider2D> OnEnterTrainersView;


    private Character character;
    private Vector2 input;

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
        CheckForEncounters();
        CheckIfInTraninersView();
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.GrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                //false로설정해야 encounter될때 애니메이션화가되지않음.
                character.Animator.IsMoving = false;
                onEncountered();
            }
        }
    }

    private void CheckIfInTraninersView()
    {
        // 참조를 적게하기위해 반환하는 콜라이더값으로 객체를찾아오자
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);

        if (collider != null)
        {
            character.Animator.IsMoving = false;
            OnEnterTrainersView?.Invoke(collider);
        }
    }
}
