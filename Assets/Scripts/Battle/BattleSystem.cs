using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;


// 배틀 진행상태를 enum 으로 정의
public enum BattleState { Start, ActionSelection, MoveSelection ,RunningTurn,Busy,PartyScreen,AboutToUse ,BattleOver}
public enum BattleAction { Move,SwitchPokemon,UseItem,Run}
// 배트시스템 전체를 관리
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit; //플레이어유닛
    [SerializeField] BattleUnit enemyUnit; // 적유닛
    [SerializeField] BattleDialogBox dialogBox; // 배틀다이어로그박스
    [SerializeField] PartyScreen partyScreen; // 파티화면
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;

    public event Action<bool> onBattleOver; //배틀종료이벤트

    BattleState state; // 현재배틀상태
    BattleState? prevState; // 이전 상태 저장
    int currentAction; // 현재 선택된행동
    int currentMove; // 현재 선택된 기술
    int currentMember; // 현재 선택된 파티 멤버
    bool aboutToUseChoice;

    PokemonParty playerParty; // 플레이어파티
    PokemonParty trainerParty; // 플레이어파티
    Pokemon wildPokemon; // 야생포켓몬

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    // 배틀시작메서드
    public void StartBattle(PokemonParty playerPary, Pokemon wildPokemon)
    {
        isTrainerBattle = false;
        this.playerParty = playerPary;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerPary, PokemonParty trainerParty)
    {
        this.playerParty = playerPary;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();


        StartCoroutine(SetupBattle());
    }

    // 배틀설정 코루틴
    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattle)
        {
            // Wild Pokemon Battle
            // 배틀정보설정
            playerUnit.Setup(playerParty.GetHealthPokemon());

            enemyUnit.Setup(wildPokemon); // 적포켓몬배치
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);     //기술이름 설정
            // yield return 코루틴함수를하면 이 코루틴 함수가 끝낼때까지 이함수가 기다림
            yield return dialogBox.TypeDialog($"야생의 {enemyUnit.Pokemon.Base.Name} (이)가 나타났다!");

        }
        else
        {
            // Trainer Battle

            // Player,Trainer Image show
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;
            yield return dialogBox.TypeDialog($"{trainer.Name}(이)가 배틀을 걸어왔다!");

            // 트레이너 포켓몬 꺼내기
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{trainer.Name}(이)가 {enemyPokemon.Base.name}을 꺼냈다!");

            // 플레이어포켓몬
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"가라 {playerPokemon.Base.name}!");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }
        partyScreen.Init(); // 파티화면초기화
        //플레이어 행동시작

        ActionSelection();
    }

    // 배틀 종료처리
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver()); // 모든포켓몬 상태 초기화
        onBattleOver(won); // 배틀종료이벤트
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

    IEnumerator AboutToUse(Pokemon newPokemon)
    {
        state = BattleState.Busy;
        yield return StartCoroutine(dialogBox.TypeDialog($"{trainer.Name}(이)가 {newPokemon.Base.Name}을 사용하려합니다. 포켓몬을 바꾸시겠습니까 ?"));
        state = BattleState.AboutToUse;

        dialogBox.EnableChoiceBox(true);
    }

    // 턴 진행을 담당하는 코루틴
    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            // 플레이어와 적의 기술 설정
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            // 우선순위를통한 선공결정
            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            // 누가먼저 선인지 체크
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed);

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;

            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondPokemon.HP > 0)
            {

                // Second Turn

                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = playerParty.Pokemons[currentMember];
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }

            // EnumyTurn

            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;

        }

        if (state != BattleState.BattleOver)
            ActionSelection();

    }



    IEnumerator EnemyMove()
    {
        state = BattleState.RunningTurn;
        // 적포켓몬 랜덤 공격

        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        // 다시 플레이어 액션
        // 전투스탯이 RunMove에의해 변경되지않았으면 다음스텝으로 간다
        if (state == BattleState.RunningTurn)
            ActionSelection();

    }

    // 기술실행처리
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        // 기술을 사용하기 전에 상태이상(마비, 얼음 등)으로 인해 행동 불가능할 수도 있음
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        // 상태 변화 출력
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        move.PP--;
        // 공격 실행 메시지 출력
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} (이)가 {move.Base.Name}을 사용했다!");

        // 명중 여부 체크
        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            // 공격 애니메이션 실행

            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1.0f);
            // 피격 애니메이션 실행
            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                // 상태 변화 기술 (공격이 아닌 기술) 처리
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);

            }
            else
            {   // 데미지를 입는다 
                // 데미지 계산 후 HP 감소
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }
            // 부가 효과 처리 (추가 상태 이상 등)
            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                }
            }
            // 목표 포켓몬이 기절했다면 메시지 출력 및 처리
            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} (이)가 기절했다.");
                targetUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(2f);
                // 전투 종료 여부 확인
                CheckForBattleOver(targetUnit);
            }

        }
        else
        { // 공격이 빗나갔을 경우 메시지 출력
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.name} 의 공격이 빗나갔습니다!");
        }

    }

    // 기술 효과 적용 처리
    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon sourceUnit, Pokemon targetUnit, MoveTarget moveTarget)
    {
        // Stat Boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                sourceUnit.ApplyBoosts(effects.Boosts);
            else
                targetUnit.ApplyBoosts(effects.Boosts);
        }

        //Status Condition
        if (effects.Status != ConditionID.none)
        {
            targetUnit.SetStatus(effects.Status);
        }

        //VolatileStatus Condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            targetUnit.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(sourceUnit);
        yield return ShowStatusChanges(targetUnit);
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

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
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwayHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Accuracy];

        // 실제사용되는 부스트값
        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];



        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
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
            if (!isTrainerBattle)
                BattleOver(true);
            else
            {
                var nextPokemon = trainerParty.GetHealthPokemon();
                if (nextPokemon != null)
                    // Send Out Pokemon
                    StartCoroutine(AboutToUse(nextPokemon));
                else
                    BattleOver(true);
            }
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
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if(state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
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
                prevState = state;
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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.Pokemon.Moves[currentMove];
            if (move.PP == 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));

        }
        //X를 누르면 다시 Move선택창으로
        else if (Input.GetKeyDown(KeyCode.X))
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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("기절한 포켓몬은 선택할수 없습니다!");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("같은 포켓몬으로 바꿀수 없습니다!");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (playerUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("계속하려면 포켓몬 고르세요!");
                return;
            }


            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.AboutToUse)
            {
                prevState = null;
                StartCoroutine(SendNextTrainerPokemon());
            }
            else
                ActionSelection();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        dialogBox.EnableActionSelector(false);
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"돌아와! {playerUnit.Pokemon.Base.Name}");
            // TODO 복귀애니메이션 Faint 임시사용
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2.0f);
        }
        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"가라! {newPokemon.Base.Name}");

        state = BattleState.RunningTurn;

        if (prevState == null)
        {

            state = BattleState.RunningTurn;
        }
        else if (prevState == BattleState.AboutToUse)
        {
            prevState = null;
            StartCoroutine(SendNextTrainerPokemon());
        }

    }


    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUseChoice = !aboutToUseChoice;

        dialogBox.UpdateChoiceBox(aboutToUseChoice);
    
        if(Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableChoiceBox(false);
            if(aboutToUseChoice)
            {
                // YES
                prevState = BattleState.AboutToUse;
                OpenPartyScreen();
            }
            else
            {
                StartCoroutine(SendNextTrainerPokemon());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerPokemon());
        }

    }
    IEnumerator SendNextTrainerPokemon()
    {
        state = BattleState.Busy;


        var nextPokemon = trainerParty.GetHealthPokemon();
        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{trainer.Name}(이)가 {nextPokemon.Base.Name}을 꺼냈다!");

        if (prevState == null)
        {

            state = BattleState.RunningTurn;
        }
        else if (prevState == BattleState.AboutToUse)
        {
            prevState = null;
            StartCoroutine(SendNextTrainerPokemon());
        }
    }
}
