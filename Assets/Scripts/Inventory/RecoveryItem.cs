using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new Recovery Item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;

    [Header("Status Conditions")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Pokemon pokemon)
    {
        // Revive
        if (revive || maxRevive)
        {
            if (pokemon.HP > 0)
                return false;

            if (revive)
                pokemon.IncreaseHP(pokemon.MaxHp / 2);
            else if (maxRevive)
                pokemon.IncreaseHP(pokemon.MaxHp);

            pokemon.CureStatus();

            return true;
        }

        if (pokemon.HP == 0)
            return false;

        // Restore HP
        if (restoreMaxHP || hpAmount > 0)
        {
            if (pokemon.HP == pokemon.MaxHp)
                return false;

            if (restoreMaxHP)
                pokemon.IncreaseHP(pokemon.MaxHp);
            else
                pokemon.IncreaseHP(hpAmount);
        }

        if (recoverAllStatus || status != ConditionID.none)
        {
            if (pokemon.Status == null && pokemon.VolatileStatus == null)
                return false;

            if (recoverAllStatus)
            {
                pokemon.CureStatus();
                pokemon.CureVolatileStatus();
            }
            else
            {
                if (pokemon.Status.Id == status)
                    pokemon.CureStatus();
                else if (pokemon.VolatileStatus.Id == status)
                    pokemon.CureVolatileStatus();
                else
                    return false;
            }
        }

        if(restoreMaxPP)
        {
            pokemon.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
        }
        else if(ppAmount > 0)
        {
            pokemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
        }

            return true;
    }
}