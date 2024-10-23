
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Major Stats")]
    public Stat strength;
    public Stat agility;
    public Stat intelligence;
    public Stat vitality;

    [Header("Offensive Stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    [Header("Magic Resistance")]
    public Stat fireResistance;
    public Stat iceResistance;
    public Stat lightingResistance;



    [Header("Magic Sustain")]
    public float ignitedTimeLast;
    public float ignitedCooldownTimeLast;
    public float chilledTimeLast;
    public float shockedTimeLast;
    
    public float ignitedTimer;
    public float ignitedCooldownTimer;
    public float chilledTimer;
    public float shockedTimer;

    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;

    [SerializeField] private int currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
        critPower.SetDefaultVal(150);
    }

    protected virtual void Update()
    {
        UpdateElementCondition();
    }

    private void UpdateElementCondition()
    {
        ignitedTimer -= Time.deltaTime;
        ignitedCooldownTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        if (ignitedCooldownTimer < 0 && isIgnited)
        {
            TakeDamage(3);
            // Debug.Log("fire");
            ignitedCooldownTimer = ignitedCooldownTimeLast;
        }

        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }

        if (chilledTimer < 0)
        {
            isChilled = false;
        }

        if (shockedTimer < 0)
        {
            isShocked = false;
        }
    }

    public virtual void TakeDamage(int _damage)
    {
        currentHealth -= _damage;

        if (currentHealth < 0)
            Die();
    }

    protected virtual void Die()
    {
        //throw new NotImplementedException();
    }

    public virtual void DoDamage(CharacterStats _target)
    {
        if (TargetCanAvoidAttack(_target))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_target, totalDamage);

        _target.TakeDamage(totalDamage);
    }

    public virtual void DoMagicalDamage(CharacterStats _target)
    {
        int totalMagicalDamage = ElementCalculator(_target);

        _target.TakeDamage(totalMagicalDamage);
    }

    private int ElementCalculator(CharacterStats _target)
    {
        int _fireDamage = fireDamage.GetValue() + intelligence.GetValue();
        int _iceDamage = iceDamage.GetValue() + intelligence.GetValue();
        int _lightingDamage = lightingDamage.GetValue() + intelligence.GetValue();

        _target.fireResistance.MinusValue(_fireDamage);
        _target.iceResistance.MinusValue(_iceDamage);
        _target.lightingResistance.MinusValue(_lightingDamage);

        bool ignited = false;
        bool chilled = false;
        bool shocked = false;

        if (_target.fireResistance.GetValue() < 0)
            ignited = true;
        if (_target.iceResistance.GetValue() < 0)
            chilled = true;
        if (_target.lightingDamage.GetValue() < 0)
            shocked = true;

        _target.ApplyAilment(ignited, chilled, shocked);

        int totalMagicalDamage = Mathf.RoundToInt(_fireDamage * 0.1f) + Mathf.RoundToInt(_iceDamage * 0.1f)
            + Mathf.RoundToInt(_lightingDamage * 0.1f) + Mathf.RoundToInt(intelligence.GetValue() * 0.2f);

        totalMagicalDamage = CheckTargetMagicalResistance(_target, totalMagicalDamage);
        return totalMagicalDamage;
    }

    private static int CheckTargetMagicalResistance(CharacterStats _target, int totalMagicalDamage)
    {
        totalMagicalDamage -= _target.magicResistance.GetValue() + (_target.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public void ApplyAilment(bool _ignite,bool _chill, bool _shock)
    {

        if (_ignite)
        {
            isIgnited = _ignite;
            ignitedTimer = ignitedTimeLast;
            ignitedCooldownTimer = ignitedCooldownTimeLast;
        }
        if (_chill)
        {
            isChilled = _chill;
            chilledTimer = chilledTimeLast;
        }
        if (_shock)
        {
            isShocked = _shock;
            shockedTimer = shockedTimeLast;
        }
    }

    private int CheckTargetArmor(CharacterStats _target, int totalDamage)
    {
        totalDamage -= _target.armor.GetValue();

        if (_target.isChilled)
        {
            totalDamage -= 20;
        }

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _target)
    {
        int totalEvasion = _target.evasion.GetValue() + _target.agility.GetValue();

        if (isShocked)
        {
            totalEvasion += 20;
        }

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }

        return false;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) < totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;

        float critDamage = totalCritPower * _damage;

        return Mathf.RoundToInt(critDamage);
    }
}
