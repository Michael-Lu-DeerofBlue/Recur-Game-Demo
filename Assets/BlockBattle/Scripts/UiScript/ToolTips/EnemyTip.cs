using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Loading;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
[ExecuteInEditMode]
public class EnemyTip : MonoBehaviour
{
    public TextMeshProUGUI NameField;
    public TextMeshProUGUI HPField;
    public TextMeshProUGUI CastingTimeField;
    public TextMeshProUGUI NextMoveName;
    private string SkillDetail;
    public TextMeshProUGUI NextMoveDetail;
    private BattleManager battleManager;
    private TipsInfo tipsinfo;

    private void Start()
    {
        tipsinfo = FindObjectOfType<TipsInfo>();
        battleManager =FindObjectOfType<BattleManager>();
    }

    public void SetText(string Name, string HP, string CastingTime, string NextMove, int nextskilldamage)
    {
        if (battleManager.ToolTipsLevel == 0)
        {
            NameField.text = tipsinfo.GetEnemyChineseName(Name);
            HPField.text = "HP: "+HP;
            CastingTimeField.text = "倒计时： "+CastingTime;
            NextMoveName.text = "技能： " + tipsinfo.GetEnemySkillChineseName(NextMove);
            NextMoveDetail.text = "效果： " + tipsinfo.GetEnemySkillDetail(NextMove, nextskilldamage); ;
        }
        else if (battleManager.ToolTipsLevel == 1)
        {
            NameField.text = Name;
            HPField.text = HP;
            CastingTimeField.text = CastingTime;
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
    }


}
