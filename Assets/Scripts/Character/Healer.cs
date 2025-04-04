using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialogText("너 좀피곤해보이는데 쉬었다갈래?",
          choices :  new List<string>() { "Yes", "No" }, 
          onChoiceSelected : (choiceIndex) => selectedChoice = choiceIndex );

        if(selectedChoice == 0)
        {
            yield return Fader.i.FadeIn(0.5f);

            var playerParty = player.GetComponent<PokemonParty>();
            playerParty.Pokemons.ForEach(p => p.Heal());
            playerParty.PartyUpdated();

            yield return Fader.i.FadeOut(0.5f);
            yield return DialogManager.Instance.ShowDialogText("치료가 다됐어");
        }
        else if (selectedChoice == 1)
        {
            yield return DialogManager.Instance.ShowDialogText("그럼 치료하고싶을때 와!");
        }

     
    }
}
