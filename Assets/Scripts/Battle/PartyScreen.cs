using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField]Text messageText;

    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;

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
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if(i<pokemons.Count)
                memberSlots[i].SetData(pokemons[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "���ϸ��� ������!";
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
