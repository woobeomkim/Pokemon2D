using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create a new TM or HM")]
public class TmItem : ItemBase
{
    [SerializeField] MoveBase move;
    [SerializeField] bool isHM;

    public override string Name => base.Name + $": {move.Name}";

    public override bool Use(Pokemon pokemon)
    {
        // Learning move is handled from Inventroy UI, If it was learned then return true
        return pokemon.HasMove(move);
    }


    public bool CanBeTaught(Pokemon pokemon)
    {
       return pokemon.Base.LearnableByItems.Contains(Move);
    }

    public override bool IsResuable => isHM;
    public override bool CanBeUsedInBattle => false;

    public MoveBase Move => move;
    public bool IsHm => isHM;

}
