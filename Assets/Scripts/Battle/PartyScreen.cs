using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField]Text messageText;

    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;

    int selection = 0;

    public Pokemon SelectedMember => pokemons[selection];


    //PartyScreen�� ActionSelection, RunningTurn, AboutToUse�� ���� �پ��� ���¿��� ȣ��� �� �ֽ��ϴ�.
    public BattleState? CalledFrom { get; set; }
    public void Init()
    {
        /*
         GetComponentsInChildren<T>()�� ���� ������Ʈ�� ��� �ڽ� ������Ʈ���� Ư�� Ÿ���� ��� ������Ʈ�� ������.
         �⺻������ ��Ȱ��ȭ�� ������Ʈ�� ���Ե��� ���� (true�� ������ ���� ����).
         �ڱ� �ڽŵ� ���ԵǹǷ�, �ڱ� �ڽ��� �����Ϸ��� Where()�� �̿��ؼ� ���͸��ؾ� ��.
         Ư�� ������Ʈ���� **�� ���� ������Ʈ�� ã������ GetComponentInChildren<T>()**�� ���. 
         GetComponentsInChildren<T>()
         ��������� ��� ���� �ڽı��� Ž��
         �ڱ� �ڽŵ� ����
         �⺻������ ��Ȱ��ȭ�� ������Ʈ�� Ž�� �� �� (true �ɼ� ��� ����)

         �� �ܰ� �Ʒ� �ڽĸ� ã�� �ʹٸ�?
         transform.GetComponent<T>()�� ����Ͽ� ���� Ž���ؾ� ��
         */
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemons[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        UpdateMemeberSelection(selection);

        messageText.text = "���ϸ��� ������!";
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var previousSelection = selection;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selection;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selection;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            selection += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selection -= 2;

        selection = Mathf.Clamp(selection, 0, pokemons.Count - 1);

        if(selection != previousSelection)
            UpdateMemeberSelection(selection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
           onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    public void UpdateMemeberSelection(int selectedMember)
    {
        for(int i=0;i<pokemons.Count;i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
