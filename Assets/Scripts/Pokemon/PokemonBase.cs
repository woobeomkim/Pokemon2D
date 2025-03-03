using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 C#�� ��Ʈ����Ʈ ������ ����� ����Ƽ�� Ư���ѱ�� 
ScriptableObject�� ����Ƽ �����Ϳ��� ���� �����Ҽ��ֵ��� �����ִ¿���

��Ʈ����Ʈ�� C#���� ��Ÿ������(�߰�����)�� Ŭ���� �޼��� �ʵ� � �ο��ϴ±��
��Ʈ����Ʈ�� ����ϸ� �����Ϸ��� ����Ƽ �����Ͱ� �ش� Ŭ������ �ʵ带 Ư���� ������� ó������
 [SerializeFiled] : ����Ƽ �ν����Ϳ��� ���� �����ϵ��� ����
[CreateAssetMenu] : ScripatableObject�� ����Ƽ �����Ϳ��� ���������ϰԸ�����
    fileName = ������ ������ �⺻�̸� (filename.asset)���� �����,
     menuName =  ����Ƽ �����Ϳ��� Create�޴� ��� ����

 CreateAssetMenu������ ����Ƽ �����Ϳ��� ������������ (����������)
 ���� ���·� ����Ǿ� ������ÿ��� ������������
 ���� �ڵ带 �ۼ��ϴ¹�ĺ��� ��������

[TextArea]�� ����Ƽ �ν����Ϳ��� ���ڿ� �Է��ʵ��� ũ�⸦ �����Ҽ� �ֵ��� ���ִ� ��Ʈ����Ʈ
string Ÿ���� ������ �ٷ궧 �������� �Է��Ҽ��ֵ��� ����ϴµ� ����
[TestArea]�� ����Ƽ �ν����Ϳ��� �������� �Է��Ҽ��ִ� �ؽ�Ʈ �ڽ��� ǥ�õ� (�⺻������ 3~10�ٹ����� ��������)
[TextAra(2,5)] : �ؽ�Ʈ �ڽ��� �ּ��� :2(�⺻�� : 3) , �ؽ�Ʈ �ڽ��� �ִ��� 5(�⺻�� : 10)
 */
[CreateAssetMenu(fileName = "Pokemon",menuName = "Pokemon/Create new Pokemon")]
/*
 ScripableObject : ������Ʈ �������� �����Ҽ��ִ� ������ �����̳� ������ �Ѵ�.
 MonoBehaviour�ʹ� �޸� ������Ʈ�� �����Ұ�, �������̹��� ���� �ε�ɶ����� ���λ���������
ScriptableObject�� ������Ʈ���� �������Ϸ� �����Ҽ��־� ���̺���Ǿ �����Ͱ�������
 ���������� , ������, ĳ���� ���õ��� ����뵵�� ���� 
    Awak,Start,Update���� ����Ƽ �����ֱ��Լ��� ȣ������ʰ� OnEnable,OnDisabel���� Ư���̺�Ʈ�Լ� ��밡��

ScriptableObject�� ����Ƽ�� ���� �ý����� ���� ����ȭ�Ǹ� ������ ScripatableObject �ν��Ͻ��� �������� ������Ʈ�� �����Ҽ��־�
�޸� ��� ����ȭ�� �Ҽ��ִ�. (�ߺ���Ƽ�� �ε� ��������)

���� : ���Ӽ���������, ĳ����,������ ���ͽ��� ������, ��ȭ��ũ��Ʈ����, �۷ι� ���� ����(GameState,AudiMangagetr)

 MonoBehaviour : �� �ȿ������� , ���� �ٲ�� ����� , ����ȭ���� (������ , ���������ʿ�) , �޸� ȿ���� (��ü���� ��������)
 ScriptableObject : ������Ʈ(����) �ȿ�������, ���� �ٲ� ������ , ����ȭ���� , ������ü�� ���� ������ ���� ����
 
OnEnable : ScripatableObject�� �ε�ɶ� �����
OnDisalbe : ScripatableObject�� ��ε�� �� �����
OnDestroy : ScripatableObject�� �����ɋ� �����
��Ÿ�ӵ��� �����͸� ���������� ������������¾���
 */
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