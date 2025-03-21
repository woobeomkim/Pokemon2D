using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create a new Pokeball")]
public class PokeballItem : ItemBase
{
    [SerializeField] float catchRateModfier = 1;

    public override bool Use(Pokemon pokemon)
    {
        if(GameController.Instance.State == GameState.Battle)
        {
            return true;
        }
        return false;
    }

    public float CatchRateModifier => catchRateModfier;
}
