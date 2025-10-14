using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "UObject/Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string ingredient1;
    public string ingredient2;
    public string result;
}
