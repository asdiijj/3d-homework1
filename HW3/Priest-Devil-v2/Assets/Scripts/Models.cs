using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class and model
namespace game
{
    // two interface
    public interface SceneController
    {
        void LoadResources();
    }
    public interface UserAction
    {
        void MoveBoat();
        void MoveRole(RoleModel role);
        void Restart();
    }

    // the class of director
    public class Director : System.Object
    {
        private static Director instance;
        public SceneController scene_controller { set; get; }

        public static Director GetInstance()
        {
            if (instance == null)
            {
                instance = new Director();
            }
            return instance;
        }
    }

    // the model of coast
    public class CoastModel
    {
        // the real object
        GameObject coast;

        // the position of the coast
        Vector3 position;

        // the position of the priest and devil on coast
        Vector3[] position_p;
        Vector3[] position_d;

        // is the coast on right or on left ?
        bool on_right;

        // the list of roles on coast
        RoleModel[] priest = new RoleModel[3];
        RoleModel[] devil = new RoleModel[3];

        // construct function
        public CoastModel(bool on_right_)
        {
            on_right = on_right_;
            position_p = new Vector3[]
            {
                new Vector3(7, 1.5f, 0),
                new Vector3(8.5f, 1.5f, 0),
                new Vector3(10, 1.5f, 0),
            };
            position_d = new Vector3[]
            {
                new Vector3(11.5f, 1.5f, 0),
                new Vector3(13, 1.5f, 0),
                new Vector3(14.5f, 1.5f, 0),
            };
            
            if (on_right)
            {
                position = new Vector3(12, 0, 0);
                coast = Object.Instantiate(Resources.Load("Prefabs/coast", typeof(GameObject)), position, Quaternion.identity, null) as GameObject;
                coast.name = "right_coast";
            }
            else
            {
                position = new Vector3(-12, 0, 0);
                coast = Object.Instantiate(Resources.Load("Prefabs/coast", typeof(GameObject)), position, Quaternion.identity, null) as GameObject;
                coast.name = "left_coast";
                for (int i = 0; i < 3; ++i)
                {
                    position_p[i].x = -position_p[i].x;
                    position_d[i].x = -position_d[i].x;
                }
            }

            for (int i = 0; i < 3; ++i)
            {
                priest[i] = null;
                devil[i] = null;
            }
        }

        // get an empty position
        public Vector3 GetPosition(bool is_priest)
        {
            if (is_priest)
            {
                for (int i = 0; i < priest.Length; ++i)
                {
                    if (priest[i] == null)
                        return position_p[i];
                }
            }
            else
            {
                for (int i = 0; i < devil.Length; ++i)
                {
                    if (devil[i] == null)
                        return position_d[i];
                }
            }
            return Vector3.zero;
        }

        // get the count number of priest or devil
        public int GetCount(bool is_priest)
        {
            int count = 0;
            if (is_priest)
            {
                for (int i = 0; i < priest.Length; ++i)
                {
                    if (priest[i] != null)
                        ++count;
                }
            }
            else
            {
                for (int i = 0; i < devil.Length; ++i)
                {
                    if (devil[i] != null)
                        ++count;
                }
            }
            return count;
        }

        public bool OnRight()
        {
            return on_right;
        }

        // a role get on coast
        public void AddRole(RoleModel role)
        {
            if (role.IsPriest())
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (priest[i] == null)
                    {
                        priest[i] = role;
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (devil[i] == null)
                    {
                        devil[i] = role;
                        return;
                    }
                }
            }
        }

        // a role get off
        public void DeleteRole(RoleModel role)
        {
            if (role.IsPriest())
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (priest[i] != null && priest[i].GetName() == role.GetName())
                    {
                        priest[i] = null;
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (devil[i] != null && devil[i].GetName() == role.GetName())
                    {
                        devil[i] = null;
                        return;
                    }
                }
            }
        }

        // only reset the role-list
        public void Reset()
        {
            for (int i = 0; i < 3; ++i)
            {
                priest[i] = null;
                devil[i] = null;
            }
        }
    }

    // the model of priest and devil
    public class RoleModel
    {
        GameObject role;
        Click click;
        bool is_priest;     // is priest or is devil
        bool on_coast;      // on coast or on boat
        CoastModel coast;

        public RoleModel(bool is_priest_)
        {
            is_priest = is_priest_;
            on_coast = true;
            coast = (Director.GetInstance().scene_controller as Controller).right_coast;

            if (is_priest)
            {
                role = Object.Instantiate(Resources.Load("Prefabs/priest", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            }
            else
            {
                role = Object.Instantiate(Resources.Load("Prefabs/devil", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            }

            click = role.AddComponent(typeof(Click)) as Click;
            click.SetRole(this);
        }

        public bool IsPriest()
        {
            return is_priest;
        }

        public bool OnCoast()
        {
            return on_coast;
        }

        public void SetPosition(Vector3 position_)
        {
            role.transform.position = position_;
        }

        public void SetName(string name_)
        {
            role.name = name_;
        }

        public string GetName()
        {
            return role.name;
        }

        public CoastModel GetCoast()
        {
            return coast;
        }

        public GameObject GetObject()
        {
            return role;
        }

        public void GetOnCoast(CoastModel coast_)
        {
            role.transform.parent = null;
            coast = coast_;
            on_coast = true;
        }

        public void GetOnBoat(BoatModel boat_)
        {
            role.transform.parent = boat_.GetObject().transform;
            coast = null;
            on_coast = false;
        }

        public void Reset()
        {
            coast = (Director.GetInstance().scene_controller as Controller).right_coast;
            GetOnCoast(coast);
            SetPosition(coast.GetPosition(is_priest));
            coast.AddRole(this);
        }

    }

    // the model of boat
    public class BoatModel
    {
        GameObject boat;
        Click click;

        // the position of the boat
        Vector3 position_right;
        Vector3 position_left;

        // the position of the roles on boat
        Vector3[] positions_r;
        Vector3[] positions_l;

        // is the boat on left or on right ?
        bool on_right;

        // the list of roles on boat
        RoleModel[] roles = new RoleModel[2];

        public BoatModel()
        {
            position_right = new Vector3(4, -0.5f, 0);
            position_left = new Vector3(-4, -0.5f, 0);
            boat = Object.Instantiate(Resources.Load("Prefabs/boat", typeof(GameObject)), position_right, Quaternion.identity) as GameObject;
            boat.name = "boat";
            positions_r = new Vector3[] { new Vector3(3, 0.5f, 0), new Vector3(5, 0.5f, 0), };
            positions_l = new Vector3[] { new Vector3(-5, 0.5f, 0), new Vector3(-3, 0.5f, 0), };
            on_right = true;
            for (int i = 0; i < roles.Length; ++i)
                roles[i] = null;
            click = boat.AddComponent(typeof(Click)) as Click;
        }

        public bool OnRight()
        {
            return on_right;
        }

        public bool IsEmpty()
        {
            for (int i = 0; i < roles.Length; ++i)
            {
                if (roles[i] != null)
                    return false;
            }
            return true;
        }

        public bool IsFull()
        {
            for (int i = 0; i < roles.Length; ++i)
            {
                if (roles[i] == null)
                    return false;
            }
            return true;
        }

        public int GetCount(bool is_priest)
        {
            int[] count = { 0, 0 };
            for (int i = 0; i < roles.Length; ++i)
            {
                if (roles[i] == null)
                    continue;
                else if (roles[i].IsPriest())
                    ++count[0];
                else
                    ++count[1];
            }
            return is_priest ? count[0] : count[1];
        }

        public Vector3 GetDestination()
        {
            on_right = !on_right;
            if (on_right)
                return position_right;
            else
                return position_left;
        }

        public Vector3 GetPosition()
        {
            if (on_right)
            {
                for (int i = 0; i < roles.Length; ++i)
                {
                    if (roles[i] == null)
                        return positions_r[i];
                }
            }
            else
            {
                for (int i = 0; i < roles.Length; ++i)
                {
                    if (roles[i] == null)
                        return positions_l[i];
                }
            }
            return Vector3.zero;
        }

        public GameObject GetObject()
        {
            return boat;
        }

        public void AddRole(RoleModel role)
        {
            for (int i = 0; i < roles.Length; ++i)
            {
                if (roles[i] == null)
                {
                    roles[i] = role;
                    return;
                }
            }
        }

        public void DeleteRole(RoleModel role)
        {
            for (int i = 0; i < roles.Length; ++i)
            {
                if (roles[i] != null && roles[i].GetName() == role.GetName())
                {
                    roles[i] = null;
                    return;
                }
            }
        }

        public void Reset()
        {
            on_right = true;
            boat.transform.position = position_right;
            for (int i = 0; i < roles.Length; ++i)
                roles[i] = null;
        }
    }

    // the click function
    public class Click : MonoBehaviour
    {
        RoleModel role = null;

        UserAction action;

        void Start()
        {
            action = Director.GetInstance().scene_controller as UserAction;
        }

        public void SetRole(RoleModel role_)
        {
            role = role_;
        }

        void OnMouseDown()
        {
            if (role == null)
                action.MoveBoat();
            else
                action.MoveRole(role);
        }
    }
}
