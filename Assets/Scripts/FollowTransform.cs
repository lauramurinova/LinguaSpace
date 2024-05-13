using Oculus.Interaction;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private RayInteractor _rayInteractor;
    
    private void FixedUpdate()
    {
        if(!_rayInteractor) return;
        
        transform.SetPositionAndRotation(_rayInteractor.Origin, _rayInteractor.Rotation);
    }
}
