using UnityEngine;

public class DragAndDropObject : MonoBehaviour
{
    private Rigidbody _rb;
    private Camera _mainCamera;
    private float _cameraDistance;
    private bool _isDragging = false;

    void Start()
    {
        //Кэшируем компоненты для производительности
        _rb = GetComponent<Rigidbody>();
        Debug.Log("GetRB!");
        _mainCamera = Camera.main;

        //Настраиваем физическое поведение при старте
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; //Для точного обнаружения столкновений быстрых объектов
        _rb.interpolation = RigidbodyInterpolation.Interpolate; //Для плавности движения
    }

    void OnMouseDown()
    {
        //Рассчитываем дистанцию до объекта для корректного позиционирования
        _cameraDistance = Vector3.Distance(transform.position, _mainCamera.transform.position);
        Debug.Log("OnMouseDown");
        //Включаем кинематический режим, чтобы физика не мешала перетаскиванию
        _rb.isKinematic = true;
        _isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!_isDragging) return;
        Debug.Log("OnMouseDrag");
        //Создаем луч от камеры через позицию курсора и вычисляем новую позицию объекта
        Ray cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = cameraRay.GetPoint(_cameraDistance);

        //Плавно перемещаем объект
        _rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15));
    }

    void OnMouseUp()
    {
        if (!_isDragging) return;

        _isDragging = false;
        //Возвращаем физику, бросая объект
        _rb.isKinematic = false;

    }
}