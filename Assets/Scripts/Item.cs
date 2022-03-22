using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    int i = 0;
    public string itemName;
    GameObject collided,particleEffect;
    bool splitItem = false, throwItem = false,pushItem = false,explode = false;
    float speed;
    private void OnCollisionEnter(Collision collision)
    {
        i = 0;
        if (collision.transform.CompareTag("Ground"))
        {
            ItemManager.instance.throwItem = false;
            Destroy(gameObject);
        }
        if (collision.transform.CompareTag("Obstacle"))
        {
            if (collision.collider.transform.GetComponent<Obstacle>() != null)
            {
                if (collision.collider.transform.GetComponent<Obstacle>().obstacleType == "Breakable")
                {
                    collision.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                    if (transform.GetChild(0).transform.GetComponent<MeshCollider>()!=null)
                    transform.GetChild(0).transform.GetComponent<MeshCollider>().enabled = false;
                    Invoke("AfterDelay", 1f);
                    collided = collision.gameObject;
                    collided.GetComponent<BoxCollider>().enabled = false;
                    for (int i = 0; i < collided.transform.childCount; i++)
                    {
                        if (collided.transform.GetChild(i).transform.GetComponent<ParticleSystem>() == null && collided.transform.GetChild(i).transform.GetComponent<Rigidbody>() == null)
                            collided.transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
                    }
                    if (itemName == "axe" || itemName == "knife")
                    {
                        if(collided.gameObject.GetComponentInChildren<ParticleSystem>()!=null)
                        collided.gameObject.GetComponentInChildren<ParticleSystem>().Play();
                        //GameObject particle = Instantiate(particleEffect);
                        //particle.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y, -collision.transform.position.z/2);
                        ItemManager.instance.throwItem = false;
                        transform.GetChild(0).transform.GetComponent<MeshRenderer>().enabled = false;
                        speed = 0.4f;
                    }
                    else
                    {
                        ItemManager.instance.shoot = false;
                        speed = 1.5f;
                    }
                    StartCoroutine(SplitItem());
                    splitItem = true;

                }
                else
                {
                    if (itemName == "axe" || itemName == "knife")
                    {
                        MeshRenderer[] allMeshes = transform.GetComponentsInChildren<MeshRenderer>();
                        for (int i = 0; i < allMeshes.Length; i++)
                        {
                            allMeshes[i].enabled = true;
                            allMeshes[i].transform.gameObject.AddComponent<Rigidbody>();
                        }
                    }
                    else
                    {
                        collided = collision.gameObject;
                        collision.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                        if (transform.GetComponent<MeshCollider>() != null)
                            transform.GetComponent<MeshCollider>().enabled = false;
                        Invoke("AfterDelay", 1.5f);
                        for (int i = 0; i < collision.collider.transform.childCount; i++)
                        {
                            collision.collider.transform.GetChild(i).GetComponent<Animator>().SetBool("scaleDown", true);
                            collision.collider.transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
                            collision.collider.transform.GetChild(i).gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * 500, ForceMode.Acceleration);
                            explode = true;
                        }

                    }
                }
            }
        }

    }
    IEnumerator SplitItem()
    {
        if (splitItem)
        {
            if (collided.transform.GetChild(i).transform.GetComponent<Rigidbody>() != null)
            {
                collided.transform.GetChild(i).transform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right * speed, ForceMode.Impulse);
                collided.transform.GetChild(i + 1).transform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left * speed, ForceMode.Impulse);
                collided.transform.GetChild(i).transform.gameObject.GetComponent<Rigidbody>().velocity *= 0.9f;
                collided.transform.GetChild(i + 1).transform.gameObject.GetComponent<Rigidbody>().velocity *= 0.9f;
                yield return new WaitForSeconds(0.1f);
                if (i < collided.transform.childCount - 2)
                {
                    i += 2;
                }
            }
        }
        if(explode)
        {
            if (collided.transform.GetChild(i).transform.GetComponent<Rigidbody>() != null)
            {
                collided.transform.GetChild(i).transform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * speed, ForceMode.Impulse);
                yield return new WaitForSeconds(0.1f);
                if (i < collided.transform.childCount - 2)
                {
                    i++;
                }
            }
        }
    }
    void Update()
    {
        if (splitItem || explode) StartCoroutine(SplitItem());
    }
    void AfterDelay()
    {
        splitItem = false;
        Destroy(gameObject);
    }
}