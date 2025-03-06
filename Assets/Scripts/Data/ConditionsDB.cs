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
                StartMessage = "독에 감염되었습니다.",
                onAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}가 독에감염되어 데미지를 입었다.");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "화상을 입었습니다..",
                onAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}가 화상을입어 데미지를 입었다.");
                }
            }
        }
    }; 
}

public enum ConditionID
{
    none,psn,brn,slp,par,frz
}