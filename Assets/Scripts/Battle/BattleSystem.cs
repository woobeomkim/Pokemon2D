using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ��Ʋ ������¸� enum ���� ����
public enum BattleState { Start, PlayerAction, PlayerMove,EnemyMove,Busy}

// ��Ʈ�ý��� ��ü�� ����
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;

    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> onBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

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
        // ��Ʋ��������
        playerUnit.Setup(playerParty.GetHealthPokemon());
        playerHud.SetData(playerUnit.Pokemon);

        enemyUnit.Setup(wildPokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        partyScreen.Init();
        //����̸� ����
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        // yield return �ڷ�ƾ�Լ����ϸ� �� �ڷ�ƾ �Լ��� ���������� ���Լ��� ��ٸ�
        yield return dialogBox.TypeDialog($"�߻��� {enemyUnit.Pokemon.Base.Name} (��)�� ��Ÿ����!");

        //�÷��̾� �ൿ����
        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        dialogBox.SetDialog("�ൿ�� ������!");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }
    void PlayerMove()
    {
        //UI������Ʈ
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    // �÷��̾ �����ϰ� ���� �ݰ��Ѵ�.
    IEnumerator PerformPlayerMove()
    {
        // ���¸� Busy�� �ٲ� currentMove�� �ٲ����ʵ��� �Ѵ�.
        state = BattleState.Busy;

        var move = playerUnit.Pokemon.Moves[currentMove];
        move.PP--;
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} (��)�� {move.Base.Name}�� ����ߴ�!");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1.0f);

        enemyUnit.PlayHitAnimation();
        // �������� �Դ´� 
        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} (��)�� �����ߴ�.");
            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            onBattleOver(true);
        }
        else
        {
            // ���� �ݰ�
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;
        // �����ϸ� ���� ����

        var move = enemyUnit.Pokemon.GetRandomMove();
        move.PP--;
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} (��)�� {move.Base.Name}�� ����ߴ�!");
       
        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1.0f);
        playerUnit.PlayHitAnimation();
        // �������� �Դ´� 
        var damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} (��)�� �����ߴ�.");
            playerUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);

            var nextPokemon = playerParty.GetHealthPokemon();
            if (nextPokemon != null)
            {
                playerUnit.Setup(nextPokemon);
                playerHud.SetData(nextPokemon);

                //����̸� ����
                dialogBox.SetMoveNames(nextPokemon.Moves);
                // yield return �ڷ�ƾ�Լ����ϸ� �� �ڷ�ƾ �Լ��� ���������� ���Լ��� ��ٸ�
                yield return dialogBox.TypeDialog($"����! {nextPokemon.Base.Name}!");

                //�÷��̾� �ൿ����
                PlayerAction();
            }
            else
            {
                onBattleOver(false);
            }            
        }
        else
        {
            // �ٽ� �÷��̾� �׼�
            PlayerAction();
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
                PlayerMove();
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
        if(Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());

        }
        //X�� ������ �ٽ� Move����â����
        else if(Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            PlayerAction();
        }
    }
}
