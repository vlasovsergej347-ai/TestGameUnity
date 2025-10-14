using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] private string recipesFolderPath;
    [SerializeField] private string prefabsFolderPath;
    [SerializeField] private Transform SpawnPos;
    [SerializeField] private List<CraftingRecipe> allRecipes;
    [SerializeField] private Dictionary<string, GameObject> prefabCache;

    public Transform buttonListParent; //Scrollview
    public GameObject buttonPrefab; //������ ������

    private HashSet<string> unlockedItems = new HashSet<string>(); //������ ��������
    private Dictionary<string, Button> itemButtons = new Dictionary<string, Button>(); //��������� ������� � �������

    private string unlockSaveKey = "UnlockedItems"; //���� ��� ���������� � pp
    private void Awake()
    {
        LoadAllRecipes();
        CacheAllPrefabs();
        //UnlockItem("Water");
        //UnlockItem("Apple");
        LoadUnlockedItems(); //��������� �������� ��� ������ ����
    }

    private void LoadAllRecipes()
    {
        allRecipes = Resources.LoadAll<CraftingRecipe>(recipesFolderPath).ToList();
        Debug.Log($"���������������� {allRecipes.Count} ��������");
    }

    private void CacheAllPrefabs()
    {
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>(prefabsFolderPath);
        prefabCache = new Dictionary<string, GameObject>();

        foreach (GameObject prefab in loadedPrefabs)
        {
            prefabCache[prefab.name] = prefab;
        }
        Debug.Log($"���������������� {prefabCache.Count} ��������");
    }

    public void CreateItem(string itemName, Transform Spawnpoint)
    {
        if (prefabCache.TryGetValue(itemName, out GameObject prefab))
        {
            Instantiate(prefab, Spawnpoint.position, Quaternion.identity);
            Debug.Log($"Created item: {itemName}");
        }
        else
        {
            Debug.LogWarning($"�������� � ������ {itemName} �� ����������");
        }
    }
    public void CreateItemFromButton(string itemName)
    {
        if (prefabCache.TryGetValue(itemName, out GameObject prefab))
        {
            Instantiate(prefab, SpawnPos.transform.position, Quaternion.identity);
            Debug.Log($"������� {itemName}");
        }
        else
        {
            Debug.LogWarning($"�������� � ������ {itemName} �� ����������");
        }
    }
    public void UnlockItem(string itemName)
    {
        if (!unlockedItems.Contains(itemName) && prefabCache.ContainsKey(itemName))
        {
            unlockedItems.Add(itemName);
            CreateItemButton(itemName);
            SaveUnlockedItems(); //��������� ����� ������� ������ ��������
        }
    }

    //����� ��� ���������� ������ �������� ���������
    private void SaveUnlockedItems()
    {
        string saveData = string.Join(";", unlockedItems);
        PlayerPrefs.SetString(unlockSaveKey, saveData);
        PlayerPrefs.Save(); //����������
        Debug.Log("�������� ������� " + saveData);
    }

    //��������
    private void OnApplicationQuit()
    {
        SaveUnlockedItems();
    }
    private void CreateItemButton(string itemName)
    {
        if (buttonPrefab == null || buttonListParent == null)
        {
            Debug.LogError("�� �������� buttonPrefab ��� buttonListParent");
            return;
        }

        //������� ����� ������
        GameObject newButtonObj = Instantiate(buttonPrefab, buttonListParent);
        Button newButton = newButtonObj.GetComponent<Button>();
        Text buttonText = newButtonObj.GetComponentInChildren<Text>();

        if (buttonText != null)
        {
            buttonText.text = itemName; //������������� ��� �������� �� ������
        }

        //������������ ���������� ������� ������� �� ������ � ������� ������-���������
        newButton.onClick.AddListener(() => CreateItemFromButton(itemName));

        //��������� ������ � �������
        itemButtons[itemName] = newButton;
    }
    public bool TryCombineItems(string item1, string item2, Transform Spawnpoint)
    {
        if (string.IsNullOrEmpty(item1) || string.IsNullOrEmpty(item2))
            return false;

        //������� ����� �� ���������� � ������ ��������
        string cleanItem1 = CleanItemName(item1);
        string cleanItem2 = CleanItemName(item2);

        CraftingRecipe recipe = allRecipes.FirstOrDefault(r =>
            (CleanItemName(r.ingredient1) == cleanItem1 && CleanItemName(r.ingredient2) == cleanItem2) ||
            (CleanItemName(r.ingredient1) == cleanItem2 && CleanItemName(r.ingredient2) == cleanItem1)
        );

        if (recipe != null && !string.IsNullOrEmpty(recipe.result))
        {
            Debug.Log($"���������� �������, ������� {recipe.result}");
            CreateItem(recipe.result, Spawnpoint);
            UnlockItem(recipe.result);
            return true;
        }

        Debug.Log("�� ������� ����������� ������� ��� ���� ������������");
        return false;
    }

    private string CleanItemName(string originalName)
    {
        return originalName.Replace("(Clone)", "").Trim();
    }
    private void LoadUnlockedItems()
    {
        if (PlayerPrefs.HasKey(unlockSaveKey))
        {
            string savedData = PlayerPrefs.GetString(unlockSaveKey);
            if (!string.IsNullOrEmpty(savedData))
            {
                string[] unlockedItemNames = savedData.Split(';');

                foreach (string itemName in unlockedItemNames)
                {
                    if (!string.IsNullOrEmpty(itemName))
                    {
                        unlockedItems.Add(itemName);
                        CreateItemButton(itemName);
                    }
                }
                Debug.Log("�������� ��������. ������� ���������: " + unlockedItems.Count);
            }
        }
        else
        {
            //���� ���������� ���, ��������� ������� ��������
            UnlockItem("Water");
            UnlockItem("Apple");
            Debug.Log("������� ��������� ��������.");
        }
    }
    public void ClearAllProgress()
    {
        unlockedItems.Clear();
        PlayerPrefs.DeleteKey(unlockSaveKey);

        //������� ������������ ������
        foreach (var button in itemButtons.Values)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        itemButtons.Clear();

        //������ ��������� ������� ��������
        UnlockItem("Water");
        UnlockItem("Apple");
        Debug.Log("���� �������� �������.");
    }

    //����� ��� ������� �������������� ������������� ���� ��������
    public void UnlockAllRecipesForTest()
    {
        foreach (var recipe in allRecipes)
        {
            if (!string.IsNullOrEmpty(recipe.result))
            {
                UnlockItem(recipe.result);
            }
        }
    }
}