using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class BaseController : MonoBehaviour, SceneController, UserAction {

    readonly Vector3 river_pos = new Vector3(0, 0, 0);

    UserGUI user_gui;

    public CoastController right_coast;
    public CoastController left_coast;
    public BoatController boat;
    private MyCharacterController[] characters;

    // 初始化
	void Awake()
    {
        Director director = Director.getInstance();
        director.scene_controller = this;
        user_gui = gameObject.AddComponent<UserGUI>() as UserGUI;
        characters = new MyCharacterController[6];
        loadResources();
    }

    // 加载资源，对象
    public void loadResources()
    {
        GameObject river = Instantiate(Resources.Load("Prefabs/river", typeof(GameObject)), river_pos, Quaternion.identity, null) as GameObject;
        river.name = "river";

        right_coast = new CoastController(true);
        left_coast = new CoastController(false);
        boat = new BoatController();

        for(int i = 0; i < 3; ++i)
        {
            characters[i] = new MyCharacterController(true);
            characters[i].setName("priest" + i);
            characters[i].setPosition(right_coast.getEmptyPosition(true));
            characters[i].getOnCoast(right_coast);
            right_coast.getOnCoast(characters[i]);
        }
        for (int i = 3; i < 6; ++i)
        {
            characters[i] = new MyCharacterController(false);
            characters[i].setName("devil" + i);
            characters[i].setPosition(right_coast.getEmptyPosition(false));
            characters[i].getOnCoast(right_coast);
            right_coast.getOnCoast(characters[i]);
        }
    }

    public void moveBoat()
    {
        if (boat.isEmpty())
            return;
        boat.Move();
        user_gui.status = judge();
    }

    public void characterIsClicked(MyCharacterController cha)
    {
        if(cha.onBoat())
        {
            CoastController coast;
            if (boat.isRight())
                coast = right_coast;
            else
                coast = left_coast;

            boat.getOffBoat(cha.getName());
            cha.moveToPosition(coast.getEmptyPosition(cha.isPriest()));
            cha.getOnCoast(coast);
            coast.getOnCoast(cha);
        }
        else
        {
            CoastController coast = cha.getCoastController();

            // boat is full
            if (boat.isFull())
                return;

            if (coast.isRight() != boat.isRight())
                return;

            coast.getOffCoast(cha.getName());
            cha.moveToPosition(boat.getEmptyPosition());
            cha.getOnBoat(boat);
            boat.getOnBoat(cha);
        }
        user_gui.status = judge();
    }

    // 1:win 2:lose
    int judge()
    {
        int right_p = right_coast.getCount(true);    // p: priest
        int right_d = right_coast.getCount(false);    // d: devil
        int left_p = left_coast.getCount(true);
        int left_d = left_coast.getCount(false);

        // win the game
        if (left_d + left_p == 6)
            return 1;

        if (boat.isRight())
        {
            right_p += boat.getCount(true);
            right_d += boat.getCount(false);
        }
        else
        {
            left_p += boat.getCount(true);
            left_d += boat.getCount(false);
        }

        if (right_p < right_d && right_p > 0)
            return 2;
        if (left_p < left_d && left_p > 0)
            return 2;
        return 0;
    }

    public void restart()
    {
        boat.reset();
        right_coast.reset();
        left_coast.reset();
        for (int i = 0; i < characters.Length; ++i)
            characters[i].reset();
    }
}
