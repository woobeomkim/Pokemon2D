using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 클래스를 직렬화할때
[System.Serializable]
public class Pokemon
{
    // 베이스클래스
    [SerializeField] PokemonBase _base;
    //현재나의레벨
    [SerializeField] int level;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;

        Init();
    }

    public PokemonBase Base { get { return _base; } }
    public int Level { get{ return level; } }


    public int Exp { get; set; }
    public int HP { get; set; }

    // 내가가지고잇는 Moves
    public List<Move> Moves { get; set; }

    // 현재무브
    public Move CurrentMove { get; set; }
    // 미리 저장해놓을 딕셔너리
    public Dictionary<Stat,int> Stats { get; private set; }

    // 얼마나 부스트되었는지 저장하는 딕셔너리
    // 포켓몬에서 부스트는 -6 ~ +6까지 부스팅된다.
    public Dictionary<Stat,int> StatBoosts { get; private set; }

    public Condition Status { get; set; }

    public int StatusTime { get; set; }
    public Condition VolatileStatus { get; set; }
    public int VolatileStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public bool HpChanged { get; set; }
    public event System.Action OnStatusChanged;
    public void Init()
    {
        // Generate Mvoes
        Moves = new List<Move>();
        foreach (var move in Base.LearanableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));

            if (Moves.Count >= 4)
                break;
        }
        
        Exp = Base.GetExpForLevel(Level);

        CalculateStats();
        
        HP = MaxHp;

        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    // Stat을 미리 계산하는함수.
    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defnecse, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);
    
        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;

        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack,6 },
            {Stat.Defnecse,6 },
            {Stat.SpAttack,6 },
            {Stat.SpDefense,6 },
            {Stat.Speed,6 },
        
        };
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0},
            { Stat.Defnecse, 0},
            { Stat.SpAttack, 0},
            { Stat.SpDefense, 0},
            { Stat.Speed, 0},

            {Stat.Accuracy,0 },
            {Stat.Evasion,0 },
        };
    }

    // 스탯을 얻어오는함수
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];

        // 부스트상수
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        // 포켓몬게임 부스트공식
        if (boost >= 0)
        {
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        }
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            string kor = "";
            switch(stat)
            {
                case Stat.Attack:
                    kor = "공격력";
                    break;
                case Stat.Defnecse:
                    kor = "방어력";
                    break;
                case Stat.SpAttack:
                    kor = "특수공격력";

                    break;
                case Stat.SpDefense:
                    kor = "특수방어력";
                    break;
                case Stat.Speed:
                    kor = "스피드";
                    break;
            }

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}의 {kor}(이)가 증가했습니다!");
            else
                StatusChanges.Enqueue($"{Base.Name}의 {kor}(이)가 감소했습니다!");


            Debug.Log($"{stat} has been bossted to {StatBoosts[stat]} ");
        }
    }

    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }

        return false;
    }

    // 포켓몬 스탯공식
    // https://bulbapedia.bulbagarden.net/wiki/Stat#Stat_modifiers 
    // 참고
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defnecse); }
    }

    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHp
    {
        get;private set;
    }

    public DamageDetails TakeDamage(Move move , Pokemon attacker)
    {
        //포켓몬의 크리티컬 확률 6.25%
        float critical = 1.0f;
        if (UnityEngine.Random.value * 100f <= 6.25f)
            critical = 2.0f;

        // 타입속성공격
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false,
        };

        // 포켓몬 데미지공식
        // https://bulbapedia.bulbagarden.net/wiki/Damage

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

        float modifiers = UnityEngine.Random.Range(0.85f, 1f) * type * critical ;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);

        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HpChanged = true;
    }

    public void SetStatus(ConditionID conditionId)
    {
        /*
         C#의 값 타입(int, float, bool, struct, enum 등)은 원래 null을 가질 수 없습니다.
        하지만 ?를 붙이면 null을 가질 수 있는 nullable 타입으로 변환됩니다.

        선언할때 ? null을 선언할수있게만들어준다
        int? a = null 가능해짐
        호출할때 ? 해당값이 null이면 실행하지않고 넘어감
        Status?.Invoke(this)
        Status가 null이면 실행하지않고 아무일도 안일어남

         */

        if (Status != null) return;
        Status = ConditionsDB.Conditions[conditionId];
        /*
         OnStart?.Invoke(this);는 이벤트가 null인지 확인한 후 현재 객체(this)를 인자로 전달하여 이벤트를 실행하는 코드
         ?. 연산자를 사용하면 이벤트에 구독자가 없을 때(null일 때) 예외가 발생하지 않고 그냥 넘어갈 수 있음.
         */
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }
    
    public void SetVolatileStatus(ConditionID conditionId)
    {
        if (VolatileStatus != null) return;
        VolatileStatus = ConditionsDB.Conditions[conditionId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }
    public void CureStatus()
    {
        Status = null;
        // Invoke()나 ()는 같은기능을하지만 널체크를하고싶을때
        // OnStatusChanged?.Invoke()를사용하자 널체크 필요없을때 간단히 OnStatusChanged()가능
        OnStatusChanged?.Invoke();
    }
    
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        /*Where 함수는 **LINQ(Language-Integrated Query)**에서 제공하는 메서드 중 하나로, 특정 조건을 만족하는 요소만 필터링(filtering)하는 역할을 합니다.
        즉, 컬렉션(배열, 리스트 등)에서 원하는 조건의 값만 추출하는 데 사용됩니다.
         Where가 반환하는 데이터는 IEnumerable<T>이므로, .ToList()를 사용하여 List<T>로 변환할 수 있습니다.
        Where는 조건을 만족하는 요소만 필터링하는 함수.
        람다식(n => 조건)을 사용하여 조건을 정의.
        객체 리스트에서도 사용 가능 (students.Where(s => s.Score > 80)).
        ToList()를 사용하면 필터링된 데이터를 List<T>로 변환 가능.

        (x => x.pp >0)
        (변수 => 조건)

        LINQ(Where Select OrderBy등등)은 컬렉션(리스트,딕셔너리 등등)을 SQL쿼리처럼 쓸수잇게만들드는기능
         */
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();

        /*                   UnityEngine.Random vs System.Random
         	                UnityEngine.Random	                        System.Random
         네임스페이스 	    UnityEngine	                                System
         용도	            유니티 전용 난수 생성기	                    일반적인 C# 난수 생성기
         난수 생성 방식	    게임 오브젝트 및 물리 연산에 최적화됨	        일반적인 의사 난수 생성기(PRNG)
         Seed 설정 가능 여부	Random.InitState(int seed) 사용	            new Random(int seed) 생성자로 설정
         float 난수 범위    	Random.Range(0f, 1f) → [0, 1] 범위 포함	    (float)new Random().NextDouble() → (0, 1) 범위 (1 포함 안 함)
         int 난수 범위	    Random.Range(min, max) → max 포함됨	        new Random().Next(min, max) → max 미포함
         멀티스레드 안전성	static이라 멀티스레드에서 문제 발생 가능	    Random 인스턴스를 따로 만들면 멀티스레드 안전
         */

        int r = UnityEngine.Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public bool OnBeforeMove()
    {
        bool canPerfromMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
                canPerfromMove = false;                  
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
                canPerfromMove = false;
        }


        return canPerfromMove;
    }

    //턴이 끝나고 실행할함수
    public void OnAfterTurn()
    {
        Status?.onAfterTurn?.Invoke(this);
        VolatileStatus?.onAfterTurn?.Invoke(this);
    }

    //배틀이 끝나고 실행할함수
    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
}

// 데미지 세부사항클래스
public class DamageDetails
{
    public bool Fainted { get; set; }

    public float Critical { get; set; }

    public float TypeEffectiveness { get; set; }
}
