using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
