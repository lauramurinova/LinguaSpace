using Oculus.Interaction;
using UnityEngine;

public class RayInteractorVisual : MonoBehaviour
{
     [SerializeField] private RayInteractor _rayInteractor;
     [SerializeField] private LineRenderer _lineRenderer;

        protected virtual void Start()
        {
            this.AssertField(_rayInteractor, nameof(_rayInteractor));
        }

        private void LateUpdate()
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, _rayInteractor.Origin);
            _lineRenderer.SetPosition(1, _rayInteractor.End);
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
