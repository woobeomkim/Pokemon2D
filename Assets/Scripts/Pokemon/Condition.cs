using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition 
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }

    // Action은 void
    // Func는 값을반환할수있음

    public Action<Pokemon> OnStart { get; set; }
    public Func<Pokemon, bool> OnBeforeMove {  get; set; }
    public Action<Pokemon> onAfterTurn {  get; set; }
}
