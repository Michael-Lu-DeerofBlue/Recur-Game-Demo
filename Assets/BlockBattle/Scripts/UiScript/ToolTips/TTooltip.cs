using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Loading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
[ExecuteInEditMode]
public class TTooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI ContentField;
    public LayoutElement LayoutElement;
    public int characterWrapLimit;
    private BattleManager battleManager;

    public void SetText(string header, string content)
    {
        battleManager = FindAnyObjectByType<BattleManager>();
        if (battleManager.ToolTipsLevel == 0)
        {
            headerField.text = header;
            ContentField.text = content;
        }
        else if (battleManager.ToolTipsLevel == 1)
        {
            headerField.text = header;
            ContentField.text = "";
        }
    }

    private void Update()
    {
        if (Application.isEditor) { 
        int headerLength= headerField.text.Length;
        int contentLength= ContentField.text.Length;

        LayoutElement.enabled = (headerLength>characterWrapLimit||contentLength>characterWrapLimit)? true:false;
        }
        Vector2 position = Input.mousePosition;
        position.x += 30;
        if (position.x > 2000)
        {
            RectTransform rectTransform = transform as RectTransform;
            position.x -= rectTransform.rect.width;
        }
        if (position.y < 180)
        {
            RectTransform rectTransform = transform as RectTransform;
            position.y += rectTransform.rect.height;
        }
        transform.position = position;
    }


}
