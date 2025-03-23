using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create a new TM or HM")]
public class TmItem : ItemBase
{
    [SerializeField] MoveBase move;

    public override bool Use(Pokemon pokemon)
    {
        // Learning move is handled from Inventroy UI, If it was learned then return true
        return pokemon.HasMove(move);
    }

    public MoveBase Move => move;


}
