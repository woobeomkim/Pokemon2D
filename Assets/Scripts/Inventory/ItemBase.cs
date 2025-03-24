using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    public virtual string Name => name;
    public string Description => description;
    public Sprite Icon => icon;

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }

    public virtual bool IsResuable => false;

    public virtual bool CanBeUsedInBattle => true;
    public virtual bool CanBeUsedOutsideBattle => true;
}
