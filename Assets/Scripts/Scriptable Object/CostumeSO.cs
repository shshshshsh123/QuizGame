using UnityEngine;

[CreateAssetMenu(fileName = "New Costume", menuName = "ScriptableObjects/CostumeSO")]
public class CostumeSO : ScriptableObject
{
    public int costumeID;
    public string costumeName;
    public CostumeType costumeType;
    public Sprite headSprite;
    public bool isUnlocked;
}

public enum CostumeType
{
    Head,
    Body,
    Legs
}