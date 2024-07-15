using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static StickerInfo;

public class BigBlockIdentifier : MonoBehaviour
{
    public List<Vector3> stickerPosition;
    public int block;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void CheckPosition(Vector3 position)
    {
        if (stickerPosition.Contains(position))
        {
            //Remove
            gameObject.transform.parent.GetComponent<AddStickerManager>().DeletePositionBlock(position, block);
            stickerPosition.Remove(position);
        }
        else
        {
            //Add
            if (stickerPosition.Count <= 1)
            {
                stickerPosition.Add(position);
                gameObject.transform.parent.GetComponent<AddStickerManager>().AddPositionBlock(position, block);
            }
            else
            {
                BroadcastMessage("EnableInteractive");
            }
        }
    }

    public void UpdateAppearence()
    {
        stickerPosition.Clear();
    }
}
