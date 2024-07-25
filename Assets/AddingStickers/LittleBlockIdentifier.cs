using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LittleBlockIdentifier : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 position;
    public Image placeHolder;  // Reference to the UI Image component
    public Sprite trans;
    public Sprite critical;
    public Sprite pierce;
    public Sprite sober;
    public Sprite swordMaster;
    public Sprite gunslinger;
    public bool clicked;
    void Start()
    {
        
    }

    // Update is called once per frame
    public void CallBigBlock()
    {
        clicked = true;
        gameObject.transform.parent.GetComponent<BigBlockIdentifier>().CheckPosition(position);
    }

    public void EnableInteractive()
    {
        gameObject.GetComponent<Button>().interactable = true;
        if (clicked)
        {
            gameObject.GetComponent<Image>().raycastTarget = true;
            clicked = false;
        }
    }

    public void DisableInteractive()
    {
        gameObject.GetComponent<Button>().interactable = false;
        if (clicked)
        {
            gameObject.GetComponent<Button>().interactable = true;
            gameObject.GetComponent<Image>().raycastTarget = false;
        }
    }

    public void UpdateAppearence()
    {
        bool haveMe = false;
        foreach (StickerInfo.StickerData sticker in AddStickerManager.stickerData)
        {
            if (sticker.IndexShape == gameObject.transform.parent.GetComponent<BigBlockIdentifier>().block) //block match
            {
                if (sticker.position1 == position)
                {
                    haveMe = true;
                    gameObject.transform.parent.GetComponent<BigBlockIdentifier>().stickerPosition.Add(position);
                    switch (sticker.StickerName)
                    {
                        case "Critical":
                            placeHolder.sprite = critical;
                            break;
                        case "Pierce":
                            placeHolder.sprite = pierce;
                            break;
                        case "Sober":
                            placeHolder.sprite = sober;
                            break;
                        case "SwordMaster":
                            placeHolder.sprite = swordMaster;
                            break;
                        case "Gunslinger":
                            placeHolder.sprite = gunslinger;
                            break;
                        default:
                            placeHolder.sprite = null;
                            break;
                    }
                }
            }
        }
        if (!haveMe)
        {
            placeHolder.sprite = trans;
        }

    }
}
