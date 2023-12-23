using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_follow : MonoBehaviour
{
    public static Transform target;

    public bool isCustomOffset;
    public Vector3 offset;

    public float smoothSpeed = 0.1f;

    private void Start()
    {
        // You can also specify your own offset from inspector
        // by making isCustomOffset bool to true
        if (!isCustomOffset)
        {
            offset = transform.position - target.position;
        }
    }

    private void LateUpdate()
    {
        SmoothFollow();
    }

    public void SmoothFollow()
    {
        if (target == null)
            return;

        Vector3 TargetPos = new Vector3(target.position.x, 0, 0);
        TargetPos += offset;
        transform.position = Vector3.Lerp(transform.position, TargetPos, smoothSpeed);
    }

    //public void SmoothFollow()
    //{
    //    Vector3 targetPos = target.position + offset;
    //    Vector3 smoothFollow = Vector3.Lerp(transform.position,
    //    targetPos, smoothSpeed);

    //    transform.position = smoothFollow;
    //    transform.LookAt(target);
    //}
}
