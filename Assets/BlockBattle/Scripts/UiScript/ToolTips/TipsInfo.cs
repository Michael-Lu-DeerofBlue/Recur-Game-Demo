using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsInfo : MonoBehaviour
{
    string header = string.Empty;
    string content = string.Empty;
    string detail1= string.Empty;
    string detail2 = string.Empty;
    string detail3= string.Empty;
    string detail4= string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void FindBlockTipsContext(int colorindex)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            LongSword longSword = player.GetComponent<LongSword>();
            if (longSword != null)
            {

                switch (colorindex)
                {
                    case 0:
                        header = "�ṥ��";
                        content="����˺�; ʹ��������ͣ��";
                        detail1 = "2���˺�";
                        detail2 = "4���˺���ͣ��2��";
                        detail3 = "7���˺���ͣ��3��";
                        detail4 = "10���˺���ͣ��5��";
                        TTooltipSystem.showBlockTips(header,content,detail1,detail2,detail3,detail4);
                        break;
                    case 1:
                        header = "����";
                        content = "�ָ���������ֵ";
                        detail1 = "5������ֵ";
                        detail2 = "8������ֵ";
                        detail3 = "12������ֵ";
                        detail4 = "15������ֵ";

                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 2:
                        header = "�ع���";
                        content = "����˺� ";
                        detail1 = "5���˺�";
                        detail2 = "8���˺�";
                        detail3 = "12���˺�";
                        detail4 = "15���˺�";
                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 3:
                        header = "�ָ�����";
                        content = "�Ƴ�������״̬";
                        detail1 = "1������";
                        detail2 = "1������";
                        detail3 = "1������";
                        detail4 = "2������";
                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 4:
                        header = "����";
                        content = "���6���˺�; ʹĿ������";
                        detail1 = "3���˺�";
                        detail2 = "5���˺�";
                        detail3 = "6���˺���ʹĿ������";
                        detail4 = "8���˺���ʹĿ������";
                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 5:
                        header = "�񵲼���";
                        content = "��ø񵲴������ɵ����˺�";
                        detail1 = "1���";
                        detail2 = "1���";
                        detail3 = "2���";
                        detail4 = "2���";

                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 6:
                        header = "ŭ��";
                        content = "���1���˺������������4�е�ͬʱ����4�����飬���30���˺�";
                        detail1 = "���1���˺�";
                        detail2 = "���1���˺�";
                        detail3 = "���1���˺�";
                        detail4 = "��������ʱ���˺�+29";
                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 8:
                        header = "��Ч����";
                        content = "Ч������";
                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                }
            }
            else
            {
                Debug.Log("Player object does not have LongSword script.");
            }
        }
        else
        {
            Debug.Log("Player object not found.");
        }
    }

    public void FindInventoryTipsContext(string Name)
    {


                switch (Name)
                {
                    case "MedKit":
                        header = "ҽ�ư�";
                        content = "�ָ�10��HP";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "SprayCan":
                        header = "�����";
                        content = "ˢ��ѡ����������ָ���";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "Mint":
                        header = "��ˬ������";
                        content = "�Ƴ����м��棬����10�������ߴ󲿷ּ���";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "PaperCutter":
                        header = "��ֽ��";
                        content = "��Ŀ��ʩ��1�����";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "FracturedPocketWatch":
                        header = "����Ļ���";
                        content = "�����е���ʩ��ͣ�ͣ�����3��";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;

                }
            }





    // Update is called once per frame
    void Update()
    {
        
    }
}
