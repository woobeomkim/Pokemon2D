using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        { 
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "���� �����Ǿ����ϴ�.",
                onAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}�� ���������Ǿ� �������� �Ծ���.");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "ȭ���� �Ծ����ϴ�.",
                onAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}�� ȭ�����Ծ� �������� �Ծ���.");
                }
            }
        },
          {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "���� ���ߴ�.",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                   if(UnityEngine.Random.Range(1,5)==1)
                   {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.name}(��)�� ����Ǿ� �����ϼ� �����ϴ�!");
                        return false;
                   }
                   return true;
                }
            }
        },
          {
            ConditionID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "������ ���ߴ�.",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                   if(UnityEngine.Random.Range(1,5)==1)
                   {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.name}(��)�� ������¿��� �����!");
                        return true;
                   }
                   return false;
                }
            }
          },
          {
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "������.",
                OnStart = (Pokemon pokemon) =>
                {
                    // 1~3�ϵ��� ����.
                    pokemon.StatusTime = UnityEngine.Random.Range(1,4);
                    Debug.Log($"{pokemon.StatusTime} ���� �����ϼ�����.");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(pokemon.StatusTime<=0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}�� �ῡ�� �����!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}�� �����ִ�!");
                    return false;
                },
            }
          },
    }; 
}

public enum ConditionID
{
    none,psn,brn,slp,par,frz
}