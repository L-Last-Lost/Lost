using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{

    private Enemy enemy;
    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        enemy.Damage();
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("die");
        enemy.Die();
    }
}
