
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 야생필드의 포켓몬
public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemon> wildPokemons;

    public Pokemon GetRandomWildPokemon()
    {
        var wildPokemon =  wildPokemons[Random.Range(0, wildPokemons.Count)];
        wildPokemon.Init();

        return wildPokemon;
    }
}
