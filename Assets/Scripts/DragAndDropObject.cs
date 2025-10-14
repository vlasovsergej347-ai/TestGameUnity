using UnityEngine;

public class DragAndDropObject : MonoBehaviour
{
    private Rigidbody _rb;
    private Camera _mainCamera;
    private float _cameraDistance;
    private bool _isDragging = false;

    void Start()
    {
        //�������� ���������� ��� ������������������
        _rb = GetComponent<Rigidbody>();
        Debug.Log("GetRB!");
        _mainCamera = Camera.main;

        //����������� ���������� ��������� ��� ������
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; //��� ������� ����������� ������������ ������� ��������
        _rb.interpolation = RigidbodyInterpolation.Interpolate; //��� ��������� ��������
    }

    void OnMouseDown()
    {
        //������������ ��������� �� ������� ��� ����������� ����������������
        _cameraDistance = Vector3.Distance(transform.position, _mainCamera.transform.position);
        Debug.Log("OnMouseDown");
        //�������� �������������� �����, ����� ������ �� ������ ��������������
        _rb.isKinematic = true;
        _isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!_isDragging) return;
        Debug.Log("OnMouseDrag");
        //������� ��� �� ������ ����� ������� ������� � ��������� ����� ������� �������
        Ray cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = cameraRay.GetPoint(_cameraDistance);

        //������ ���������� ������
        _rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15));
    }

    void OnMouseUp()
    {
        if (!_isDragging) return;

        _isDragging = false;
        //���������� ������, ������ ������
        _rb.isKinematic = false;

    }
}