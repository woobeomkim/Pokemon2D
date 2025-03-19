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


    //PartyScreen은 ActionSelection, RunningTurn, AboutToUse와 같은 다양한 상태에서 호출될 수 있습니다.
    public BattleState? CalledFrom { get; set; }
    public void Init()
    {
        /*
         GetComponentsInChildren<T>()은 현재 오브젝트와 모든 자식 오브젝트에서 특정 타입의 모든 컴포넌트를 가져옴.
         기본적으로 비활성화된 오브젝트는 포함되지 않음 (true를 넣으면 포함 가능).
         자기 자신도 포함되므로, 자기 자신을 제외하려면 Where()를 이용해서 필터링해야 함.
         특정 오브젝트에서 **한 개의 컴포넌트만 찾으려면 GetComponentInChildren<T>()**을 사용. 
         GetComponentsInChildren<T>()
         재귀적으로 모든 하위 자식까지 탐색
         자기 자신도 포함
         기본적으로 비활성화된 오브젝트는 탐색 안 함 (true 옵션 사용 가능)

         한 단계 아래 자식만 찾고 싶다면?
         transform.GetComponent<T>()를 사용하여 직접 탐색해야 함
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

        messageText.text = "포켓몬을 고르세요!";
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
