using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    // SerializeFiled를 하려면 Pokemon내부클래스에서도 변수가 SerializeFiled여야함
    [SerializeField] List<Pokemon> pokemons;

    public event Action onUpdated;
    public List<Pokemon> Pokemons
    {
        get { return pokemons; }
        set 
        {
            pokemons = value; 
            onUpdated?.Invoke(); 
        }
    }

    private void Awake()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    private void Start()
    {
      
    }

    public Pokemon GetHealthPokemon()
    {
        /*LINQ 기능 LINQ (Language Integrated Query)
          LINQ는 **C#**에서 데이터를 쉽게 필터링, 정렬, 검색할 수 있도록 도와주는 언어 내장 기능입니다.
         LINQ는 언어 통합 쿼리로, 데이터를 다룰 때 SQL처럼 쿼리문을 작성할 수 있게 해줍니다. 예를 들어,
        **컬렉션(List, 배열 등)**에 있는 데이터에서 특정 조건을 만족하는 데이터를 검색하거나 필터링하는 데 사용됩니다.
        
         Where (필터링)
          Where는 조건을 만족하는 데이터만 필터링해서 반환하는 메서드입니다. 쉽게 말해, 리스트에서 조건에 맞는 아이템들만 걸러내는 것입니다.
         
        FirstOrDefault는 조건에 맞는 첫 번째 아이템을 반환하는 메서드입니다. 만약 조건에 맞는 아이템이 없다면 **null**을 반환합니다.
         */
        return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }

    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemons.Count < 6)
        {
            pokemons.Add(newPokemon);
            onUpdated?.Invoke();
        }
        else
        {
            // TODO : PC에집어넣기 
        }
    }

    public IEnumerator CheckForEvolutions()
    {
        foreach(var pokemon in pokemons)
        {
            var evolution = pokemon.CheckForEvolution();
            if(evolution!=null)
            {
              yield return EvolutionManager.i.Evolve(pokemon, evolution);
            }
        }
        onUpdated?.Invoke();
    }

    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }
}
