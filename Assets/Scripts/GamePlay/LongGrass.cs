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
            //false�μ����ؾ� encounter�ɶ� �ִϸ��̼�ȭ����������.
            player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle();
        }
    }
    public bool TriggerRepeatedly => true;
}
