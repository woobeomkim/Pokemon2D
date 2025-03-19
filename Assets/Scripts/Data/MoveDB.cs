using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB 
{
    static Dictionary<string, MoveBase> moves;

    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();
        // ���ڿ� �Է½� ������Ʈ�� ��� Resources ���丮������ MoveBase Ÿ���� ��� ������Ʈ�� ã�ƿ´�.
        var moveList = Resources.LoadAll<MoveBase>("");
        foreach (var move in moveList)
        {
            if (moves.ContainsKey(move.Name))
            {
                Debug.Log($"�̹� ��ϵ� ����Դϴ�. : {move.Name}");
                continue;
            }

            moves[move.Name] = move;
        }
    }

    public static MoveBase GetMoveByName(string name)
    {
        if (!moves.ContainsKey(name))
        {
            Debug.Log($"��� �����ͺ��̽��� ���� ����Դϴ�. : {name}");
            return null;
        }

        return moves[name];
    }
}
