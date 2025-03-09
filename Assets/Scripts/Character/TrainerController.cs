using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    
    Character character;

    public string Name
    {
        get => name;
    }

    public Sprite Sprite
    {
        get => sprite;
    }

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }
    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        // 느낌표표시
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        //플레이어쪽으로 걷게하기
        var diff = player.transform.position - transform.position;
        // 플레이어쪽으로 향하는 방향벡터를 빼주지않으면 겹치게이동한다
        var moveVec = diff - diff.normalized;
        //타일맵이므로 정수좌표를원한다
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // 대화하기
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => { GameController.Instance.StartTrainerBattle(this); }));

    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}
