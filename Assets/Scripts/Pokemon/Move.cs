using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    MoveBase Base { get; set; }
    int PP { get; set; }

    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP =pBase.PP;
    }
}
