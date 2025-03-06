using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// 배틀 진행상태를 enum 으로 정의
public enum BattleState { Start, ActionSelection, MoveSelection ,PerformMove,Busy,PartyScreen,BattleOver}

// 배트시스템 전체를 관리
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> onBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    PokemonParty playerParty;
    Pokemon wildPokemon;

    public void BattleStart(PokemonParty playerPary, Pokemon wildPokemon)
    {
        this.playerParty = playerPary;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        // 배틀정보설정
        playerUnit.Setup(playerParty.GetHealthPokemon());

        enemyUnit.Setup(wildPokemon);
  
        partyScreen.Init();
        //기술이름 설정
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        // yield return 코루틴함수를하면 이 코루틴 함수가 끝낼때까지 이함수가 기다림
        yield return dialogBox.TypeDialog($"야생의 {enemyUnit.Pokemon.Base.Name} (이)가 나타났다!");

        //플레이어 행동시작

        ChooseFirstTurn();
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        onBattleOver(won);
    }

    void ChooseFirstTurn()
    {
        if (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed)
        {
            ActionSelection();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("행동을 고르세요!");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }
    void MoveSelection()
    {
        //UI업데이트
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    // 플레이어가 공격하고 적이 반격한다.
    IEnumerator PlayerMove()
    {
        // 상태를 Busy로 바꿔 currentMove가 바뀌지않도록 한다.
        state = BattleState.PerformMove;

        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);
        
        // 전투스탯이 RunMove에의해 변경되지않았으면 다음스텝으로 간다
        if(state==BattleState.PerformMove)
            StartCoroutine(EnemyMove());
      
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;
        // 적포켓몬 랜덤 공격

        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        // 다시 플레이어 액션
        // 전투스탯이 RunMove에의해 변경되지않았으면 다음스텝으로 간다
        if (state == BattleState.PerformMove)
            ActionSelection();
        
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} (이)가 {move.Base.Name}을 사용했다!");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1.0f);

        targetUnit.PlayHitAnimation();

        if (move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, sourceUnit.Pokemon,targetUnit.Pokemon);

        }
        else
        { // 데미지를 입는다 
            var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
            yield return targetUnit.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        if (targetUnit.Pokemon.HP <=0)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} (이)가 기절했다.");
            targetUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            CheckForBattleOver(targetUnit);
        }


        // 화상이나 독 상태에 걸려 턴이후 데미지를 입는지
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.UpdateHP();

        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} (이)가 기절했다.");
            sourceUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            CheckForBattleOver(sourceUnit);
        }
    }

    IEnumerator RunMoveEffects(Move move , Pokemon sourceUnit, Pokemon targetUnit)
    {
        var effects = move.Base.Effects;

        // Stat Boosting
        if (effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
                sourceUnit.ApplyBoosts(effects.Boosts);
            else
                targetUnit.ApplyBoosts(effects.Boosts);
        }

        //Status Condition
        if(effects.Status != ConditionID.none)
        {
            targetUnit.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(sourceUnit);
        yield return ShowStatusChanges(targetUnit);
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while(pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if(faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthPokemon();
            if (nextPokemon != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            BattleOver(true);
        }
    }

    // 크리티컬공격이 됏을때 다이어로그박스 함수
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("급소를 공격했다!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("효과가 좋은것 같다!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("효과가 없는것 같다!");
    }

    // 게임컨트롤러에서 관리하기위해 핸들업데이트로 변경
    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if(state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    void HandleActionSelection()
    {
        // selector 업데이트
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        //Clamp를 활용한 0~3보정
        currentAction = Mathf.Clamp(currentAction, 0, 3);
        // UI업데이트
        dialogBox.UpdateActionSelection(currentAction);

        // 액션에따른 행동

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
            }
            else if (currentAction == 2)
            {
                // Pokemon
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Run
            }
        }
    }

    void HandleMoveSelection()
    {
        // selector 업데이트
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        //Clamp를 활용한 0~3보정
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        // UI 업데이트
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        // move선택
        if(Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());

        }
        //X를 누르면 다시 Move선택창으로
        else if(Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemeberSelection(currentMember);

        if(Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if(selectedMember.HP <=0)
            {
                partyScreen.SetMessageText("기절한 포켓몬은 선택할수 없습니다!");
                return;
            }
            if(selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("같은 포켓몬으로 바꿀수 없습니다!");
                return;
            }
        
            partyScreen.gameObject.SetActive(false);

            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
        }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        dialogBox.EnableActionSelector(false);
        bool currentPokemonFainted = true;
        if (playerUnit.Pokemon.HP > 0)
        {
            currentPokemonFainted = false;
            yield return dialogBox.TypeDialog($"돌아와! {playerUnit.Pokemon.Base.Name}");
            // TODO 복귀애니메이션 Faint 임시사용
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2.0f);
        }
        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"가라! {newPokemon.Base.Name}");

        if(currentPokemonFainted)
            ChooseFirstTurn();
        else
             StartCoroutine(EnemyMove());
    }
}
