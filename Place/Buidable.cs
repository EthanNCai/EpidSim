using UnityEngine;

[System.Serializable]
public class BuildableInfo
{
    public string name;
    public string description;
    // 如果你有图标什么的，也可以加上去～
    public Sprite icon;
     public BuildableInfo(string name, string description, Sprite icon)
    {
        this.name = name;
        this.description = description;
        this.icon = icon;
    }
}
