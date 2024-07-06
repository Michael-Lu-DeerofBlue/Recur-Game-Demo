using TMPro;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
public class ToolTip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement LayoutElement;
    public int characterWrapLimit;
    private BattleManager battleManager;

    public void SetText(string header, string content)
    {
       battleManager=FindAnyObjectByType<BattleManager>();
        if (battleManager.ToolTipsLevel == 0)
        {
            headerField.text = header;
            contentField.text = content;
        }else if(battleManager.ToolTipsLevel == 1)
        {
            headerField.text = header;
            contentField.text = "";
        }
    }

    private void Update()
    {
        Vector2 position = Input.mousePosition;
        position.x += 30f;
        transform.position = position;
    }

}



