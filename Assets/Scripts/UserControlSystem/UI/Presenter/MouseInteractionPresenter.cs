using System.Linq;
using Abstractions;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UserControlSystem;

public sealed class MouseInteractionPresenter : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private SelectableValue _selectedObject;
    [SerializeField] private EventSystem _eventSystem;
    
    [SerializeField] private Vector3Value _groundClicksRMB;
    [SerializeField] private AttackableValue _attackablesRMB;
    [SerializeField] private Transform _groundTransform;
    
    private Plane _groundPlane;

    private void Start()
    {
        _groundPlane = new Plane(_groundTransform.up, 0);
        var clickStreamLeft = Observable.EveryUpdate().Where(_=>Input.GetMouseButtonDown(0));
        clickStreamLeft.Subscribe(leftClick);
        var clickStreamRight = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(1));
        clickStreamRight.Subscribe(rightClick);
    }

    
    private void leftClick(long obj)
    {
        if (_eventSystem.IsPointerOverGameObject())
        {
            return;
        }
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray);
        if (WeHit<ISelectable>(hits, out var selectable))
        {
            _selectedObject.SetValue=selectable;
        }
        else
        {
            _selectedObject.SetValue=null;
        }
    }
    private void rightClick(long obj)
    {
        if (_eventSystem.IsPointerOverGameObject())
        {
            return;
        }
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray);
        if (WeHit<IAttackable>(hits, out var attackable))
        {
            _attackablesRMB.SetValue=attackable;
        }
        else if (_groundPlane.Raycast(ray, out var enter))
        {
            _groundClicksRMB.SetValue=(ray.origin + ray.direction * enter);
        }
    }

    private bool WeHit<T>(RaycastHit[] hits, out T result) where T : class
    {
        result = default;
        if (hits.Length == 0)
        {
            return false;
        }    
        result = hits
            .Select(hit => hit.collider.GetComponentInParent<T>())
            .FirstOrDefault(c => c != null);
        return result != default;
    }
}