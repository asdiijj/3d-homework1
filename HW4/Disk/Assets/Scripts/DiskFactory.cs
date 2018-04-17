

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这个类用来生产飞碟，游戏场景不需要知道飞碟生产细节，只要找工厂拿就可以了
public class DiskFactory : MonoBehaviour
{

    private GameObject diskPrefab;

    private List<DiskData> free = new List<DiskData>(); // 未使用的飞碟列表
    private List<DiskData> used = new List<DiskData>(); // 已使用的飞碟列表

    private void Awake()
    {

        // 加载飞碟的预制
        diskPrefab = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/disk"), Vector3.zero, Quaternion.identity);
        diskPrefab.SetActive(false);
    }

    // 从工厂得到飞碟的方法
    public GameObject GetDisk()
    {
        // 工厂要生产并送出的飞碟
        GameObject disk = null;

        // 若列表中有未使用的飞碟，送出飞碟的gameObject
        if (free.Count > 0)
        {
            disk = free[0].gameObject;
            free.Remove(free[0]);
        }
        // 否则新建一个disk
        else
        {
            disk = GameObject.Instantiate<GameObject>(diskPrefab, Vector3.zero, Quaternion.identity);
            disk.AddComponent<DiskData>();  // 加入数据组件
        }

        // 加工disk，随机生成disk的各项属性信息
        float point = UnityEngine.Random.Range(0, 6f);

        // easy mode
        if (point > 3f)
        {
            disk.GetComponent<DiskData>().color = Color.blue;
            disk.GetComponent<Renderer>().material.color = Color.blue;
            disk.GetComponent<DiskData>().speed = 4.0f;
            disk.GetComponent<DiskData>().size = 1.5f;
        }
        // medium mode
        else if (point > 1f && point <= 3f)
        {
            disk.GetComponent<DiskData>().color = Color.red;
            disk.GetComponent<Renderer>().material.color = Color.red;
            disk.GetComponent<DiskData>().speed = 6.0f;
            disk.GetComponent<DiskData>().size = 1f;
        }
        // difficult mode
        else
        {
            disk.GetComponent<DiskData>().color = Color.black;
            disk.GetComponent<Renderer>().material.color = Color.black;
            disk.GetComponent<DiskData>().speed = 8.0f;
            disk.GetComponent<DiskData>().size = 0.5f;
        }
        point = UnityEngine.Random.Range(-1f, 1f) < 0 ? -1 : 1;
        disk.GetComponent<DiskData>().direction = new Vector3(point, 1, 0);
        Vector3 scale = disk.gameObject.transform.localScale;
        scale.x *= disk.GetComponent<DiskData>().size;
        scale.y *= disk.GetComponent<DiskData>().size;
        scale.z *= disk.GetComponent<DiskData>().size;
        disk.gameObject.transform.localScale = scale;

        // 出产disk
        used.Add(disk.GetComponent<DiskData>());
        return disk;
    }

    // 回收飞碟，将用过的飞碟改成未使用状态
    public void FreeDisk(GameObject disk)
    {
        for (int i = 0; i < used.Count; ++i)
        {
            if (disk.GetInstanceID() == used[i].gameObject.GetInstanceID())
            {
                used[i].gameObject.SetActive(false);
                free.Add(used[i]);
                used.Remove(used[i]);
                return;
            }
        }
    }
}
