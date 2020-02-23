using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public enum TargetType
    {
        None,
        Player,
        Terrain
    };
    public TargetType targetType;
    public Vector3 offset = Vector3.zero;
    [SerializeField] float smoothTime;

    private void Start()
    {
       
    }
    void Update()
    {
        SwitchTargetToFollow();
    }
    void SwitchTargetToFollow()
    {
        switch(targetType)
        {
            case TargetType.None:
                //free for all;
                break;
            case TargetType.Player:
                Vector3 desiredPosition = target.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothTime);
                transform.position = smoothedPosition;

                transform.LookAt(target);
                break;
            case TargetType.Terrain:
                // will look at the terrain
                break;
        }
    }
}
