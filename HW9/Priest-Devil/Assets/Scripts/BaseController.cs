using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public enum BoatStatus { P, D, PP, DD, PD }

public enum NextStatus { NONE, ON, MOVE, OFF }

public class BaseController : MonoBehaviour, SceneController, UserAction {

    readonly Vector3 river_pos = new Vector3(0, 0, 0);

    UserGUI user_gui;

    public CoastController right_coast;
    public CoastController left_coast;
    public BoatController boat;
    private MyCharacterController[] characters;

    GameObject movingObj;
    Vector3 target;

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

    public Vector3 moveBoat()
    {
        Vector3 target = new Vector3();
        if (boat.isEmpty())
            return target;
        target = boat.Move();
        user_gui.status = judge();
        return target;
    }

    public Vector3 characterIsClicked(MyCharacterController cha)
    {
        Vector3 target = new Vector3();
        if(cha.onBoat())
        {
            CoastController coast;
            if (boat.isRight())
                coast = right_coast;
            else
                coast = left_coast;

            boat.getOffBoat(cha.getName());
            target = coast.getEmptyPosition(cha.isPriest());
            cha.moveToPosition(target);
            cha.getOnCoast(coast);
            coast.getOnCoast(cha);
        }
        else
        {
            CoastController coast = cha.getCoastController();

            // boat is full
            if (boat.isFull())
                return target;

            if (coast.isRight() != boat.isRight())
                return target;

            coast.getOffCoast(cha.getName());
            target = boat.getEmptyPosition();
            cha.moveToPosition(target);
            cha.getOnBoat(boat);
            boat.getOnBoat(cha);
        }
        user_gui.status = judge();
        return target;
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
        movingObj = null;
        user_gui.next = NextStatus.ON;
        for (int i = 0; i < characters.Length; ++i)
            characters[i].reset();
    }

    public BoatStatus getNextBS()
    {
        BoatStatus status = new BoatStatus();
        int p = right_coast.getCount(true);
        int d = right_coast.getCount(false);
        if (boat.isRight())
        {
            p += boat.getCount(true);
            d += boat.getCount(false);
        }
        bool isRight = boat.isRight();

        // the right path
        if (p == 3 && d == 3 && isRight)
        {
            status = BoatStatus.PD;
        }
        else if (p == 2 && d == 2 && !isRight)
        {
            status = BoatStatus.P;
        }
        else if (p == 3 && d == 2 && isRight)
        {
            status = BoatStatus.DD;
        }
        else if (p == 3 && d == 0 && !isRight)
        {
            status = BoatStatus.D;
        }
        else if (p == 3 && d == 1 && isRight)
        {
            status = BoatStatus.PP;
        }
        else if (p == 1 && d == 1 && !isRight)
        {
            status = BoatStatus.PD;
        }
        else if (p == 2 && d == 2 && isRight)
        {
            status = BoatStatus.PP;
        }
        else if (p == 0 && d == 2 && !isRight)
        {
            status = BoatStatus.D;
        }
        else if (p == 0 && d == 3 && isRight)
        {
            status = BoatStatus.DD;
        }
        else if (p == 0 && d == 1 && !isRight)
        {
            status = BoatStatus.D;
        }
        else if (p == 0 && d == 2 && isRight)
        {
            status = BoatStatus.DD;
        }
        // the other status
        else if (p == 3 && d == 2 && !isRight)
        {
            status = BoatStatus.D;
        }
        else if (p == 3 && d == 1 && !isRight)
        {
            status = BoatStatus.DD;
        }

        return status;
    }

    public void nextOnBoat()
    {
        BoatStatus status = getNextBS();
        nextOffBoat();
        if (status == BoatStatus.P)
        {
            for (int i = 0; i < characters.Length; ++i)
            {
                if (characters[i].getCoastController().isRight() == boat.isRight()
                    && characters[i].isPriest())
                {
                    characterIsClicked(characters[i]);
                    break;
                }
            }
        }
        else if (status == BoatStatus.PP)
        {
            int count = 0;
            for (int i = 0; i < characters.Length; ++i)
            {
                if (characters[i].getCoastController().isRight() == boat.isRight()
                    && characters[i].isPriest())
                {
                    characterIsClicked(characters[i]);
                    ++count;
                    if (count == 2)
                        break;
                }
            }
        }
        else if (status == BoatStatus.D)
        {
            for (int i = 0; i < characters.Length; ++i)
            {
                if (characters[i].getCoastController().isRight() == boat.isRight()
                    && !characters[i].isPriest())
                {
                    characterIsClicked(characters[i]);
                    break;
                }
            }
        }
        else if (status == BoatStatus.DD)
        {
            int count = 0;
            for (int i = 0; i < characters.Length; ++i)
            {
                if (characters[i].getCoastController().isRight() == boat.isRight()
                    && !characters[i].isPriest())
                {
                    characterIsClicked(characters[i]);
                    ++count;
                    if (count == 2)
                        break;
                }
            }
        }
        else if (status == BoatStatus.PD)
        {
            int count_p = 0;
            int count_d = 0;
            for (int i = 0; i < characters.Length; ++i)
            {
                if (characters[i].getCoastController().isRight() == boat.isRight())
                {
                    if (count_p == 0 && characters[i].isPriest())
                    {
                        characterIsClicked(characters[i]);
                        count_p++;
                    }
                    else if (count_d == 0 && !characters[i].isPriest())
                    {
                        characterIsClicked(characters[i]);
                        count_d++;
                    }
                }
            }
        }
    }
    public void nextOffBoat()
    {
        for (int i = 0; i < boat.charlist.Length; ++i)
        {
            if (boat.charlist[i] == null)
                continue;
            else
            {
                characterIsClicked(boat.charlist[i]);
            }
        }
    }

    public void setMovingObj(GameObject obj)
    {
        movingObj = obj;
    }
    public GameObject getMovingObj()
    {
        return movingObj;
    }

    public void setTarget(Vector3 t)
    {
        target = t;
    }
    public Vector3 getTarget()
    {
        return target;
    }

    public NextStatus getNS()
    {
        return user_gui.next;
    }
    public void setNS(NextStatus n)
    {
        user_gui.next = n;
    }
}
