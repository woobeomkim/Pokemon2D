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

        messageText.text = "포켓몬을 고르세요!";
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
