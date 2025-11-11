using UnityEngine;

[CreateAssetMenu(fileName = "New Costume", menuName = "ScriptableObjects/CostumeSO")]
public class CostumeSO : ScriptableObject
{
    public int costumeID;
    public string costumeName;
    public CostumeType costumeType;
    public Sprite costumeSprite;
    public bool isUnlocked;

    public void UnlockCostume()
    {
        isUnlocked = true;
    }
}

public enum CostumeType
{
    Head,
    Body,
    Legs
}