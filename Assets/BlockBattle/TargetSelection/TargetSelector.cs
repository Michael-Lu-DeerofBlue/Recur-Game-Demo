using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TargetSelector : MonoBehaviour
{
    // Start is called before the first frame update
    private HeroInfo heroInfo;
    private Enemy[] enemies;
    private Enemy leftTopEnemy;
    public Enemy CurrentTarget;
    private int currentIndex = -1;
    void Start()
    {
        heroInfo = FindObjectOfType<HeroInfo>();
    }

    public void SelectLeftTopTarget()
    {
        enemies = FindObjectsOfType<Enemy>().Where(enemy => !enemy.isdead).ToArray();

        if (enemies.Length == 0)
        {
            Debug.Log("No enemies found");
            return;

        }

        leftTopEnemy = enemies[0];
        Vector3 leftTopPosition = leftTopEnemy.transform.position;

        foreach (var enemy in enemies)
        {
            Vector3 position = enemy.transform.position;
            Debug.Log($"Name: {enemy.name}, Position: {position}");
            if (position.x < leftTopPosition.x || (position.x == leftTopPosition.x && position.y > leftTopPosition.y))
            {
                leftTopEnemy = enemy;
                leftTopPosition = position;
            }
        }
        transform.position = leftTopPosition;
        CurrentTarget = leftTopEnemy;
        heroInfo.SetSelectedEnemy(CurrentTarget);
    }

    public void SwitchTargetByClick(Enemy Target)
    {
        transform.position = Target.transform.position;
        CurrentTarget = Target;
        heroInfo.SetSelectedEnemy(CurrentTarget);
    }

    public void SwitchTargetByArrow()
    {
        enemies = FindObjectsOfType<Enemy>().Where(enemy => !enemy.isdead).ToArray();
    }
    public void SwitchTargetWhileIsDead()
    {
        enemies = FindObjectsOfType<Enemy>().Where(enemy => !enemy.isdead).ToArray();

        if (enemies.Length == 0)
        {
            Debug.Log("No enemies found");
            return;
        }

        Enemy nearestRightEnemy = null;
        float nearestRightDistance = float.MaxValue;
        Vector3 currentPosition = transform.position;

        foreach (var enemy in enemies)
        {
            Vector3 position = enemy.transform.position;
            float xDifference = position.x - currentPosition.x;
            float yDifference = Mathf.Abs(position.y - currentPosition.y);
            if (xDifference > 0 && yDifference <= 5 && xDifference < nearestRightDistance)
            {
                nearestRightDistance = xDifference;
                nearestRightEnemy = enemy;
            }
        }

        if (nearestRightEnemy != null)
        {
            CurrentTarget = nearestRightEnemy;
        }
        else
        {
            float nearestDistance = float.MaxValue;
            Enemy nearestEnemy = null;

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(currentPosition, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            CurrentTarget = nearestEnemy;
        }

        if (CurrentTarget != null)
        {
            transform.position = CurrentTarget.transform.position;
            heroInfo.SetSelectedEnemy(CurrentTarget);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
