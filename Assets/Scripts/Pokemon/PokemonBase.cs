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

    //포켓몬은 최대 2개타입까지 가질수있다 (EX : 불/드래곤 리자몽)
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    // Base Stat
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    // 설계 : 포켓몬은 각자 배울수있는 기술이 정해져있다 (BASE클래스에 있는게맞다).
    [SerializeField] List<LearanableMove> learanableMoves;

    /*
     Property는 필드값을 제어하기위한 C#의 특별한멤버
     필드의 데이터를 보호하면서도 외부에서 접근할수 있도록 도와주는 기능을한다.

    public으로 선언하면 외부에 공개되어 보안에 취약해 보통은 Getter와 Setter를 선언하지만
    매번 선언해야해서 코드가길어짐
    get { return ~ } : 값을 읽을때 get이 실행됨
    set { v = data} : 값을 변경할때 set이 실행됨
    일반 변수처럼 사용가능하지만 내부적으로 Getter와 Setter가 실행됨

    위방식도 반복되기때문에 자동프로퍼티가있다.
    public int HP {get;set;} : get set이 자동으로 실행됨 -> 코드가간결해짐 
    get private set처럼 다른 범위도가능

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


// 클래스를 직렬화할때는 System.Serializable
// 직렬화는 객체 데이터를 저장하거나 네트워크를 통해 전송할 수 있도록 변환하는 과정입니다.
// 유니티에서는 Inspector에서 데이터를 편집하거나, JSON/파일 저장을 할 때 직렬화가 필요합니다.
[System.Serializable]
public class LearanableMove
{
    // 배울수있는 기술 클래스에는 
    // 기술과 몇렙에 배울수있는 속성이있다.
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