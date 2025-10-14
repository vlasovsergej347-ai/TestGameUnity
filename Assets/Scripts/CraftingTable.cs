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
        //���������, ��� ������ ����� ��������� DragAndDropObject
        DragAndDropObject item = other.GetComponent<DragAndDropObject>();
        if (item != null && !_currentItems.Contains(item))
        {
            _currentItems.Add(item);
            Debug.Log($"������� � ������ {other} �� �����");

            // �������� �������������� ��������, ����� �� >= 2
            if (_currentItems.Count >= 2)
            {
                TryCombineItemsOnTable();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //������� ������� �� ������ ��� ������ �� ��������
        DragAndDropObject item = other.GetComponent<DragAndDropObject>();
        if (item != null && _currentItems.Contains(item))
        {
            _currentItems.Remove(item);
            Debug.Log($"������� � ������ {other} ����� �� �����");
        }
    }

    private void TryCombineItemsOnTable()
    {
        if (_currentItems.Count < 2) return;

        //����������� HashSet � List ��� �������� ������� �� �������
        List<DragAndDropObject> itemsList = new List<DragAndDropObject>(_currentItems);

        //����� ������ ��� �������� �� �����
        string item1Name = itemsList[0].gameObject.name.Replace("(Clone)", "").Trim();
        string item2Name = itemsList[1].gameObject.name.Replace("(Clone)", "").Trim();

        Debug.Log($"�������� �������������� {item1Name} + {item2Name}");

        //���������� ����� �� CraftingManager
        bool success = _craftingManager.TryCombineItems(item1Name, item2Name, itemsList[0].transform);

        if (success)
        {
            //���� ���������� �������, ���������� �������� ��������
            foreach (DragAndDropObject item in _currentItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            _currentItems.Clear();
        }
    }
}