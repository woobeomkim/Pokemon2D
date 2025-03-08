
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �߻��ʵ��� ���ϸ�
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
