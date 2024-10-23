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
    [SerializeField] private GameObject dotPrefab;  // ��ȡ���ԭ��
    [SerializeField] private Transform dotsParent;  // Unity�ŵ��λ��

    [Header("Return info")]
    [SerializeField] private float returningSpeed;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        // ���ȴ������
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
            // ���շ���
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
        // �����½���ԭ�ͣ�λ�ã���ת����
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        // ���ԭ�����п��ƽű�����һ�����ǻ�ȡ�ýű����д���ʵ���prefabӦ�ö���Ҫ��һ���ű���Ҳ����Ҫһ��Controller��
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





        //��Ȼ�������ȡ���Ľű�ȥ������ѽ�������
        newSwordScript.SetupSword(finalDir, swordGravity,player,returningSpeed, freezeTime);
        // ��־�Ѿ����ɽ�����һ��һ�ѽ�
        player.AssignNewSword(newSword);
        // �������ɵ�
        DotsActive(false);
    }
    
    //������ָ���λ��
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
            // �ѵ������ dotsParent��
            dots[i] = Instantiate(dotPrefab,player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        // �������������y����ģ���˲���Ҫ��������
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y )* t + .5f * (Physics2D.gravity * swordGravity) * (t * t);
        
        return position;
    }
}
 