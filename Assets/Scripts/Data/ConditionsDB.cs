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
                StartMessage = "ȭ���� �Ծ����ϴ�..",
                onAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}�� ȭ�����Ծ� �������� �Ծ���.");
                }
            }
        }
    }; 
}

public enum ConditionID
{
    none,psn,brn,slp,par,frz
}