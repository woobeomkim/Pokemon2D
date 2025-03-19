using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonDB
{
    static Dictionary<string, PokemonBase> pokemons;

    public static void Init()
    {
        pokemons = new Dictionary<string, PokemonBase>();
        // 빈문자열 입력시 프로젝트내 모든 Resources 디렉토리내에서 PokemonBase 타입의 모든 오브젝트를 찾아온다.
        var pokemonArray = Resources.LoadAll<PokemonBase>("");
        foreach (var pokemon in pokemonArray)
        {
            if(pokemons.ContainsKey(pokemon.Name))
            {
                Debug.Log($"이미 등록된 포켓몬입니다. : {pokemon.Name}");
                continue;
            }

            pokemons[pokemon.Name] = pokemon;
        }
    }

    public static PokemonBase GetPokemonByName(string name)
    {
        if (!pokemons.ContainsKey(name))
        {
            Debug.Log($"포켓몬 데이터베이스에 없는 포켓몬입니다. : {name}");
            return null;
        }
        
        return pokemons[name];
    }
}
