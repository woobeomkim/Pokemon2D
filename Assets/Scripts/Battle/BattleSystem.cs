using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;


// ��Ʋ ������¸� enum ���� ����
public enum BattleState { Start, ActionSelection, MoveSelection ,RunningTurn,Busy,PartyScreen,AboutToUse , MoveForget,BattleOver}
public enum BattleAction { Move,SwitchPokemon,UseItem,Run}
// ��Ʈ�ý��� ��ü�� ����
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit; //�÷��̾�����
    [SerializeField] BattleUnit enemyUnit; // ������
    [SerializeField] BattleDialogBox dialogBox; // ��Ʋ���̾�α׹ڽ�
    [SerializeField] PartyScreen partyScreen; // ��Ƽȭ��
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] MoveSelectionUI moveSelectionUI;

    public event Action<bool> onBattleOver; //��Ʋ�����̺�Ʈ

    BattleState state; // �����Ʋ����
    BattleState? prevState; // ���� ���� ����
    int currentAction; // ���� ���õ��ൿ
    int currentMove; // ���� ���õ� ���
    int currentMember; // ���� ���õ� ��Ƽ ���
    bool aboutToUseChoice;

    PokemonParty playerParty; // �÷��̾���Ƽ
    PokemonParty trainerParty; // �÷��̾���Ƽ
    Pokemon wildPokemon; // �߻����ϸ�

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    int escapeAttempts;
    MoveBase moveToLearn;

    // ��Ʋ���۸޼���
    public void StartBattle(PokemonParty playerPary, Pokemon wildPokemon)
    {
        isTrainerBattle = false;
        this.playerParty = playerPary;
        this.wildPokemon = wildPokemon;

        player = playerParty.GetComponent<PlayerController>();
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

    // ��Ʋ���� �ڷ�ƾ
    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattle)
        {
            // Wild Pokemon Battle
            // ��Ʋ��������
            playerUnit.Setup(playerParty.GetHealthPokemon());

            enemyUnit.Setup(wildPokemon); // �����ϸ��ġ
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);     //����̸� ����
            // yield return �ڷ�ƾ�Լ����ϸ� �� �ڷ�ƾ �Լ��� ���������� ���Լ��� ��ٸ�
            yield return dialogBox.TypeDialog($"�߻��� {enemyUnit.Pokemon.Base.Name} (��)�� ��Ÿ����!");

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
            yield return dialogBox.TypeDialog($"{trainer.Name}(��)�� ��Ʋ�� �ɾ�Դ�!");

            // Ʈ���̳� ���ϸ� ������
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{trainer.Name}(��)�� {enemyPokemon.Base.name}�� ���´�!");

            // �÷��̾����ϸ�
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"���� {playerPokemon.Base.name}!");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }

        escapeAttempts = 0;
        partyScreen.Init(); // ��Ƽȭ���ʱ�ȭ
        //�÷��̾� �ൿ����

        ActionSelection();
    }

    // ��Ʋ ����ó��
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver()); // ������ϸ� ���� �ʱ�ȭ
        onBattleOver(won); // ��Ʋ�����̺�Ʈ
    }


    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("�ൿ�� ������!");
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
        //UI������Ʈ
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator AboutToUse(Pokemon newPokemon)
    {
        state = BattleState.Busy;
        yield return StartCoroutine(dialogBox.TypeDialog($"{trainer.Name}(��)�� {newPokemon.Base.Name}�� ����Ϸ��մϴ�. ���ϸ��� �ٲٽðڽ��ϱ� ?"));
        state = BattleState.AboutToUse;

        dialogBox.EnableChoiceBox(true);
    }

    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return StartCoroutine(dialogBox.TypeDialog($"�ؾ���� ����� �������ּ��� !"));
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveForget;
    }

    // �� ������ ����ϴ� �ڷ�ƾ
    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            // �÷��̾�� ���� ��� ����
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            // �켱���������� ��������
            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            // �������� ������ üũ
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
            else if(playerAction == BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
                yield return ThrowPokeball();
            }
            else if (playerAction == BattleAction.Run)
            {
                dialogBox.EnableActionSelector(false);
                yield return TryToEscape();
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
        // �����ϸ� ���� ����

        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        // �ٽ� �÷��̾� �׼�
        // ���������� RunMove������ ��������ʾ����� ������������ ����
        if (state == BattleState.RunningTurn)
            ActionSelection();

    }

    // �������ó��
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        // ����� ����ϱ� ���� �����̻�(����, ���� ��)���� ���� �ൿ �Ұ����� ���� ����
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        // ���� ��ȭ ���
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        move.PP--;
        // ���� ���� �޽��� ���
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} (��)�� {move.Base.Name}�� ����ߴ�!");

        // ���� ���� üũ
        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            // ���� �ִϸ��̼� ����

            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1.0f);
            // �ǰ� �ִϸ��̼� ����
            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                // ���� ��ȭ ��� (������ �ƴ� ���) ó��
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);

            }
            else
            {   // �������� �Դ´� 
                // ������ ��� �� HP ����
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }
            // �ΰ� ȿ�� ó�� (�߰� ���� �̻� ��)
            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                }
            }
            // ��ǥ ���ϸ��� �����ߴٸ� �޽��� ��� �� ó��
            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }

        }
        else
        { // ������ �������� ��� �޽��� ���
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.name} �� ������ ���������ϴ�!");
        }

    }

    // ��� ȿ�� ���� ó��
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

        // ȭ���̳� �� ���¿� �ɷ� ������ �������� �Դ���
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.UpdateHP();

        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
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

        // �������Ǵ� �ν�Ʈ��
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

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {

        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} (��)�� �����ߴ�.");
        faintedUnit.PlayFaintAnimation();

        yield return new WaitForSeconds(2f);
        
        if(!faintedUnit.IsPlayerUnit)
        {
            //EXP Gain
            int expYield = faintedUnit.Pokemon.Base.ExpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit.Pokemon.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} (��)�� {expGain} �� ����ġ�� ȹ���ߴ�!");
            yield return playerUnit.Hud.SetExpSmooth();
            // Check Level up
            // 2������ 3������ �Ҽ��� ������ while������
            while(playerUnit.Pokemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} (��)�� {playerUnit.Pokemon.Level}�� �Ǿ���!");

                // Try to Learn a new Move
                var newMove = playerUnit.Pokemon.GetLearnableMoveAtCurrLevel();
                if( newMove != null)
                {
                    if (playerUnit.Pokemon.Moves.Count  < PokemonBase.MaxNumOfMvoes)
                    {
                        playerUnit.Pokemon.LearnMove(newMove);
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} (��)�� {newMove.Base.Name}�� �����!");
                        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                    }
                    else
                    {
                        // TODO ����Ѱ� ����� ���������
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} (��)�� {newMove.Base.Name}�� �������Ѵ�!");
                        yield return dialogBox.TypeDialog($"�׷��� 4���̻��� ����� ������ ����!");
                        yield return ChooseMoveToForget(playerUnit.Pokemon, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState.MoveForget);
                        yield return new WaitForSeconds(2.0F);
                    }
                }

                yield return playerUnit.Hud.SetExpSmooth(true);
            }


            yield return new WaitForSeconds(1.0F);
        }
        // ���� ���� ���� Ȯ��
        CheckForBattleOver(faintedUnit);
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

    // ũ��Ƽ�ð����� ������ ���̾�α׹ڽ� �Լ�
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("�޼Ҹ� �����ߴ�!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("ȿ���� ������ ����!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("ȿ���� ���°� ����!");
    }

    // ������Ʈ�ѷ����� �����ϱ����� �ڵ������Ʈ�� ����
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
        else if(state == BattleState.MoveForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if(moveIndex == PokemonBase.MaxNumOfMvoes)
                {
                    // ����� ������ʴ´�
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} (��)�� {moveToLearn.Name}�� ����� �ʾҴ�!"));
                }
                else
                {
                    // ����� �ذ� ���ο���������
                    var selectedMove = playerUnit.Pokemon.Moves[moveIndex].Base;

                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} (��)�� {selectedMove.Name}�� �ذ� {moveToLearn.Name}�� �����!"));                    
                    playerUnit.Pokemon.Moves[moveIndex] = new Move(moveToLearn);
                }

                moveToLearn = null;
                state = BattleState.RunningTurn;
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
     
    }

    void HandleActionSelection()
    {
        // selector ������Ʈ
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        //Clamp�� Ȱ���� 0~3����
        currentAction = Mathf.Clamp(currentAction, 0, 3);
        // UI������Ʈ
        dialogBox.UpdateActionSelection(currentAction);

        // �׼ǿ����� �ൿ

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
                StartCoroutine(RunTurns(BattleAction.UseItem));
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
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }

    void HandleMoveSelection()
    {
        // selector ������Ʈ
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        //Clamp�� Ȱ���� 0~3����
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        // UI ������Ʈ
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        // move����
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.Pokemon.Moves[currentMove];
            if (move.PP == 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));

        }
        //X�� ������ �ٽ� Move����â����
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
                partyScreen.SetMessageText("������ ���ϸ��� �����Ҽ� �����ϴ�!");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("���� ���ϸ����� �ٲܼ� �����ϴ�!");
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
                partyScreen.SetMessageText("����Ϸ��� ���ϸ� ������!");
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
            yield return dialogBox.TypeDialog($"���ƿ�! {playerUnit.Pokemon.Base.Name}");
            // TODO ���;ִϸ��̼� Faint �ӽû��
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2.0f);
        }
        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"����! {newPokemon.Base.Name}");

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
        yield return dialogBox.TypeDialog($"{trainer.Name}(��)�� {nextPokemon.Base.Name}�� ���´�!");

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

    IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if(isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"Ʈ���̳��� ���ϸ󿡰Դ� ����������!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} (��)�� ���ͺ��� ����ߴ�!");

        // ȸ���� ���°��� Quaternian.identify
        var pokeballObj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2,0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();

        // Animations
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2f), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon);

        for(int i=0;i< Mathf.Min(shakeCount,3); i++)
        {
            yield return new WaitForSeconds(0.5f); 
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            // Pokemon catch
            yield return dialogBox.TypeDialog($"�ų���! {enemyUnit.Pokemon.Base.Name}�� ��Ҵ�!");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name}�� ��Ƽ�� ���Դ�!");

            Destroy(pokeball);
            BattleOver(true);
        }
        else
        {
            // pokemon broke out
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} �� ������ ���Դ�!");
            else
                yield return dialogBox.TypeDialog("�Ʊ��� ���Ǵ� ��Ҵµ�!");
        
            Destroy(pokeball);
            state = BattleState.RunningTurn;
        }
    }

    int TryToCatchPokemon(Pokemon pokemon)
    {
        float a = (3 * pokemon.MaxHp - 2 * pokemon.HP) * pokemon.Base.CatchRate * ConditionsDB.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHp);

        if (a >= 255)
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;

        while(shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;

            ++shakeCount;
        }

        return shakeCount;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if(isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"Ʈ���̳� ��Ʋ������ �������� ����!");
            state = BattleState.RunningTurn;
            yield break;
        }

        ++escapeAttempts;

        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;

        if(enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"�����ϰ� �����ƴ�!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if(UnityEngine.Random.Range(0,256) < f)
            {
                yield return dialogBox.TypeDialog($"�����ϰ� �����ƴ�!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"����ĥ������!");
                state = BattleState.RunningTurn;
            }
        }
    }
}
