using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    private CraftingManager _craftingManager;
    private HashSet<DragAndDropObject> _currentItems = new HashSet<DragAndDropObject>();

    private void Start()
    {
        _craftingManager = FindObjectOfType<CraftingManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Проверяем, что объект имеет компонент DragAndDropObject
        DragAndDropObject item = other.GetComponent<DragAndDropObject>();
        if (item != null && !_currentItems.Contains(item))
        {
            _currentItems.Add(item);
            Debug.Log($"Предмет с именем {other} на столе");

            // Пытаемся скомбинировать предметы, когда их >= 2
            if (_currentItems.Count >= 2)
            {
                TryCombineItemsOnTable();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Убираем предмет из списка при выходе из триггера
        DragAndDropObject item = other.GetComponent<DragAndDropObject>();
        if (item != null && _currentItems.Contains(item))
        {
            _currentItems.Remove(item);
            Debug.Log($"Предмет с именем {other} убран со стола");
        }
    }

    private void TryCombineItemsOnTable()
    {
        if (_currentItems.Count < 2) return;

        //Преобразуем HashSet в List для удобства доступа по индексу
        List<DragAndDropObject> itemsList = new List<DragAndDropObject>(_currentItems);

        //Берем первые два предмета на столе
        string item1Name = itemsList[0].gameObject.name.Replace("(Clone)", "").Trim();
        string item2Name = itemsList[1].gameObject.name.Replace("(Clone)", "").Trim();

        Debug.Log($"Пытаемся скомбинировать {item1Name} + {item2Name}");

        //Используем метод из CraftingManager
        bool success = _craftingManager.TryCombineItems(item1Name, item2Name, itemsList[0].transform);

        if (success)
        {
            //Если комбинация успешна, уничтожаем исходные предметы
            foreach (DragAndDropObject item in _currentItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            _currentItems.Clear();
        }
    }
}