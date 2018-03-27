## 简答题

- > 解释 游戏对象（GameObjects） 和 资源（Assets）的区别与联系

    **游戏对象：** 是所有组件的容器，是一些资源的集合体，是资源使用的具体表现。

    **资源：** 资源不能直接用于游戏场景中，需要借助代码和对象来表现。

    **区别与联系：** 对象是游戏制作中的模型，组件，；资源则是对象使用的材料。对象可由多个资源构成，资源也可用于多个对象。

---

- > 下载几个游戏案例，分别总结资源、对象组织的结构（指资源的目录组织结构与游戏对象树的层次结构）

    项目的资源一般均放在Assets文件夹内，再细分到pictures，models等文件夹。

    对象采用树形结构组织，以完整个体为父对象，再下分继承各种子对象。

---

- > 编写一个代码，使用 debug 语句来验证 MonoBehaviour 基本行为或事件触发的条件

    <pre>
    <code>
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;

        public class BehaviourTest : MonoBehaviour {
            void Awake()
            {
                Debug.Log("Awake");
            }
            void Start () {
                Debug.Log("Start");
            }
            void Update () {
                Debug.Log("Update");
            }
            void FixedUpdate()
            {
                Debug.Log("FixedUpdate");
            }
            void LateUpdate()
            {
                Debug.Log("LateUpdate");
            }
            void OnGUI()
            {
                Debug.Log("OnGUI");
            }
            void OnDisable()
            {
                Debug.Log("OnDisable");
            }
            void OnEnable()
            {
                Debug.Log("OnEnable");
            }
        }
    </code>
    </pre>

---

- > 查找脚本手册，了解 GameObject，Transform，Component 对象

    - **翻译**

        GameObject 是Unity中代表人物，道具，场景的基本对象。它们本身并不能完全发挥作用，需要作为其它部件的容器，才能发挥功能。

        Transform 部件决定了每个对象在场景中的位置、旋转和缩放比例。每个对象都有一个Transform。

        Component 部件是一个游戏中对象和行为的细节，是每个游戏对象功能部分。

    - **描述**

        对象属性：activeInHierarchy activeSelf isStatic scene tag transform

        转换属性：Position Rotation scale

        部件：Mesh Filter, Mesh Renderer, Box Collider

    - **UML**
---

- > 整理相关学习资料，编写简单代码验证以下技术的实现：

    - 查找对象
    <pre>
    <code>
        // find an object by name
        public static GameObject Find(string name);

        // find an object by tag
        public static GameObject FindGameObjectWithTag(string tag);

        // find any object by tag
        public static GameObject[] FindGameObjectsWithTag(string tag);

        // find an object by type
        public static GameObject FindObjectOfType(System.Type type);

        // find any object by type
        public static GameObject[] FindObjectsOfType(System.Type type);
    </code>
    </pre>

    - 添加子对象
    <pre>
    <code>
        public static GameObject CreatePrimitive(PrimitiveType type);
    </code>
    </pre>

    - 遍历对象树
    <pre>
    <code>
        foreach(Transform child in transform) {
            // operation
        }
    </code>
    </pre>

    - 清除所有子对象
    <pre>
    <code>
        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }
    </code>
    </pre>

---

- > 资源预设（Prefabs）与 对象克隆 (clone)

    - 预设（Prefabs）相当于构造一个模板，将相同属性与组件、资源封装在一起，当需要再次创建类似对象时，只需直接将预设作为模板，即可快速创建。适合于批量处理。

    - 预设的实例是相互关联的，而克隆的个体则互相独立。

    - table预制
    <pre>
    <code>
        public class clone : MonoBehaviour {
            public GameObject table;

            void Start() {
                GameObject anotherTable = (GameObject)Instantiate(table.gameObject);
                anotherTable.transform.position = new Vector3(0,1,0);
                anotherTable.transform.parent = this.transform;
            }
        }
    </code>
    </pre>

---

- > 尝试解释组合模式（Composite Pattern / 一种设计模式）。

    组合模式，将对象组合成树形结构，以表示“部分-整体”的层次结构，使得客户端对单个对象和组合对象的使用具有一致性。

    <pre>
    <code>
        //具体使用例子：

        //子对象的某个方法
        void fun() {
            Debug.Log("test");
        }

        //那么父对象就可以使用其子对象的方法
        void start() {
            this.BroadcastMessage("fun");
        }
    </code>
    </pre>

---
