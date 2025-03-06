using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach(var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

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
                StartMessage = "화상을 입었습니다.",
                onAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}가 화상을입어 데미지를 입었다.");
                }
            }
        },
          {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "마비를 당했다.",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                   if(UnityEngine.Random.Range(1,5)==1)
                   {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.name}(이)가 마비되어 움직일수 없습니다!");
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
                StartMessage = "동상을 당했다.",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                   if(UnityEngine.Random.Range(1,5)==1)
                   {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.name}(이)가 동상상태에서 깨어났다!");
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
                StartMessage = "잠들었다.",
                OnStart = (Pokemon pokemon) =>
                {
                    // 1~3턴동안 잠든다.
                    pokemon.StatusTime = UnityEngine.Random.Range(1,4);
                    Debug.Log($"{pokemon.StatusTime} 동안 움직일수없다.");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(pokemon.StatusTime<=0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}이 잠에서 깨어났다!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}이 잠들어있다!");
                    return false;
                },
            }
          },

          // VolatileStatus
    {
            ConditionID.confusion,
            new Condition()
            {
                Name = "Confusion",
                StartMessage = "혼란에 빠졌다.",
                OnStart = (Pokemon pokemon) =>
                {
                    // 혼란상태 1~4턴동안 .
                    pokemon.VolatileStatusTime = UnityEngine.Random.Range(1,5);
                    Debug.Log($"{pokemon.VolatileStatusTime} 동안 움직일수없다.");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(pokemon.VolatileStatusTime<=0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}이 정신을 차렸다!");
                        return true;
                    }

                    pokemon.VolatileStatusTime--;
                    if(UnityEngine.Random.Range(1,3) == 1)
                        return true;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}이 혼란에 빠졌다!");
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"혼란으로인해 자신을 공격했다!");
                    return false;
                },
            }
          },


    }; 
}

public enum ConditionID
{
    none,psn,brn,slp,par,frz,
    confusion
}