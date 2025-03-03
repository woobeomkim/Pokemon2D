using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon",menuName = "Pokemon/Create new Pokemon")]

public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    //���ϸ��� �ִ� 2��Ÿ�Ա��� �������ִ� (EX : ��/�巡�� ���ڸ�)
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    // Base Stat
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    // ���� : ���ϸ��� ���� �����ִ� ����� �������ִ� (BASEŬ������ �ִ°Ը´�).
    [SerializeField] List<LearanableMove> learanableMoves;

    /*
     Property�� �ʵ尪�� �����ϱ����� C#�� Ư���Ѹ��
     �ʵ��� �����͸� ��ȣ�ϸ鼭�� �ܺο��� �����Ҽ� �ֵ��� �����ִ� ������Ѵ�.

    public���� �����ϸ� �ܺο� �����Ǿ� ���ȿ� ����� ������ Getter�� Setter�� ����������
    �Ź� �����ؾ��ؼ� �ڵ尡�����
    get { return ~ } : ���� ������ get�� �����
    set { v = data} : ���� �����Ҷ� set�� �����
    �Ϲ� ����ó�� ��밡�������� ���������� Getter�� Setter�� �����

    ����ĵ� �ݺ��Ǳ⶧���� �ڵ�������Ƽ���ִ�.
    public int HP {get;set;} : get set�� �ڵ����� ����� -> �ڵ尡�������� 
    get private setó�� �ٸ� ����������

     */

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public PokemonType Type1
    {
        get { return type1; }
    }

    public PokemonType Type2
    {
        get { return type2; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearanableMove> LearanableMoves
        { get { return learanableMoves; } }
}


// Ŭ������ ����ȭ�Ҷ��� System.Serializable
// ����ȭ�� ��ü �����͸� �����ϰų� ��Ʈ��ũ�� ���� ������ �� �ֵ��� ��ȯ�ϴ� �����Դϴ�.
// ����Ƽ������ Inspector���� �����͸� �����ϰų�, JSON/���� ������ �� �� ����ȭ�� �ʿ��մϴ�.
[System.Serializable]
public class LearanableMove
{
    // �����ִ� ��� Ŭ�������� 
    // ����� ��� �����ִ� �Ӽ����ִ�.
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
        { get { return moveBase; } }
    public int Level
        { get { return level; } }
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Pshchic,
    Bug,
    Rock,
    Ghost,
    Dragon,
}