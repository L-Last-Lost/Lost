using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}
public class ThrowSword_Skill : Skill
{

    public SwordType swordType = SwordType.Regular;

    [Header("Bounce info")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;

    [Header("Peirce info")]
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private float hitCooldown;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinGravity;

    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTime;

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;  // 获取描点原型
    [SerializeField] private Transform dotsParent;  // Unity放点的位置

    [Header("Return info")]
    [SerializeField] private float returningSpeed;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        // 首先创造出点
        GenerationDots();

        SetupGravity();
    }

    private void SetupGravity()
    {
        switch (swordType)
        {
            case SwordType.Regular:
                break;
            case SwordType.Pierce:
                swordGravity = pierceGravity;
                break;
            case SwordType.Bounce:
                swordGravity = bounceGravity;
                break;
            case SwordType.Spin:
                swordGravity = spinGravity;
                break;
        }
    }

    protected override void Update()
    {   
        if (Input.GetKeyUp(KeyCode.Q))
        {
            // 最终方向
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            for(int i = 0; i < dots.Length; i++) 
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }
    public void CreateSword()
    {
        // 创建新剑，原型，位置，旋转方向
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        // 这个原型中有控制脚本，这一步便是获取该脚本，有创建实体的prefab应该都需要有一个脚本，也就需要一个Controller吗
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();
        
        
        if(swordType == SwordType.Bounce)
        {
            newSwordScript.SetupBounce(true, bounceAmount);
        }
        switch (swordType)
        {
            case SwordType.Regular:
                break;
            case SwordType.Pierce:
                swordGravity = pierceGravity;
                newSwordScript.SetupPierce(pierceAmount);
                break;
            case SwordType.Bounce:
                swordGravity = bounceGravity;
                newSwordScript.SetupBounce(true, bounceAmount);
                break;
            case SwordType.Spin:
                swordGravity = spinGravity;
                newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);
                break;
        }





        //当然用这个获取到的脚本去控制这把剑的生成
        newSwordScript.SetupSword(finalDir, swordGravity,player,returningSpeed, freezeTime);
        // 标志已经生成剑，玩家获得一把剑
        player.AssignNewSword(newSword);
        // 不再生成点
        DotsActive(false);
    }
    
    //获得鼠标指向的位置
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerationDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            // 把点放在了 dotsParent中
            dots[i] = Instantiate(dotPrefab,player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        // 这里的重力就是y方向的，因此不需要拆开来计算
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y )* t + .5f * (Physics2D.gravity * swordGravity) * (t * t);
        
        return position;
    }
}
 