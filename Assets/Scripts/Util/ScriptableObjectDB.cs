using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDB<T> : MonoBehaviour where T : ScriptableObject
{
    static Dictionary<string, T> objects;

    public static void Init()
    {
        objects = new Dictionary<string, T>();
        // 빈문자열 입력시 프로젝트내 모든 Resources 디렉토리내에서 PokemonBase 타입의 모든 오브젝트를 찾아온다.
        var objectArray = Resources.LoadAll<T>("");
        foreach (var obj in objectArray)
        {
            if (objects.ContainsKey(obj.name))
            {
                Debug.Log($"이미 등록된 포켓몬입니다. : {obj.name}");
                continue;
            }

            objects[obj.name] = obj;
        }
    }

    public static T GetObjectByName(string name)
    {
        if (!objects.ContainsKey(name))
        {
            Debug.Log($"오브젝트 데이터베이스에 없는 데이터입니다. : {name}");
            return null;
        }

        return objects[name];
    }
}
