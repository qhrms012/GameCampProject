using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public GameObject[] preFabs;


    List<GameObject>[] pools;

    private void Awake()
    {
        pools = new List<GameObject>[preFabs.Length];

        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(preFabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }

}
