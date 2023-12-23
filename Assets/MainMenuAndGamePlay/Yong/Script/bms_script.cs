using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bms_script : MonoBehaviour
{
    public GameObject chartHand;
    public GameObject chart;
    private bool isNotThrowing = true;
    private bool throwBomb = false;
    private Rigidbody rb;
    public GameObject explosion;
    public float thrownForce = 0.9f;
    public GameObject part;
    
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
    }

    public void throwButton()
    {
        throwBomb = true;
        chart.GetComponent<Animator>().SetBool("throwing", true);
    }

    // Update is called once per frame
    void Update()
    {
        //keep bomb in player hand
        if (isNotThrowing)
        {
            transform.position = new Vector3(chartHand.transform.position.x, chartHand.transform.position.y, chartHand.transform.position.z);
        }

        if (throwBomb)
        {
            StartCoroutine(throwBombFonct());
        }

       
    }
    IEnumerator throwBombFonct()
    {
        transform.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(0.6f);
        part.GetComponent<ParticleSystem>().startSize = 0.1f;
        rb.AddForce(chart.transform.forward * thrownForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.2f);
        isNotThrowing = false;
        throwBomb = false;
        StartCoroutine(ExplodeBomb());
        chart.GetComponent<Animator>().SetBool("throwing", false);
    }


    IEnumerator ExplodeBomb()
    {
        // Wait until the bomb touches the ground
        while (!Physics.Raycast(this.transform.position, -Vector3.up, 0.01f))
        {

            yield return null;
        }


        //Instantiate(explosionPrefab, bomb.transform.position, bomb.transform.rotation);

        /*Collider[] colliders = Physics.OverlapSphere(bomb.transform.position, explosionRadius);
         foreach (Collider collider in colliders)
         {
             Health health = collider.gameObject.GetComponent<Health>();
             if (health != null)
             {
                 health.TakeDamage(explosionDamage);
             }
         }*/

        Debug.Log("not touching");
        Instantiate(explosion, transform.position + new Vector3(0, -0.3f, 0), Quaternion.identity);
        Destroy(this.gameObject);
     }
}
