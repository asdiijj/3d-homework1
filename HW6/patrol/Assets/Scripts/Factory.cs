using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{

    private static List<GameObject> used = new List<GameObject>();
    private static List<GameObject> free = new List<GameObject>();

    // create a patrol and put it to destination
    public GameObject setObject(Vector3 destination, Quaternion direction)
    {
        if (free.Count == 0)
        {
            GameObject gameObject = Instantiate(Resources.Load("prefabs/patrol"),
                destination, direction) as GameObject;
            gameObject.AddComponent<PActionManager>();
            used.Add(gameObject);
        }
        else
        {
            used.Add(free[0]);
            free.RemoveAt(0);
            used[used.Count - 1].SetActive(true);
            used[used.Count - 1].transform.position = destination;
            used[used.Count - 1].transform.localRotation = direction;
        }
        return used[used.Count - 1];
    }

    public void freeObject(GameObject obj)
    {
        obj.SetActive(false);
        used.Remove(obj);
        free.Add(obj);
    }

    public void Clear()
    {
        while (used.Count != 0)
        {
            freeObject(used[0]);
        }
    }
}
