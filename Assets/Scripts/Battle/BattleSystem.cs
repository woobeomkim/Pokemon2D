using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 배틀 진행상태를 enum 으로 정의
public enum BattleState { Start, PlayerAction, PlayerMove,EnemyMove,Busy}

// 배트시스템 전체를 관리
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;

    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;
    int currentAction;
    int currentMove;
    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        // 배틀정보설정
        playerUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);

        enemyUnit.Setup();
        enemyHud.SetData(enemyUnit.Pokemon);

        //기술이름 설정
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        // yield return 코루틴함수를하면 이 코루틴 함수가 끝낼때까지 이함수가 기다림
        yield return dialogBox.TypeDialog($"야생의 {enemyUnit.Pokemon.Base.Name} (이)가 나타났다!");
        yield return new WaitForSeconds(1f);

        //플레이어 행동시작
        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("행동을 고르세요!"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        //UI업데이트
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    // 플레이어가 공격하고 적이 반격한다.
    IEnumerator PerformPlayerMove()
    {
        // 상태를 Busy로 바꿔 currentMove가 바뀌지않도록 한다.
        state = BattleState.Busy;

        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} (이)가 {move.Base.Name}을 사용했다!");
        yield return new WaitForSeconds(1.0f);
        // 데미지를 입는다 
        bool isFainted = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        StartCoroutine(enemyHud.UpdateHP());

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} (이)가 기절했다.");
        }
        else
        {
            // 적이 반격
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;
        // 적포켓몬 랜덤 공격

        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} (이)가 {move.Base.Name}을 사용했다!");
        yield return new WaitForSeconds(1.0f);
        // 데미지를 입는다 
        bool isFainted = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        StartCoroutine(playerHud.UpdateHP());

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} (이)가 기절했다.");
        }
        else
        {
            // 다시 플레이어 액션
            PlayerAction();
        }
    }

    private void Update()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    {
        // selector 업데이트
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        // UI업데이트
        dialogBox.UpdateActionSelection(currentAction);

        // 액션에따른 행동

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                PlayerMove();
            }
            else if (currentAction == 1)
            {
            }
        }
    }

    void HandleMoveSelection()
    {
        // selector 업데이트
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count-1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
                currentMove += 2;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        // UI 업데이트
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        // move선택
        if(Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());

        }
    }
}
