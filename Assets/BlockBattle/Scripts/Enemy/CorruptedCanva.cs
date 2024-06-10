using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CorruptedCanva : Enemy
{
    public int attackdamage = 2;
    public int OrruptingWindDamage = 2;
    public int attackWeight = 2;
    public int PaintingSplashWeight = 1;
    public int corruptingWindWeight = 1;

    public override void ExecuteTurn()
    {
        int sum = attackWeight + PaintingSplashWeight + corruptingWindWeight;
        float attackProbability = (float)attackWeight / sum;
        float paintingSplashProbability = (float)PaintingSplashWeight / sum;
        float corruptingWindProbability = (float)corruptingWindWeight / sum;
        float randomValue = Random.value;
        if (randomValue < attackProbability)
        {
             Attack(attackdamage);  // Attack action
        }
        else if (randomValue < attackProbability + paintingSplashProbability)
        {
            LockRotation();  // PaintingSplash action
        }
        else
        {
            Attack(OrruptingWindDamage);
            Debug.Log("LockRatationforNextBlock");// CorruptingWind action
        }
    }
}
    // Start is called before the first frame update
