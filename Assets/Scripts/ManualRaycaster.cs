using UnityEngine;

public class ManualRaycaster : MonoBehaviour
{
    public Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //��������� ��������� � ������ hit.collider.gameObject
                Debug.Log($"�� ������ � {hit}");
            }
        }
    }
}
