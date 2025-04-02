using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{
    static Dictionary<string, ItemBase> items;

    public static void Init()
    {
        items = new Dictionary<string, ItemBase>();
        // 빈문자열 입력시 프로젝트내 모든 Resources 디렉토리내에서 ItemBase 타입의 모든 오브젝트를 찾아온다.
        var itemList = Resources.LoadAll<ItemBase>("");
        foreach (var move in itemList)
        {
            if (items.ContainsKey(move.Name))
            {
                Debug.Log($"이미 등록된 아이템입니다. : {move.Name}");
                continue;
            }

            items[move.Name] = move;
        }
    }

    public static ItemBase GetItemByName(string name)
    {
        if (!items.ContainsKey(name))
        {
            Debug.Log($"아이템 데이터베이스에 없는 기술입니다. : {name}");
            return null;
        }

        return items[name];
    }
}
