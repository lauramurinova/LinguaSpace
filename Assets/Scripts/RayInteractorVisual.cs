using Oculus.Interaction;
using UnityEngine;

public class RayInteractorVisual : MonoBehaviour
{
    [SerializeField] private RayInteractor _rayInteractor;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _minDistanceToShowLine = 1f;
    [SerializeField] private float _minDistanceFromMenuToShowLine = 0.35f;
    [SerializeField] private Transform _playerMenu;

    protected virtual void Start()
    {
        this.AssertField(_rayInteractor, nameof(_rayInteractor));
    }

    private void LateUpdate()
    {
        if (_rayInteractor == null || _lineRenderer == null)
            return;

        if ((_rayInteractor.CollisionInfo.HasValue && Vector3.Distance(_rayInteractor.Origin, _rayInteractor.CollisionInfo.Value.Point) < _minDistanceToShowLine))
        {
            _lineRenderer.positionCount = 0;
        }
        else
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, _rayInteractor.Origin);
            _lineRenderer.SetPosition(1, _rayInteractor.End);
        }
    }

    #region Inject

    public void InjectAllRayInteractorDebugGizmos(RayInteractor rayInteractor)
    {
        InjectRayInteractor(rayInteractor);
    }

    public void InjectRayInteractor(RayInteractor rayInteractor)
    {
        _rayInteractor = rayInteractor;
    }

    #endregion
}
