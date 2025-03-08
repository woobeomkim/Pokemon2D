using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 C#의 어트리뷰트 문법을 사용한 유니티의 특수한기능 
ScriptableObject를 유니티 에디터에서 쉽게 생성할수있도록 도와주는역할

어트리뷰트는 C#에서 메타데이터(추가정보)를 클래스 메서드 필드 등에 부여하는기능
어트리뷰트를 사용하면 컴파일러나 유니티 에디터가 해당 클래스나 필드를 특별한 방식츠로 처리가능
 [SerializeFiled] : 유니티 인스펙터에서 편집 가능하도록 설정
[CreateAssetMenu] : ScripatableObject를 유니티 에디터에서 생성가능하게만들음
    fileName = 생성될 파일의 기본이름 (filename.asset)으로 저장됨,
     menuName =  유니티 에디터에서 Create메뉴 경로 설정

 CreateAssetMenu의장점 유니티 에디터에서 직접생성가능 (관리가쉬움)
 파일 형태로 저장되어 씬변경시에도 데이터유지됨
 직접 코드를 작성하는방식보다 직관적임

[TextArea]는 유니티 인스펙터에서 문자열 입력필드의 크기를 조절할수 있도록 해주는 애트리뷰트
string 타입의 변수를 다룰때 여러줄을 입력할수있도록 허용하는데 사용됨
[TestArea]는 유니티 인스펙터에서 여러줄을 입력할수있는 텍스트 박스로 표시됨 (기본적으로 3~10줄범위로 조절가능)
[TextAra(2,5)] : 텍스트 박스의 최소줄 :2(기본값 : 3) , 텍스트 박스의 최대줄 5(기본값 : 10)
 */
[CreateAssetMenu(fileName = "Pokemon",menuName = "Pokemon/Create new Pokemon")]
/*
 ScripableObject : 프로젝트 에셋으로 존재할수있는 데이터 컨테이너 역할을 한다.
 MonoBehaviour와는 달리 오브젝트에 부착불가, 모노비헤이버는 씬이 로드될때마다 새로생성되지만
ScriptableObject는 프로젝트내의 에셋파일로 저장할수있어 씬이변경되어도 데이터가유지됨
 전역데이터 , 설정값, 캐릭터 스택등의 저장용도로 적합 
    Awak,Start,Update같은 유니티 생명주기함수가 호출되지않고 OnEnable,OnDisabel같은 특정이벤트함수 사용가능

ScriptableObject는 유니티의 에셋 시스템을 통해 직렬화되며 동일한 ScripatableObject 인스턴스를 여러개의 오브젝트에 공유할수있어
메모리 사용 최적화를 할수있다. (중복데티어 로드 방지가능)

예시 : 게임설정데이터, 캐릭터,아이템 몬스터스텟 데이터, 대화스크립트관리, 글로벌 상태 저장(GameState,AudiMangagetr)

 MonoBehaviour : 씬 안에서관리 , 씬이 바뀌면 사라짐 , 직렬화지원 (제한적 , 파일저장필요) , 메모리 효율성 (객체마다 개별저장)
 ScriptableObject : 프로젝트(에셋) 안에서관리, 씬이 바뀌어도 유지됨 , 직렬화지원 , 여러객체가 같은 데이터 공유 가능
 
OnEnable : ScripatableObject가 로드될때 실행됨
OnDisalbe : ScripatableObject가 언로드될 때 실행됨
OnDestroy : ScripatableObject가 삭제될떄 실행됨
런타임동안 데이터를 공유하지만 영구저장되지는않음
 */
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

//강화할수있는값들을 따로 Stat enum으로 빼줬다.
public enum Stat
{
    Attack,
    Defnecse,
    SpAttack,
    SpDefense,
    Speed,

    // (이 두 개는 실제 스탯이 아니라, 기술 명중률을 증가시키는 데 사용됨)
    Accuracy,
    Evasion,
}

class TypeChart
{
    // 타입에대한 속성 2차원배열로 미리만들어두기
    static float[][] chart =
    {
        //                       Nor   Fir   Wat   Ele   Gra   Ice   Fig   Poi   Gro   Fly   Psy   Bug   Roc   Gho   Dra   Dar  Ste    Fai
        /*Normal*/  new float[] {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 0,    1f,   1f,   0.5f, 1f},
        /*Fire*/    new float[] {1f,   0.5f, 0.5f, 1f,   2f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   0.5f, 1f,   2f,   1f},
        /*Water*/   new float[] {1f,   2f,   0.5f, 1f,   0.5f, 1f,   1f,   1f,   2f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f,   1f,   1f},
        /*Electric*/new float[] {1f,   1f,   2f,   0.5f, 0.5f, 1f,   1f,   1f,   0f,   2f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   1f},
        /*Grass*/   new float[] {1f,   0.5f, 2f,   1f,   0.5f, 1f,   1f,   0.5f, 2f,   0.5f, 1f,   0.5f, 2f,   1f,   0.5f, 1f,   0.5f, 1f},
        /*Ice*/     new float[] {1f,   0.5f, 0.5f, 1f,   2f,   0.5f, 1f,   1f,   2f,   2f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f},
        /*Fighting*/new float[] {2f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f,   0.5f, 0.5f, 0.5f, 2f,   0f,   1f,   2f,   2f,   0.5f},
        /*Poison*/  new float[] {1f,   1f,   1f,   1f,   2f,   1f,   1f,   0.5f, 0.5f, 1f,   1f,   1f,   0.5f, 0.5f, 1f,   1f,   0f,   2f},
        /*Ground*/  new float[] {1f,   2f,   1f,   2f,   0.5f, 1f,   1f,   2f,   1f,   0f,   1f,   0.5f, 2f,   1f,   1f,   1f,   2f,   1f},
        /*Flying*/  new float[] {1f,   1f,   1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   0.5f, 1f},
        /*Psychic*/ new float[] {1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f,   0.5f, 1f,   1f,   1f,   1f,   0f,   0.5f, 1f},
        /*Bug*/     new float[] {1f,   0.5f, 1f,   1f,   2f,   1f,   0.5f, 0.5f, 1f,   0.5f, 2f,   1f,   1f,   0.5f, 1f,   2f,   0.5f, 0.5f},
        /*Rock*/    new float[] {1f,   2f,   1f,   1f,   1f,   2f,   0.5f, 1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f,   1f,   0.5f, 1f},
        /*Ghost*/   new float[] {0f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   2f,   1f,   0.5f, 1f,   1f},
        /*Dragon*/  new float[] {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 0f},
        /*Dark*/    new float[] {1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f,   0.5f, 1f,   0.5f},
        /*Steel*/   new float[] {1f,   0.5f, 0.5f, 0.5f, 1f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   0.5f, 2f},
        /*Fairy*/   new float[] {1f,   0.5f, 1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   0.5f, 1f},
    };

    // 배열값 얻어온는함수
    public static float GetEffectiveness(PokemonType attackerType, PokemonType defenderType)
    {
        if (attackerType == PokemonType.None || defenderType == PokemonType.None)
            return 1f;

        int row = (int )attackerType - 1;
        int col = (int )defenderType - 1;

        return chart[row][col]; ;
    }
}