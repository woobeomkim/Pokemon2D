using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutaableTree : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogManager.Instance.ShowDialogText("�� ������ ���� �ڸ��� �־�δ�..");

        var pokemonWihtCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "�ڸ���"));

        if (pokemonWihtCut != null)
        {
            int selectedChoice= 0;
            yield return DialogManager.Instance.ShowDialogText($"{pokemonWihtCut.Base.Name}�� �ڸ��⸦ ����Ͻðڽ��ϱ� ?",
                choices: new List<string> { "Yes", "No" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if(selectedChoice ==0)
            {
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWihtCut.Base.Name}�� �ڸ��⸦ ����ߴ�!");
                gameObject.SetActive(false);
            }
        
        }
    }
}
