using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SurfableWater : MonoBehaviour, Interactable,IPlayerTriggerable
{
    bool isJumpingToWater = false;

    public bool TriggerRepeatedly => true;

    public IEnumerator Interact(Transform initiator)
    {
        var animator = initiator.GetComponent<CharacterAnimator>();
        if(animator.IsSurfing || isJumpingToWater)
            yield break;

        yield return DialogManager.Instance.ShowDialogText("물이 매우 깊어보인다..");

        var pokemonWihtSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "파도타기"));

        if (pokemonWihtSurf != null)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"{pokemonWihtSurf.Base.Name}의 파도타기를 사용하시겠습니까 ?",
                choices: new List<string> { "Yes", "No" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWihtSurf.Base.Name}이 파도타기를 사용했다!");

               
                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                isJumpingToWater = true;
                yield return initiator.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();
                isJumpingToWater = false;
                animator.IsSurfing = true;
            }

        }
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        // 실제확률은 5퍼정도
        if (UnityEngine.Random.Range(1, 101) <= 10)
        {
            GameController.Instance.StartBattle(BattleTrigger.Water);
        }
    }
}
