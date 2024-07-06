using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsInfo : MonoBehaviour
{
    string header = string.Empty;
    string content= string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void FindToolTipsContext(int colorindex)
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
                        header = "redblock";
                        content="longsword 红色 red block skill ";
                        TooltipSystem.Show(header,content);
                        break;
                    case 1:
                        header = "Greenblock";
                        content = "longsword green block skill ";
                        TooltipSystem.Show(header, content);
                        break;
                    case 2:
                        header = "Orangeblock";
                        content = "longsword Orange block skill 橙色技能 ";
                        TooltipSystem.Show(header, content);
                        break;
                    case 3:
                        header = "DBlueblock";
                        content = "longsword Dblue block skill ";
                        TooltipSystem.Show(header, content);
                        break;
                    case 4:
                        header = "Purpleblock";
                        content = "longsword Purple block skill ";
                        TooltipSystem.Show(header, content);
                        break;
                    case 5:
                        header = "Yellowblock";
                        content = "longsword yellow block skill ";
                        TooltipSystem.Show(header, content);
                        break;
                    case 6:
                        header = "Lightblueblock";
                        content = "longsword lb block skill ";
                        TooltipSystem.Show(header, content);
                        break;
                    case 8:
                        header = "stunblock";
                        content = "do nothing block ";
                        TooltipSystem.Show(header, content);
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


    // Update is called once per frame
    void Update()
    {
        
    }
}
