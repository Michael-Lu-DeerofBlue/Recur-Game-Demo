using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Loading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
[ExecuteInEditMode]
public class BlockTip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI ContentField;
    public TextMeshProUGUI detailText1;
    public TextMeshProUGUI detailText2;
    public TextMeshProUGUI detailText3;
    public TextMeshProUGUI detailText4;
    private BattleManager battleManager;
    public GameObject detailblock1;
    public GameObject detailblock2;
    public GameObject detailblock3;
    public GameObject detailblock4;

    public void SetText(string header, string content,string detail1,string detail2,string detail3, string detail4)
    {
        battleManager = FindAnyObjectByType<BattleManager>();
        if (battleManager.ToolTipsLevel == 0)
        {
            headerField.text = header;
            ContentField.text = content;
            detailText1.text = detail1;
            detailText2.text = detail2; 
            detailText3.text = detail3;
            detailText4.text = detail4;
        }
        else if (battleManager.ToolTipsLevel == 1)
        {
            headerField.text = header;
            ContentField.text = content;
            detailText1.text = "";
            detailText2.text = "";
            detailText3.text = "";
            detailText4.text = "";

        }
    }

    private void Update()
    {
        Vector2 position = Input.mousePosition;
        position.x += 30;
        if (position.x > 2000)
        {
            RectTransform rectTransform = transform as RectTransform;
            position.x -= rectTransform.rect.width;
        }
        if (position.y < 880)
        {
            RectTransform rectTransform = transform as RectTransform;
            position.y += rectTransform.rect.height;
        }
        transform.position = position;
        Debug.Log(transform.position);
    }


}
