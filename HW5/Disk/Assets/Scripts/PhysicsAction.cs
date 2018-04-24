
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 实际使用的飞行动作 - 物理运动版本
public class PhysicsAction : SSAction
{
    // 水平飞行速度
    float speed;

    // 初始飞行方向
    Vector3 direction;

    // 此版本新增成员，刚体
    Rigidbody rigidbody;

    public override void Start()
    {
        enable = true;
        speed = gameobject.GetComponent<DiskData>().speed;
        direction = gameobject.GetComponent<DiskData>().direction;

        rigidbody = gameobject.GetComponent<Rigidbody>();
        if (rigidbody)
        {
            // 设置刚体
            rigidbody.velocity = speed * direction;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    // 不需要改变rigidbody的值，无需使用FixedUpdate
    public override void Update()
    {
        // 因为设置了刚体，不需要在这里实现运动轨迹
        if (gameobject.activeSelf)
        {
            // 飞碟落地
            if (transform.position.y < -4)
            {
                destroy = true;
                enable = false;
                callback.SSActionEvent(this);
            }
        }
    }

    public static PhysicsAction GetSSAction()
    {
        PhysicsAction action = ScriptableObject.CreateInstance<PhysicsAction>();
        return action;
    }
}
