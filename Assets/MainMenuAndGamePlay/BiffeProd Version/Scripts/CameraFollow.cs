using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private Vector3 velTemp = Vector3.zero;
    public float smoothness = 1.2f;

    private void Update()
    {
        Vector3 targetPos = new Vector3(target.position.x, this.transform.position.y, this.transform.position.z);
        Vector3 newPos = Vector3.SmoothDamp(this.transform.position, targetPos, ref velTemp, smoothness);
        transform.position = newPos;
    }
}
