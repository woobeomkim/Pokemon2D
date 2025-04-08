
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

// 야생필드의 포켓몬
public class MapArea : MonoBehaviour
{
    [SerializeField] List<PokemonEncounterRecord> wildPokemons;
    [SerializeField] List<PokemonEncounterRecord> wildPokemonsInWater;

    [HideInInspector]
    [SerializeField] int totalChance;
  
    [HideInInspector]
    [SerializeField] int totalChanceInWater;
    private void OnValidate()
    {
        CalculateChacePercentage();
    }

    private void Start()
    {
        CalculateChacePercentage();
    }

    void CalculateChacePercentage()
    {
        totalChance = -1;
        totalChanceInWater = -1;

        if (wildPokemons.Count > 0)
        {
            totalChance = 0;
            foreach (var record in wildPokemons)
            {
                record.chanceLower = totalChance;
                record.chanceUpper = totalChance + record.chancePercentage;

                totalChance += record.chancePercentage;
            }
        }

        if (wildPokemonsInWater.Count > 0)
        {
            totalChanceInWater = 0;
            foreach (var record in wildPokemonsInWater)
            {
                record.chanceLower = totalChanceInWater;
                record.chanceUpper = totalChanceInWater + record.chancePercentage;

                totalChanceInWater += record.chancePercentage;
            }
        }
    }

    public Pokemon GetRandomWildPokemon(BattleTrigger trigger)
    {
        var pokemonList = (trigger == BattleTrigger.LongGrass) ? wildPokemons : wildPokemonsInWater;

        int randVal =  UnityEngine.Random.Range(0, 101);
        var pokemonRecord = pokemonList.First(p => randVal >= p.chanceLower && randVal <= p.chanceUpper);

        var levelRange = pokemonRecord.levelRange;
        int level = levelRange.y == 0 ? levelRange.x : UnityEngine.Random.Range(levelRange.x, levelRange.y + 1);
        
        var wildPokemon = new Pokemon(pokemonRecord.pokemon, level);
        wildPokemon.Init();

        return wildPokemon;

    }
}

[System.Serializable]
public class PokemonEncounterRecord
{
    public PokemonBase pokemon;
    public Vector2Int levelRange;
    public int chancePercentage;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }
}