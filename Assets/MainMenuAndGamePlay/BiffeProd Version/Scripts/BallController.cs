using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;
public class BallController : MonoBehaviour
{
    public static GameObject Ball;
    private Rigidbody rb;
    public float throwSlowdownFactor = 0.8f;
    public LayerMask groundLayer;
    public GameObject ballIndicator;
    public bool IsBallIndicator = false;
    public Color ballIndicatorCol;
    public float RayLength = 0.12f;
    public Transform prevBallHolder;
    public Transform currBallHolder;
    [HideInInspector] public bool ballIsGrounded = true;
    PhotonView PV;
    public Transform superCatcher;
    bool IsThrow;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        Ball = this.gameObject;
    }
    void Start()
    {
        ballIndicator = this.gameObject.transform.GetChild(0).gameObject;
        ballIndicator.transform.parent = null;
        rb = GetComponent<Rigidbody>();
        camera_follow.target = this.transform;
    }
    private void OnDisable()
    {
        ballIndicator.SetActive(false);
    }
    void FixedUpdate()
    {
        ballIndicator.SetActive(true);
        if(!Timer.IsFinished)
            PV.RPC("ballInfos", RpcTarget.All);
    }
    [PunRPC]
    void ballInfos()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out hit, 50f, groundLayer))
        {
            ballIndicator.GetComponent<MeshRenderer>().sharedMaterial.color = ballIndicatorCol;
            Vector3 hitPos = hit.point;
            ballIndicator.transform.position = hitPos;
            ballIndicator.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        if (Physics.Raycast(ray, RayLength))
        {
            ballIsGrounded = true;
            if (rb != null)
            {
                if (rb.velocity.magnitude > 0)
                {
                    rb.velocity *= 1 - (throwSlowdownFactor * Time.deltaTime);
                }
            }
        }
        else
        {
            ballIsGrounded = false;
        }
    }
    public void callCoroutine(float travelTime)
    {
        PV.RPC("landBall", RpcTarget.All, travelTime);
    }
    [PunRPC]
    public IEnumerator landBall(float travelTime)
    {
        IsThrow = true;
        yield return new WaitForSeconds(travelTime);
        rb.useGravity = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (!ballIsGrounded && other.CompareTag("Player"))
        {
            superCatcher = other.transform;
            if (prevBallHolder != null)
            {
                if (superCatcher.gameObject.layer == prevBallHolder.gameObject.layer && IsThrow)
                {
                    if (superCatcher.GetComponent<CharacterController>().XPos > 0)
                    {
                        UImanager.instance.updateBlueTeamScore();
                    }
                    else if (superCatcher.GetComponent<CharacterController>().XPos < 0)
                    {
                        UImanager.instance.updateRedTeamScore();
                    }
                    PV.RPC("ReturnThrow", RpcTarget.All);
                }
            }
            superCatcher = null;
        }
    }

    [PunRPC]
    void ReturnThrow()
    {
        IsThrow = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlaygroundBoundaries")
        {
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.position = new Vector3(0, 0.32258f, 0);
            transform.eulerAngles = Vector3.zero;
        }
    }
} 
