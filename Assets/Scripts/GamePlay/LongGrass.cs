using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{

    public void OnPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(1, 101) <= 10)
        {
            //false로설정해야 encounter될때 애니메이션화가되지않음.
            player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle();
        }
    }
    public bool TriggerRepeatedly => true;
}
