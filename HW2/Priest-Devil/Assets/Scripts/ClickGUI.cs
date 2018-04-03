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
	}

    public void setController(MyCharacterController cha)
    {
        character_controller = cha;
    }

    void OnMouseDown()
    {
        if(gameObject.name == "boat")
        {
            action.moveBoat();
        }
        else
        {
            action.characterIsClicked(character_controller);
        }
    }
}
