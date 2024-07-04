using UnityEngine;

[System.Serializable]
public class ExcelConversationData
{
    [SerializeField] string actor;
    public string Actor { get { return this.actor; } set { this.actor = value; } }

    [SerializeField] string text;
    public string Text { get { return text; } set { this.text = value; } }

}