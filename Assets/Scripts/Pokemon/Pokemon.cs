using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Ŭ������ ����ȭ�Ҷ�
[System.Serializable]
public class Pokemon
{
    // ���̽�Ŭ����
    [SerializeField] PokemonBase _base;
    //���糪�Ƿ���
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

    // �����������մ� Moves
    public List<Move> Moves { get; set; }

    // ���繫��
    public Move CurrentMove { get; set; }
    // �̸� �����س��� ��ųʸ�
    public Dictionary<Stat,int> Stats { get; private set; }

    // �󸶳� �ν�Ʈ�Ǿ����� �����ϴ� ��ųʸ�
    // ���ϸ󿡼� �ν�Ʈ�� -6 ~ +6���� �ν��õȴ�.
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

    // Stat�� �̸� ����ϴ��Լ�.
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

    // ������ �������Լ�
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];

        // �ν�Ʈ���
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        // ���ϸ���� �ν�Ʈ����
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
                    kor = "���ݷ�";
                    break;
                case Stat.Defnecse:
                    kor = "����";
                    break;
                case Stat.SpAttack:
                    kor = "Ư�����ݷ�";

                    break;
                case Stat.SpDefense:
                    kor = "Ư������";
                    break;
                case Stat.Speed:
                    kor = "���ǵ�";
                    break;
            }

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}�� {kor}(��)�� �����߽��ϴ�!");
            else
                StatusChanges.Enqueue($"{Base.Name}�� {kor}(��)�� �����߽��ϴ�!");


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

    // ���ϸ� ���Ȱ���
    // https://bulbapedia.bulbagarden.net/wiki/Stat#Stat_modifiers 
    // ����
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
        //���ϸ��� ũ��Ƽ�� Ȯ�� 6.25%
        float critical = 1.0f;
        if (UnityEngine.Random.value * 100f <= 6.25f)
            critical = 2.0f;

        // Ÿ�ԼӼ�����
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false,
        };

        // ���ϸ� ����������
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
         C#�� �� Ÿ��(int, float, bool, struct, enum ��)�� ���� null�� ���� �� �����ϴ�.
        ������ ?�� ���̸� null�� ���� �� �ִ� nullable Ÿ������ ��ȯ�˴ϴ�.

        �����Ҷ� ? null�� �����Ҽ��ְԸ�����ش�
        int? a = null ��������
        ȣ���Ҷ� ? �ش簪�� null�̸� ���������ʰ� �Ѿ
        Status?.Invoke(this)
        Status�� null�̸� ���������ʰ� �ƹ��ϵ� ���Ͼ

         */

        if (Status != null) return;
        Status = ConditionsDB.Conditions[conditionId];
        /*
         OnStart?.Invoke(this);�� �̺�Ʈ�� null���� Ȯ���� �� ���� ��ü(this)�� ���ڷ� �����Ͽ� �̺�Ʈ�� �����ϴ� �ڵ�
         ?. �����ڸ� ����ϸ� �̺�Ʈ�� �����ڰ� ���� ��(null�� ��) ���ܰ� �߻����� �ʰ� �׳� �Ѿ �� ����.
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
        // Invoke()�� ()�� ��������������� ��üũ���ϰ������
        // OnStatusChanged?.Invoke()��������� ��üũ �ʿ������ ������ OnStatusChanged()����
        OnStatusChanged?.Invoke();
    }
    
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        /*Where �Լ��� **LINQ(Language-Integrated Query)**���� �����ϴ� �޼��� �� �ϳ���, Ư�� ������ �����ϴ� ��Ҹ� ���͸�(filtering)�ϴ� ������ �մϴ�.
        ��, �÷���(�迭, ����Ʈ ��)���� ���ϴ� ������ ���� �����ϴ� �� ���˴ϴ�.
         Where�� ��ȯ�ϴ� �����ʹ� IEnumerable<T>�̹Ƿ�, .ToList()�� ����Ͽ� List<T>�� ��ȯ�� �� �ֽ��ϴ�.
        Where�� ������ �����ϴ� ��Ҹ� ���͸��ϴ� �Լ�.
        ���ٽ�(n => ����)�� ����Ͽ� ������ ����.
        ��ü ����Ʈ������ ��� ���� (students.Where(s => s.Score > 80)).
        ToList()�� ����ϸ� ���͸��� �����͸� List<T>�� ��ȯ ����.

        (x => x.pp >0)
        (���� => ����)

        LINQ(Where Select OrderBy���)�� �÷���(����Ʈ,��ųʸ� ���)�� SQL����ó�� �����հԸ����±��
         */
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();

        /*                   UnityEngine.Random vs System.Random
         	                UnityEngine.Random	                        System.Random
         ���ӽ����̽� 	    UnityEngine	                                System
         �뵵	            ����Ƽ ���� ���� ������	                    �Ϲ����� C# ���� ������
         ���� ���� ���	    ���� ������Ʈ �� ���� ���꿡 ����ȭ��	        �Ϲ����� �ǻ� ���� ������(PRNG)
         Seed ���� ���� ����	Random.InitState(int seed) ���	            new Random(int seed) �����ڷ� ����
         float ���� ����    	Random.Range(0f, 1f) �� [0, 1] ���� ����	    (float)new Random().NextDouble() �� (0, 1) ���� (1 ���� �� ��)
         int ���� ����	    Random.Range(min, max) �� max ���Ե�	        new Random().Next(min, max) �� max ������
         ��Ƽ������ ������	static�̶� ��Ƽ�����忡�� ���� �߻� ����	    Random �ν��Ͻ��� ���� ����� ��Ƽ������ ����
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

    //���� ������ �������Լ�
    public void OnAfterTurn()
    {
        Status?.onAfterTurn?.Invoke(this);
        VolatileStatus?.onAfterTurn?.Invoke(this);
    }

    //��Ʋ�� ������ �������Լ�
    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
}

// ������ ���λ���Ŭ����
public class DamageDetails
{
    public bool Fainted { get; set; }

    public float Critical { get; set; }

    public float TypeEffectiveness { get; set; }
}
