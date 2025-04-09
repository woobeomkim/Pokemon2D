using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectAction : CutsceneAction
{
    [SerializeField] GameObject go;

    public override IEnumerator Play()
    {
        go.gameObject.SetActive(false);
        yield break;
    }
}
