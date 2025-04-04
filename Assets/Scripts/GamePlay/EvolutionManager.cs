using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image pokemonImage;

    public event Action OnStartEvolution;
    public event Action OnCompleteEvolution;

    public static EvolutionManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public IEnumerator Evolve (Pokemon pokemon, Evolution evolution)
    {
        OnStartEvolution?.Invoke();
        evolutionUI.SetActive(true);

        pokemonImage.sprite = pokemon.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name}가 진화중이다!");

        var oldPokemon = pokemon.Base;
        pokemon.Evolve(evolution);  
        pokemonImage.sprite = pokemon.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"{oldPokemon.Name}가 {pokemon.Base.Name}로 진화했습니다!");

        evolutionUI.SetActive(false);
        OnCompleteEvolution?.Invoke();
    }
}
