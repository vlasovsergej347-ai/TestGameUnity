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
    public GameObject buttonPrefab; //Префаб кнопки

    private HashSet<string> unlockedItems = new HashSet<string>(); //Хранит предметы
    private Dictionary<string, Button> itemButtons = new Dictionary<string, Button>(); //Связывает предмет с кнопкой

    private string unlockSaveKey = "UnlockedItems"; //Ключ для сохранения в pp
    private void Awake()
    {
        LoadAllRecipes();
        CacheAllPrefabs();
        //UnlockItem("Water");
        //UnlockItem("Apple");
        LoadUnlockedItems(); //Загружаем прогресс при старте игры
    }

    private void LoadAllRecipes()
    {
        allRecipes = Resources.LoadAll<CraftingRecipe>(recipesFolderPath).ToList();
        Debug.Log($"Зарегестрировано {allRecipes.Count} рецептов");
    }

    private void CacheAllPrefabs()
    {
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>(prefabsFolderPath);
        prefabCache = new Dictionary<string, GameObject>();

        foreach (GameObject prefab in loadedPrefabs)
        {
            prefabCache[prefab.name] = prefab;
        }
        Debug.Log($"Зарегестрировано {prefabCache.Count} префабов");
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
            Debug.LogWarning($"Предмета с именем {itemName} не существует");
        }
    }
    public void CreateItemFromButton(string itemName)
    {
        if (prefabCache.TryGetValue(itemName, out GameObject prefab))
        {
            Instantiate(prefab, SpawnPos.transform.position, Quaternion.identity);
            Debug.Log($"Создали {itemName}");
        }
        else
        {
            Debug.LogWarning($"Предмета с именем {itemName} не существует");
        }
    }
    public void UnlockItem(string itemName)
    {
        if (!unlockedItems.Contains(itemName) && prefabCache.ContainsKey(itemName))
        {
            unlockedItems.Add(itemName);
            CreateItemButton(itemName);
            SaveUnlockedItems(); //Сохраняем после каждого нового открытия
        }
    }

    //Метод для сохранения списка открытых предметов
    private void SaveUnlockedItems()
    {
        string saveData = string.Join(";", unlockedItems);
        PlayerPrefs.SetString(unlockSaveKey, saveData);
        PlayerPrefs.Save(); //сохранение
        Debug.Log("Прогресс сохранён " + saveData);
    }

    //Автосейв
    private void OnApplicationQuit()
    {
        SaveUnlockedItems();
    }
    private void CreateItemButton(string itemName)
    {
        if (buttonPrefab == null || buttonListParent == null)
        {
            Debug.LogError("Не назначен buttonPrefab или buttonListParent");
            return;
        }

        //Создаем новую кнопку
        GameObject newButtonObj = Instantiate(buttonPrefab, buttonListParent);
        Button newButton = newButtonObj.GetComponent<Button>();
        Text buttonText = newButtonObj.GetComponentInChildren<Text>();

        if (buttonText != null)
        {
            buttonText.text = itemName; //Устанавливаем имя предмета на кнопку
        }

        //Регистрируем обработчик события нажатия на кнопку с помощью лямбда-выражения
        newButton.onClick.AddListener(() => CreateItemFromButton(itemName));

        //Сохраняем кнопку в словарь
        itemButtons[itemName] = newButton;
    }
    public bool TryCombineItems(string item1, string item2, Transform Spawnpoint)
    {
        if (string.IsNullOrEmpty(item1) || string.IsNullOrEmpty(item2))
            return false;

        //Очищаем имена от постфиксов и лишних пробелов
        string cleanItem1 = CleanItemName(item1);
        string cleanItem2 = CleanItemName(item2);

        CraftingRecipe recipe = allRecipes.FirstOrDefault(r =>
            (CleanItemName(r.ingredient1) == cleanItem1 && CleanItemName(r.ingredient2) == cleanItem2) ||
            (CleanItemName(r.ingredient1) == cleanItem2 && CleanItemName(r.ingredient2) == cleanItem1)
        );

        if (recipe != null && !string.IsNullOrEmpty(recipe.result))
        {
            Debug.Log($"Комбинация успешна, создано {recipe.result}");
            CreateItem(recipe.result, Spawnpoint);
            UnlockItem(recipe.result);
            return true;
        }

        Debug.Log("Не найдено подходящего рецепта для этих ингредиентов");
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
                Debug.Log("Прогресс загружен. Открыто предметов: " + unlockedItems.Count);
            }
        }
        else
        {
            //Если сохранений нет, добавляем базовые элементы
            UnlockItem("Water");
            UnlockItem("Apple");
            Debug.Log("Созданы начальные предметы.");
        }
    }
    public void ClearAllProgress()
    {
        unlockedItems.Clear();
        PlayerPrefs.DeleteKey(unlockSaveKey);

        //Очищаем существующие кнопки
        foreach (var button in itemButtons.Values)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        itemButtons.Clear();

        //Заново добавляем базовые предметы
        UnlockItem("Water");
        UnlockItem("Apple");
        Debug.Log("Весь прогресс сброшен.");
    }

    //Метод для отладки принудительная разблокировка всех рецептов
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