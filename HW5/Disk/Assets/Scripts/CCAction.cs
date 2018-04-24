
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 实际使用的飞行动作
public class CCAction : SSAction
{
    // accleration of gravity
    float g;

    // 水平飞行速度
    float speed;

    // 初始飞行方向
    Vector3 direction;

    // 飞行时间
    float time;

    public override void Start()
    {
        enable = true;
        g = 9.8f;
        speed = gameobject.GetComponent<DiskData>().speed;
        direction = gameobject.GetComponent<DiskData>().direction;
        time = 0;
        gameobject.GetComponent<Rigidbody>().isKinematic = true;
    }
    
    public override void Update()
    {
        if (gameobject.activeSelf)
        {
            time += Time.deltaTime;

            // disk垂直方向的运动
            transform.Translate(Vector3.down * g * time * Time.deltaTime);

            // disk水平方向的运动
            transform.Translate(direction * speed * Time.deltaTime);

            // 飞碟落地
            if (transform.position.y < -4)
            {
                destroy = true;
                enable = false;
                callback.SSActionEvent(this);
            }
        }
    }
    
    public static CCAction GetSSAction()
    {
        CCAction action = ScriptableObject.CreateInstance<CCAction>();
        return action;
    }
}
