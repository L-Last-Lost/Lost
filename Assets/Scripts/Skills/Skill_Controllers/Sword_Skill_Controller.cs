using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;
    private float returningSpeed;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTime;

    [Header("Pierce info")]
    private int pierceAmount;


    [Header("Bounce info")]
    [SerializeField] private float bounceSpeed = 20;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;
    private bool isTouchFirstEnemy = false;

    private float hitTimer;
    private float hitCooldown;
    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
        anim = GetComponentInChildren<Animator>();

    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    // 没有继承Skill，也就没有 player实例
    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _returningSpeed, float _freezeTime)
    {
        player = _player;
        rb.gravityScale = _gravityScale;
        rb.velocity = _dir;
        returningSpeed = _returningSpeed;
        freezeTime = _freezeTime;

        if (pierceAmount <= 0)
            anim.SetBool("Rotation", true);

        Invoke("DestroyMe", 7);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounces)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;

        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration,float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }
    public void ReturningSword()
    {
        canRotate = true;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
        
        if(pierceAmount<=0)
            anim.SetBool("Rotation", true);
    }
    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Vector2.Distance(transform.position, player.transform.position) * returningSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
            {
                player.CatchTheSword();
                isReturning = false;
                anim.SetBool("Rotation", false);
            }
        }

        BounceLogic();

        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenHitEnemy();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;
                hitTimer -= Time.deltaTime;

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
                    foreach (var hit in colliders)
                    {
                        // 是敌人，有敌人脚本
                        if (hit.GetComponent<Enemy>() != null)
                            hit.GetComponent<Enemy>().Damage();
                    }
                }
            }
        }
    }

    private void StopWhenHitEnemy()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        if (!isTouchFirstEnemy)
        {
            isTouchFirstEnemy = true;
            spinTimer = spinDuration;
        }
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                enemyTarget[targetIndex].GetComponent<Enemy>().Damage();
                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            enemy.Damage();
            enemy.StartCoroutine("FreezeTimerFor", freezeTime);
        }


        collision.GetComponent<Enemy>()?.Damage();
        
        SetupTargetsForBounce(collision);

        StuckInto(collision);
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    // 是敌人，有敌人脚本
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if(pierceAmount>0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenHitEnemy();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0)
            return;

        anim.SetBool("Rotation", false);
        // 位置跟随碰撞物体
        transform.parent = collision.transform;
    }
}
