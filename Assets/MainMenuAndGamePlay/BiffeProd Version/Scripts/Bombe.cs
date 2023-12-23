using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class Bombe : MonoBehaviour
{
    public GameObject explosion;
    public float explosionRadius = 0.5f;
    public LayerMask CharacterLayerMask;
    public float throwEnnemyForce = 3f;
    public CharacterController thrower;
    PhotonView PV;
    private GameObject expClone;
    private GameObject SFXClone;
    bool IsExploded = false;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (thrower != null)
        {
            int id = thrower.GetComponent<PhotonView>().ViewID;
            PV.RPC("updateThrowerToAllPlayer", RpcTarget.All, id);
        }
    }
    [PunRPC]
    public void updateThrowerToAllPlayer(int id)
    {
        thrower = PhotonView.Find(id).GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Timer.IsFinished)
            return;

        transform.LookAt(Camera.main.transform.position);
    }
    public void callCoroutine(float travelTime)
    {
        Invoke("ExplodeBomb", travelTime);
        //PV.RPC("ExplodeBomb", RpcTarget.All, travelTime);
    }

    //[PunRPC]
    //public IEnumerator ExplodeBomb(float travelTime)
    //{
    //    yield return new WaitForSeconds(travelTime);



    //    PhotonNetwork.Destroy(this.gameObject);
    //    //Destroy(this.gameObject);

    //    //yield return new WaitForSeconds(1f);
    //    //PhotonNetwork.Destroy(expClone);
    //    ////Destroy(expClone);

    //    //yield return new WaitForSeconds(1f);
    //    //PhotonNetwork.Destroy(SFXClone);
    //}

    void ExplodeBomb()
    {
        PV.RPC("ExplodeBombRPC", RpcTarget.All);
    }
    [PunRPC]
    public void ExplodeBombRPC(PhotonMessageInfo info)
    {
        if (PV.IsMine && !IsExploded)
        {
            expClone = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BombEffect"), this.transform.position, Quaternion.identity);
            SFXClone = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BombSFX"), this.transform.position, Quaternion.identity);
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, explosionRadius, CharacterLayerMask);
            foreach (Collider collider in colliders)
            {
                CharacterController character = collider.gameObject.GetComponent<CharacterController>();
                if (character.hasBall && character != thrower)
                {
                    thrower.catchBall();
                    character.lostBall();

                    collider.gameObject.GetComponent<Animator>().SetTrigger("death");
                }
            }

            Invoke("DestroyEffects", 1f);
            Invoke("DestroyBomb", 2f);
            IsExploded = true;
        }

        // Hide bomb
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void DestroyEffects()
    {
        PhotonNetwork.Destroy(expClone);
    }

    void DestroyBomb()
    {
        PhotonNetwork.Destroy(SFXClone);
        PhotonNetwork.Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, explosionRadius);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (other.CompareTag("Player") && other.GetComponent<CharacterController>() != thrower)
        {
            callCoroutine(0f);
        }
        if (other.CompareTag("PlaygroundBoundaries"))
        {
            callCoroutine(0f);
        }
    }
}
