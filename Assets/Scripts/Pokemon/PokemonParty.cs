using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    // SerializeFiled�� �Ϸ��� Pokemon����Ŭ���������� ������ SerializeFiled������
    [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons
    {
        get { return pokemons; }
    }

    private void Start()
    {
        foreach(var pokemon in pokemons)
        {
            pokemon.Init();
        }
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
        }
        else
        {
            // TODO : PC������ֱ� 
        }
    }
}
