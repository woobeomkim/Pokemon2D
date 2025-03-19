using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonDB
{
    static Dictionary<string, PokemonBase> pokemons;

    public static void Init()
    {
        pokemons = new Dictionary<string, PokemonBase>();
        // ���ڿ� �Է½� ������Ʈ�� ��� Resources ���丮������ PokemonBase Ÿ���� ��� ������Ʈ�� ã�ƿ´�.
        var pokemonArray = Resources.LoadAll<PokemonBase>("");
        foreach (var pokemon in pokemonArray)
        {
            if(pokemons.ContainsKey(pokemon.Name))
            {
                Debug.Log($"�̹� ��ϵ� ���ϸ��Դϴ�. : {pokemon.Name}");
                continue;
            }

            pokemons[pokemon.Name] = pokemon;
        }
    }

    public static PokemonBase GetPokemonByName(string name)
    {
        if (!pokemons.ContainsKey(name))
        {
            Debug.Log($"���ϸ� �����ͺ��̽��� ���� ���ϸ��Դϴ�. : {name}");
            return null;
        }
        
        return pokemons[name];
    }
}
