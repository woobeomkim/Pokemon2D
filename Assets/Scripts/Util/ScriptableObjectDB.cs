using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDB<T> : MonoBehaviour where T : ScriptableObject
{
    static Dictionary<string, T> objects;

    public static void Init()
    {
        objects = new Dictionary<string, T>();
        // ���ڿ� �Է½� ������Ʈ�� ��� Resources ���丮������ PokemonBase Ÿ���� ��� ������Ʈ�� ã�ƿ´�.
        var objectArray = Resources.LoadAll<T>("");
        foreach (var obj in objectArray)
        {
            if (objects.ContainsKey(obj.name))
            {
                Debug.Log($"�̹� ��ϵ� ���ϸ��Դϴ�. : {obj.name}");
                continue;
            }

            objects[obj.name] = obj;
        }
    }

    public static T GetObjectByName(string name)
    {
        if (!objects.ContainsKey(name))
        {
            Debug.Log($"������Ʈ �����ͺ��̽��� ���� �������Դϴ�. : {name}");
            return null;
        }

        return objects[name];
    }
}
