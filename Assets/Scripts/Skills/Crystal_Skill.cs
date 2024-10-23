using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Explosive crystal")]
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private bool canUSeMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float mutiStackCooldown;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    public override void UseSkill()
    {
        base.UseSkill();


        if (currentCrystal == null)
        {
            currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
            Crystal_Skill_Controller currentCrystalScrpit = currentCrystal.GetComponent<Crystal_Skill_Controller>();

            currentCrystalScrpit.SetupCrystal(crystalDuration,canExplode,canMoveToEnemy,moveSpeed, FindClosestEnemy(currentCrystal.transform));
        }
        else
        {

            Vector2 playerPos = player.transform.position;

            player.transform.position = currentCrystal.transform.position;
            
            currentCrystal.transform.position = playerPos;
            currentCrystal.GetComponent<Crystal_Skill_Controller>().FinishCrystal();
        }
    }
    
    private bool CanUseMultiCrystal()
    {
        if (canUSeMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform));


                if(crystalLeft.Count <= 0)
                {

                }
            }

            return true;
        }

        return false;
    }
    private void RefillCrystal()
    {
        for(int i = 0; i < amountOfStacks; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }
}
