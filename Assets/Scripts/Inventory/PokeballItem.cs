using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create a new Pokeball")]
public class PokeballItem : ItemBase
{
    [SerializeField] float catchRateModfier = 1;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }

    public override bool CanBeUsedOutsideBattle => false;

    public float CatchRateModifier => catchRateModfier;
}
