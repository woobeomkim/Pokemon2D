using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{
    static Dictionary<string, ItemBase> items;

    public static void Init()
    {
        items = new Dictionary<string, ItemBase>();
        // ���ڿ� �Է½� ������Ʈ�� ��� Resources ���丮������ ItemBase Ÿ���� ��� ������Ʈ�� ã�ƿ´�.
        var itemList = Resources.LoadAll<ItemBase>("");
        foreach (var move in itemList)
        {
            if (items.ContainsKey(move.Name))
            {
                Debug.Log($"�̹� ��ϵ� �������Դϴ�. : {move.Name}");
                continue;
            }

            items[move.Name] = move;
        }
    }

    public static ItemBase GetItemByName(string name)
    {
        if (!items.ContainsKey(name))
        {
            Debug.Log($"������ �����ͺ��̽��� ���� ����Դϴ�. : {name}");
            return null;
        }

        return items[name];
    }
}
