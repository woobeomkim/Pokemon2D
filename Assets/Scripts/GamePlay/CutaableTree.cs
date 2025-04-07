using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutaableTree : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogManager.Instance.ShowDialogText("이 나무는 왠지 자를수 있어보인다..");

        var pokemonWihtCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "자르기"));

        if (pokemonWihtCut != null)
        {
            int selectedChoice= 0;
            yield return DialogManager.Instance.ShowDialogText($"{pokemonWihtCut.Base.Name}의 자르기를 사용하시겠습니까 ?",
                choices: new List<string> { "Yes", "No" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if(selectedChoice ==0)
            {
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWihtCut.Base.Name}이 자르기를 사용했다!");
                gameObject.SetActive(false);
            }
        
        }
    }
}
