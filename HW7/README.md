### README

- 作业1：简单粒子制作，按要求完成一个粒子系统并且用脚本代码控制。

- 下载 Assets 文件夹覆盖到unity3d项目，打开MyScene

- [视频连接](http://v.youku.com/v_show/id_XMzYzMzU1OTcwOA==.html?spm=a2h3j.8428770.3416059.1)

---

### 实验报告

1. 按照要求制作光球，过程无需赘述，可查看源文件或展示视频；

2. 代码控制脚本文件为 prefabs/ring.cs ，作用是令光球和一个光环Ring不断循环换色：

```CSharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ring : MonoBehaviour {

    public ParticleSystem particleSystem;
    private int count;

	// Use this for initialization
	void Start () {
        particleSystem = gameObject.GetComponent<ParticleSystem>();
        count = 0;
	}

	// Update is called once per frame
	void Update () {
        if (particleSystem.isPlaying)
        {
            return;
        }
        switch(count)
        {
            case 0:
                particleSystem.startColor = Color.red;
                break;
            case 1:
                particleSystem.startColor = Color.blue;
                break;
            case 2:
                particleSystem.startColor = Color.yellow;
                break;
            case 3:
                particleSystem.startColor = Color.white;
                break;
            default:
                break;
        }
        particleSystem.Play();
        count++;
        if (count == 4)
            count = 0;
	}
}

```
