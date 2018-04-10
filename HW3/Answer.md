## 简答题

- > 参考 Fantasy Skybox FREE 构建自己的游戏场景

    商店里找不到免费的 Fantasy Skybox，直接使用群里给的 skybox：

    ![](/HW3/Assets/1.png)

    尝试使用 skybox 时遇到一点问题，从上图可以看到，直接使用所给的资源的话，会出现很明显的边界；

    将每张图片的 WrapMode 设置为 Clamp 即可，如图：

    ![](/HW3/Assets/4.png)

    设置后可以看到边框消失了，真正实现无缝连接：

    ![](/HW3/Assets/2.png)

    配合游戏对象使用：

    ![](/HW3/Assets/3.png)

---

- > 写一个简单的总结，总结游戏对象的使用

    游戏对象（GameObject）是一个游戏中最重要的组成部分，对游戏对象的使用一般包括：

    第一步：建模，制作预设，从而可以使用不同形式不同用处的 Object；

    第二步：在场景（Scene）或脚本（Script）中将其实例化，并且设置其各项属性（位置、材质等

    另外，大部分对象的功能实现，由函数完成，当要实现一些游戏的功能时，则是通过调用游戏对象的函数，赋予不同的参数从而决定改变对象的属性。

---

- > 牧师与魔鬼 动作分离版

    本周实验题为：实现上周游戏的动作管理器。

    课件中使用了很多的基类和管理类来实现，但因为这个游戏比较简单，不需要太多复杂的动作，因此我只简单地抽象出游戏对象的动作成为一个类；也就是上一份代码的 Move_script ，在这次作业中我抽象为 ActionManager 类来统一实现所有对象的动作。

    具体新增代码为 Action.cs ：

    <pre>
    <code>
    public class ActionManager : MonoBehaviour
    {
        float move_speed = 15;

        // 0:No-moving 1:moving-to-middle 2:moving-to-destination 3:move-a-boat
        int move_status;

        Vector3 destination;
        Vector3 middle;

        public GameObject obj;

        void Update()
        {
            if (move_status == 1)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, middle, move_speed * Time.deltaTime);
                if (obj.transform.position == middle)
                    move_status = 2;
            }
            else if (move_status == 2)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, destination, move_speed * Time.deltaTime);
                if (obj.transform.position == destination)
                    move_status = 0;
            }
            else if(move_status == 3)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, destination, move_speed * Time.deltaTime);
                if (obj.transform.position == destination)
                    move_status = 0;
            }
        }

        // a boat move
        public void Move(GameObject obj_, Vector3 destination_)
        {
            if (move_status != 0)
                return;
            obj = obj_;
            destination = destination_;
            move_status = 3;
        }

        public void MoveTheRole(GameObject obj_, Vector3 middle_, Vector3 destination_)
        {
            if (move_status != 0)
                return;
            obj = obj_;
            destination = destination_;
            middle = middle_;
            move_status = 1;
        }

        public bool IsMoving()
        {
            if (move_status == 0)
                return false;
            return true;
        }

        public void Reset()
        {
            move_status = 0;
        }
    }
    </code>
    </pre>

    其余几个原 cs 文件也有修改，详细可在代码文件夹中查看；

    另外，与上周作业相比，这周作业修改了一些bug：

    1. 某个对象移动时，其余对象不能改变位置，保证了稳定性；

    2. 游戏胜利触发条件为6人上岸的瞬间，而不是船到岸的时刻。

---
**详细代码请在 Priest-Devil-v2 文件夹中浏览；**

**双击 Priest-Devil-v2/Assets/start 即可开始游戏。**
