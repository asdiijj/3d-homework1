using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game;

public class Controller : MonoBehaviour, SceneController, UserAction
{

    // members
    public CoastModel right_coast;
    public CoastModel left_coast;
    public BoatModel boat;
    private RoleModel[] priest;
    private RoleModel[] devil;

    public ActionManager action_manager;

    public UserGUI user_gui;

    // Use this for initialization
    void Start () {
        Director director = Director.GetInstance();
        director.scene_controller = this;

        LoadResources();

        action_manager = gameObject.AddComponent<ActionManager>() as ActionManager;

        user_gui = gameObject.AddComponent<UserGUI>() as UserGUI;
	}

    public void MoveBoat()
    {
        if (user_gui.status != 0)
            return;
        if (action_manager.IsMoving())
            return;
        if (boat.IsEmpty())
            return;
        action_manager.Move(boat.GetObject(), boat.GetDestination());
        user_gui.status = judge();
    }

    public void MoveRole(RoleModel role)
    {
        if (user_gui.status != 0)
            return;
        if (action_manager.IsMoving())
            return;
        CoastModel role_coast = null;
        if (role.OnCoast())
        {
            role_coast = role.GetCoast();
            if ((boat.IsFull()) || (role_coast.OnRight() != boat.OnRight()))
                return;

            // the coast
            role_coast.DeleteRole(role);

            // the role
            
            Vector3 destination = boat.GetPosition();
            Vector3 middle = destination;
            middle.y = role.GetObject().transform.position.y;
            action_manager.MoveTheRole(role.GetObject(), middle, destination);
            role.GetOnBoat(boat);

            // the boat
            boat.AddRole(role);
        }
        else
        {
            if (boat.OnRight())
                role_coast = right_coast;
            else
                role_coast = left_coast;

            // the boat
            boat.DeleteRole(role);

            // the role
            Vector3 destination = role_coast.GetPosition(role.IsPriest());
            Vector3 middle = destination;
            middle.x = role.GetObject().transform.position.x;
            action_manager.MoveTheRole(role.GetObject(), middle, destination);
            role.GetOnCoast(role_coast);

            // the coast
            role_coast.AddRole(role);
        }
        user_gui.status = judge();
    }

    public void LoadResources()
    {
        right_coast = new CoastModel(true);
        left_coast = new CoastModel(false);
        boat = new BoatModel();
        priest = new RoleModel[3];
        devil = new RoleModel[3];

        for (int i = 0; i < 3; ++i)
        {
            priest[i] = new RoleModel(true);
            priest[i].SetName("priest" + i);
            priest[i].SetPosition(right_coast.GetPosition(true));
            priest[i].GetOnCoast(right_coast);
            right_coast.AddRole(priest[i]);

            devil[i] = new RoleModel(false);
            devil[i].SetName("devil" + i);
            devil[i].SetPosition(right_coast.GetPosition(false));
            devil[i].GetOnCoast(right_coast);
            right_coast.AddRole(devil[i]);
        }
    }

    public void Restart()
    {
        right_coast.Reset();
        left_coast.Reset();
        boat.Reset();
        for(int i = 0; i < 3; ++i)
        {
            priest[i].Reset();
            devil[i].Reset();
        }
    }

    public int judge()
    {
        int right_p = right_coast.GetCount(true);    // p: priest
        int right_d = right_coast.GetCount(false);    // d: devil
        int left_p = left_coast.GetCount(true);
        int left_d = left_coast.GetCount(false);

        // win the game
        if (left_d + left_p == 6)
            return 1;

        if (boat.OnRight())
        {
            right_p += boat.GetCount(true);
            right_d += boat.GetCount(false);
        }
        else
        {
            left_p += boat.GetCount(true);
            left_d += boat.GetCount(false);
        }

        if (right_p < right_d && right_p > 0)
            return 2;
        if (left_p < left_d && left_p > 0)
            return 2;
        return 0;
    }
}
