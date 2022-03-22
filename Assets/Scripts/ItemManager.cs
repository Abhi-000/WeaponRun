using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemManager : MonoBehaviour
{
    public int[] itemCount;
    public static ItemManager instance;
    [HideInInspector]public bool throwItem = false,shoot = false;
    private GameObject currItem,newItem;
    string[] allItems = { "knife", "axe", "bazooka" };
    int itemNum;
    public Button[] itemBtns;
    public TextMeshProUGUI[] itemCountsText;
    private void Start()
    {
        for(int i=0;i< itemCountsText.Length;i++)
        {
            itemCountsText[i].text =itemCount[i].ToString();
        }
    }
    void UpdateItemCount(int itemNo)
    {
        itemCount[itemNo]--;
        for (int i = 0; i < itemCountsText.Length; i++)
        {
            if (itemCount[i] >= 0)
            {
                if (itemCount[i] == 0)
                {
                    itemBtns[i].interactable = false;
                }
                itemCountsText[i].text =itemCount[i].ToString();
            }
        }
    }
    private void Awake()
    {
        instance = this;
    }
    public void EquipItem(int itemNo)
    {
        UpdateItemCount(itemNo);
        itemNum = itemNo;
        if (itemNo == 0 || itemNo == 1)
        {
            Item[] allItemsInChild = transform.GetComponentsInChildren<Item>();
            for (int i = 0; i < allItemsInChild.Length; i++)
            {
                Debug.Log(i);
                if (allItemsInChild[i].itemName == allItems[itemNo])
                {
                    allItemsInChild[i].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                    currItem = allItemsInChild[i].gameObject;
                }
            }
            PlayerController.instance.startRunning = false;
            transform.GetComponent<Animator>().SetBool("throw", true);
            transform.GetComponent<Animator>().SetBool("run", false);
        }
        else
        {
            Item[] allItemsInChild = transform.GetComponentsInChildren<Item>();
            for (int i = 0; i < allItemsInChild.Length; i++)
            {
                Debug.Log(i);
                if (allItemsInChild[i].itemName == allItems[itemNo])
                {
                    for (int j = 1; j < allItemsInChild[i].transform.childCount; j++)
                    {
                        allItemsInChild[i].transform.GetChild(j).GetComponent<MeshRenderer>().enabled = true;
                        currItem = allItemsInChild[i].gameObject;
                    }
                }
            }
            PlayerController.instance.startRunning = false;
            transform.GetComponent<Animator>().SetBool("shoot", true);
            transform.GetComponent<Animator>().SetBool("run", false);
        }
    }
    public void ThrowItem()
    {
        if (itemNum == 0 || itemNum == 1)
        {
            TrailRenderer[] allTrails;
            allTrails = currItem.transform.GetComponentsInChildren<TrailRenderer>();
            for(int i=0;i<allTrails.Length;i++)
            {
                allTrails[i].enabled = true;
            }
            newItem = Instantiate(currItem, currItem.transform.parent);
            allTrails = currItem.transform.GetComponentsInChildren<TrailRenderer>();
            for (int i = 0; i < allTrails.Length; i++)
            {
                allTrails[i].enabled = false;
            }
            newItem.GetComponent<Animation>().Play();
            newItem.transform.parent = null;
            currItem.transform.GetChild(0).transform.GetComponent<MeshRenderer>().enabled = false;
            newItem.AddComponent<Rigidbody>();
            throwItem = true;
            Invoke("AfterDelay", 2f);
        }
        else
        {
            newItem = Instantiate(currItem.transform.GetChild(0).gameObject, currItem.transform.parent);
            newItem.transform.GetComponent<MeshRenderer>().enabled = true;
            newItem.transform.parent = null;
            newItem.transform.position = new Vector3(0.23f, -11.2f, newItem.transform.position.z);
            newItem.transform.rotation = new Quaternion(0,0,0,0);
            newItem.gameObject.AddComponent<Rigidbody>();
            newItem.gameObject.GetComponentInChildren<ParticleSystem>().Play();
            //ex.SetActive(true);
            shoot = true;
        }
    }
    void AfterDelay()
    {
        PlayerController.instance.startRunning = true;
        transform.GetComponent<Animator>().SetBool("run", true);
        transform.GetComponent<Animator>().SetBool("throw", false);
    }
    public void UnequpItem()
    {
        MeshRenderer[] meshInChild =  currItem.transform.GetComponentsInChildren<MeshRenderer>();
        for(int i=0;i<meshInChild.Length;i++)
        {
            meshInChild[i].transform.GetComponent<MeshRenderer>().enabled = false;
        }
        PlayerController.instance.startRunning = true;
        transform.GetComponent<Animator>().SetBool("run", true);
        transform.GetComponent<Animator>().SetBool("shoot", false);
    }
    private void Update()
    {
        if (throwItem)
        {
            newItem.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
            newItem.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
            newItem.GetComponent<Rigidbody>().AddForce(Vector3.forward * 120);
            newItem.GetComponent<Rigidbody>().velocity = newItem.GetComponent<Rigidbody>().velocity * 0.9f;
        }
        else if(shoot)
        {
            if (newItem != null)
            {
                newItem.transform.GetComponent<Rigidbody>().constraints= RigidbodyConstraints.FreezeRotation;
                newItem.transform.GetComponent<Rigidbody>().AddForce(Vector3.forward * 90);
                newItem.GetComponent<Rigidbody>().velocity = newItem.GetComponent<Rigidbody>().velocity * 0.9f;
            }
        }
    }
}
