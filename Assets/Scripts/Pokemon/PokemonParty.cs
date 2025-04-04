using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    // SerializeFiled�� �Ϸ��� Pokemon����Ŭ���������� ������ SerializeFiled������
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
        /*LINQ ��� LINQ (Language Integrated Query)
          LINQ�� **C#**���� �����͸� ���� ���͸�, ����, �˻��� �� �ֵ��� �����ִ� ��� ���� ����Դϴ�.
         LINQ�� ��� ���� ������, �����͸� �ٷ� �� SQLó�� �������� �ۼ��� �� �ְ� ���ݴϴ�. ���� ���,
        **�÷���(List, �迭 ��)**�� �ִ� �����Ϳ��� Ư�� ������ �����ϴ� �����͸� �˻��ϰų� ���͸��ϴ� �� ���˴ϴ�.
        
         Where (���͸�)
          Where�� ������ �����ϴ� �����͸� ���͸��ؼ� ��ȯ�ϴ� �޼����Դϴ�. ���� ����, ����Ʈ���� ���ǿ� �´� �����۵鸸 �ɷ����� ���Դϴ�.
         
        FirstOrDefault�� ���ǿ� �´� ù ��° �������� ��ȯ�ϴ� �޼����Դϴ�. ���� ���ǿ� �´� �������� ���ٸ� **null**�� ��ȯ�մϴ�.
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
            // TODO : PC������ֱ� 
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
