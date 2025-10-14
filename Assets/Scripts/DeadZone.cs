using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        Debug.Log($"Предмет с именем {other} уничтожен");
    }
}
