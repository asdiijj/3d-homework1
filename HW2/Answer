## 简答题

- > 游戏对象运动的本质是什么？

    运动的本质，就是游戏对象（Object）位置坐标的变换。

---

- > 请用三种方法以上方法，实现物体的抛物线运动。

    <pre>
    <code>
    // 方法一: use position
    void Update () {
	   this.transform.position += Vector3.left * Time.deltaTime;
	   this.transform.position += Vector3.up * Time.deltaTime * Time.deltaTime;
    }

    // 方法二：use translate
    void Update () {
        this.transform.Translate(speed * Vector3.left * Time.deltaTime);
        this.transform.Translate(gravity * Time.deltaTime * Time.deltaTime / 2 * Vector3.down);
    }

    // 方法三：use rigidbody
    void Start() {
        Rigidbody rb;
        rb = this.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 0);
    }
    </code>
    </pre>

---

- > 写一个程序，实现一个完整的太阳系。

    详细代码可看solar文件夹;

    太阳会自发光，且固定位置不动；

    其它八大行星以不同的轨道和速度围绕其公转，同时又以不同的速度自转；

    具体实现示例如图：

    ![](/Assets/solar_pic.png)


## 编程实践-牧师与恶魔

- > 列出游戏中的事物（Objects）。

    牧师Priest 恶魔Devil 河流River 河岸Coast 按钮 标签

---

- > 用表格列出玩家动作表（规则表）

| 对象 | 动作 | 条件 |
| :-: | :-: | :-: |
| 人物 | 上船 | 船上有空位，且人物与船同侧 |
| 人物 | 上岸 | 有最少一个人物在船上 |
| 船 | 移动 | 船上有人，船移动到对岸 |
| 胜利 | GUI | 6个人物均在左岸 |
| 失败 | GUI | 某一边的恶魔多于牧师 |

---
**详细代码请在 Priest-Devil 文件夹中浏览；**

**双击 Priest-Devil/Assets/start 即可开始游戏。**
