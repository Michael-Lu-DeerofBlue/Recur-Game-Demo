using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour
{
    public Text damageText; // 连接 UI Text 组件
    void Start()
    {
        damageText = GetComponent<Text>();
    }
    public void ShowDamageNumber(float damage)
    {
        int roundedDamage = Mathf.RoundToInt(damage);
        damageText.text = roundedDamage.ToString();
        StartCoroutine(AnimateDamageNumber());
    }

    private IEnumerator AnimateDamageNumber()
    {
        Color originalColor = damageText.color;
        Vector3 originalPosition = transform.position;


        damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        float duration = 0.1f;
        float elapsedTime = 0f;

        // set a value to 1 in 0.1 second
        while (elapsedTime < duration)
        {
            damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0, 1, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);

        // parabola movement
        float totalDuration = 0.7f;
        elapsedTime = 0f;
        Vector3 peakPosition = originalPosition + new Vector3(-10, 9, 0);
        Vector3 endPosition = peakPosition + new Vector3(0, -9, 0);

        while (elapsedTime < totalDuration)
        {
            float t = elapsedTime / totalDuration;
            
            Vector3 currentPosition = Vector3.Lerp(Vector3.Lerp(originalPosition, peakPosition, t), Vector3.Lerp(peakPosition, endPosition, t), t);
            transform.position = currentPosition;

            damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        transform.position = originalPosition;
    }
}

