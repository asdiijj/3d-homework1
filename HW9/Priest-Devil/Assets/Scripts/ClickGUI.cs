using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class ClickGUI : MonoBehaviour {

    UserAction action;
    MyCharacterController character_controller;

	// Use this for initialization
	void Start () {
        action = Director.getInstance().scene_controller as UserAction;
        action.setMovingObj(null);
    }

    public void setController(MyCharacterController cha)
    {
        character_controller = cha;
    }

    void OnMouseDown()
    {
        action.setNS(NextStatus.ON);
        if (action.getMovingObj() != null)
        {
            return;
        }
        action.setMovingObj(gameObject);
        if(gameObject.name == "boat")
        {
            action.setTarget(action.moveBoat());
        }
        else
        {
            action.setTarget(action.characterIsClicked(character_controller));
        }
    }

    private void OnGUI()
    {
        if (action.getMovingObj() == null)
        {
            return;
        }
        GameObject movingObj = action.getMovingObj();
        Vector3 target = action.getTarget();
        if (movingObj.transform.position == target)
        {
            action.setMovingObj(null);
        }
    }
}
