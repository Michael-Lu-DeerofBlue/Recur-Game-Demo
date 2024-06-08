using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInfo : MonoBehaviour
{
    public int HitPoint;
    public int MaxHitPoint;
    public int AttackValue;
    private float MaxWeight;
    public string WeaponType;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExecuteBehavior(string behavior)
    {
        //switch (behavior)
        //{
        //    case "0010FFFF":
        //        Debug.Log("Executing Blue Behavior");
        //        break;
        //    case "00FF07FF":
        //        Debug.Log("Executing Green Behavior");
        //        break;
        //    case "FF0000FF":
        //        Debug.Log("Executing Red Behavior");
        //        break;
        //    case "C9C200FF":
        //        Debug.Log("Executing Yellow Behavior");
        //        break;
        //    case "FF8600FF":
        //        Debug.Log("Executing Orange Behavior");
        //        break;
        //    case "A400FFFF":
        //        Debug.Log("Executing Purple Behavior");
        //        break;
        //    case "0100C2FF":
        //        Debug.Log("Executing Dark Blue Behavior");
        //        break;
        //    default:
        //        Debug.Log("Unknown behavior");
        //        break;
        //}
        AttackEnemy();
    }

    public void HitHandle(int damage)
    {
        HitPoint -= damage;
        Debug.Log("Player is hit. HP: " + HitPoint);
        if (HitPoint <= 0)
        {
            HitPoint = 0;
            Debug.Log("Player is dead.");
        }
    }

    void AttackEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 1)
        {
            BaseEnemy enemy = enemies[0].GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.HitHandle(AttackValue);
            }
        }
        else if (enemies.Length > 1)
        {
            Debug.Log("muti enemies function will be added later.");
        }
    }
}