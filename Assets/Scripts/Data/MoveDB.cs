using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB 
{
    static Dictionary<string, MoveBase> moves;

    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();
        // 빈문자열 입력시 프로젝트내 모든 Resources 디렉토리내에서 MoveBase 타입의 모든 오브젝트를 찾아온다.
        var moveList = Resources.LoadAll<MoveBase>("");
        foreach (var move in moveList)
        {
            if (moves.ContainsKey(move.Name))
            {
                Debug.Log($"이미 등록된 기술입니다. : {move.Name}");
                continue;
            }

            moves[move.Name] = move;
        }
    }

    public static MoveBase GetMoveByName(string name)
    {
        if (!moves.ContainsKey(name))
        {
            Debug.Log($"기술 데이터베이스에 없는 기술입니다. : {name}");
            return null;
        }

        return moves[name];
    }
}
