using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Director : System.Object
    {
        private static Director instance;

        public static Director getInstance()
        {
            if (instance == null)
                instance = new Director();
            return instance;
        }
        
        public SceneController scene_controller { get; set; }
    }

    public interface SceneController
    {
        void loadResources();
    }

    public interface UserAction
    {
        Vector3 moveBoat();
        Vector3 characterIsClicked(MyCharacterController cha);
        void restart();
        void setMovingObj(GameObject obj);
        GameObject getMovingObj();
        void setTarget(Vector3 t);
        Vector3 getTarget();
        NextStatus getNS();
        void setNS(NextStatus n);
        void nextOnBoat();
        void nextOffBoat();
    }

    //--------------------- Move ----------------------------------
    public class Move: MonoBehaviour
    {
        float move_speed = 10;

        // 0:No-moving 1:moving-to-middle 2:moving-to-destination
        int move_status;

        Vector3 destination;
        Vector3 middle;

        void Update()
        {
            if(move_status == 1)
            {
                transform.position = Vector3.MoveTowards(transform.position, middle, move_speed * Time.deltaTime);
                if (transform.position == middle)
                    move_status = 2;
            }
            else if(move_status == 2)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, move_speed * Time.deltaTime);
                if (transform.position == destination)
                    move_status = 0;
            }
        }

        public void setDestination(Vector3 d)
        {
            destination = d;
            middle = d;

            if(transform.position.y == d.y) // boat move
            {
                move_status = 2;
            }
            else if(transform.position.y > d.y) // to middle
            {
                middle.y = transform.position.y;
            }
            else // to destination
            {
                middle.x = transform.position.x;
            }
            move_status = 1;
        }

        public void Reset()
        {
            move_status = 0;
        }
    }

    //--------------------- CharacterController -------------------
    public class MyCharacterController
    {
        readonly GameObject character;
        readonly Move move_script;
        readonly ClickGUI click_gui;
        readonly bool is_priest;

        bool on_boat;
        CoastController coast_controller;

        // create an object
        public MyCharacterController(bool is_priest_)
        {
            string prefab_str = "";
            if (is_priest_)
                prefab_str = "Prefabs/priest";
            else
                prefab_str = "Prefabs/devil";

            character = Object.Instantiate(Resources.Load(prefab_str, typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            is_priest = is_priest_;

            move_script = character.AddComponent(typeof(Move)) as Move;
            click_gui = character.AddComponent(typeof(ClickGUI)) as ClickGUI;
            click_gui.setController(this);
        }

        public bool isPriest()
        {
            return is_priest;
        }

        public bool onBoat()
        {
            return on_boat;
        }

        public void reset()
        {
            move_script.Reset();
            coast_controller = (Director.getInstance().scene_controller as BaseController).right_coast;
            getOnCoast(coast_controller);
            setPosition(coast_controller.getEmptyPosition(is_priest));
            coast_controller.getOnCoast(this);
        }

        public void setName(string name_)
        {
            character.name = name_;
        }

        public string getName()
        {
            return character.name;
        }

        public void setPosition(Vector3 pos)
        {
            character.transform.position = pos;
        }

        public void getOnCoast(CoastController coast)
        {
            coast_controller = coast;
            character.transform.parent = null;
            on_boat = false;
        }

        public void getOnBoat(BoatController boat)
        {
            coast_controller = null;
            character.transform.parent = boat.getGameObj().transform;
            on_boat = true;
        }

        public CoastController getCoastController()
        {
            return coast_controller;
        }

        public void moveToPosition(Vector3 destination)
        {
            move_script.setDestination(destination);
        }
    }

    //--------------------- CoastController -----------------------
    public class CoastController
    {
        // the real object
        readonly GameObject coast;

        // the position of the coast
        readonly Vector3 right_pos = new Vector3(7, 0.75f, 0);
        readonly Vector3 left_pos = new Vector3(-7, 0.75f, 0);

        // the position of the characters on coast
        readonly Vector3[] positions;

        // is the coast on right or on left ?
        readonly bool is_right;

        MyCharacterController[] charlist;

        public CoastController(bool is_right_)
        {
            positions = new Vector3[]
            {
                new Vector3(4.5f, 2, 0),
                new Vector3(5.3f, 2, 0),
                new Vector3(6.1f, 2, 0),
                new Vector3(6.9f, 2, 0),
                new Vector3(7.7f, 2, 0),
                new Vector3(8.5f, 2, 0),
            };

            charlist = new MyCharacterController[6];

            if (is_right_)
            {
                coast = Object.Instantiate(Resources.Load("Prefabs/coast", typeof(GameObject)), right_pos, Quaternion.identity, null) as GameObject;
                coast.name = "right_coast";
            }
            else
            {
                coast = Object.Instantiate(Resources.Load("Prefabs/coast", typeof(GameObject)), left_pos, Quaternion.identity, null) as GameObject;
                coast.name = "left_coast";
                for (int i = 0; i < 6; ++i)
                    positions[i].x = -positions[i].x;
            }
            is_right = is_right_;
        }

        public Vector3 getEmptyPosition(bool is_priest)
        {
            if(is_priest)
            {
                for(int i = 0; i < 3; i++)
                {
                    if (charlist[i] == null)
                    {
                        return positions[i];
                    }
                }

            }
            else
            {
                for (int i = 3; i < 6; i++)
                {
                    if (charlist[i] == null)
                    {
                        return positions[i];
                    }
                }
            }
            return new Vector3();
        }

        public void getOnCoast(MyCharacterController cha)
        {
            if(cha.isPriest())
            {
                for(int i = 0; i < 3; i++)
                {
                    if(charlist[i] == null)
                    {
                        charlist[i] = cha;
                        return;
                    }
                }
            }
            else
            {
                for (int i = 3; i < 6; i++)
                {
                    if (charlist[i] == null)
                    {
                        charlist[i] = cha;
                        return;
                    }
                }
            }
            return;
        }

        public MyCharacterController getOffCoast(string name)
        {
            for (int i = 0; i < charlist.Length; i++)
            {
                if (charlist[i] != null && charlist[i].getName() == name)
                {
                    MyCharacterController cha = charlist[i];
                    charlist[i] = null;
                    return cha;
                }
            }
            return null;
        }

        public int getCount(bool is_priest)
        {
            int[] num = { 0, 0 };
            for(int i = 0; i < charlist.Length; i++)
            {
                if (charlist[i] == null)
                    continue;
                else if (charlist[i].isPriest())
                    ++num[0];
                else
                    ++num[1];
            }
            if (is_priest)
                return num[0];
            else
                return num[1];
        }

        public void reset()
        {
            charlist = new MyCharacterController[6];
        }

        public bool isRight()
        {
            return is_right;
        }
    }

    //--------------------- BoatController ------------------------
    public class BoatController
    {
        readonly GameObject boat;
        readonly Move move_script;

        readonly Vector3 right_pos = new Vector3(3, 0.5f, 0);
        readonly Vector3 left_pos = new Vector3(-3, 0.5f, 0);
        readonly Vector3[] right_positions;
        readonly Vector3[] left_positions;

        bool is_right;
        public MyCharacterController[] charlist = new MyCharacterController[2];

        public BoatController()
        {
            is_right = true;

            right_positions = new Vector3[]
            {
                new Vector3(2.5f, 1, 0),
                new Vector3(3.5f, 1, 0),
            };
            left_positions = new Vector3[]
            {
                new Vector3(-3.5f, 1, 0),
                new Vector3(-2.5f, 1, 0),
            };

            boat = Object.Instantiate(Resources.Load("Prefabs/boat", typeof(GameObject)), right_pos, Quaternion.identity, null) as GameObject;
            boat.name = "boat";

            move_script = boat.AddComponent(typeof(Move)) as Move;
            boat.AddComponent(typeof(ClickGUI));
        }

        public Vector3 Move()
        {
            Vector3 target;
            if(is_right)
            {
                move_script.setDestination(left_pos);
                is_right = false;
                target = left_pos;
            }
            else
            {
                move_script.setDestination(right_pos);
                is_right = true;
                target = right_pos;
            }
            return target;
        }

        public bool isEmpty()
        {
            for (int i = 0; i < charlist.Length; ++i)
            {
                if (charlist[i] != null)
                    return false;
            }
            return true;
        }

        public bool isFull()
        {
            for(int i = 0; i < charlist.Length; ++i)
            {
                if (charlist[i] == null)
                    return false;
            }
            return true;
        }

        public GameObject getGameObj()
        {
            return boat;
        }

        public int getCount(bool is_priest)
        {
            int[] num = { 0, 0 };
            for (int i = 0; i < charlist.Length; i++)
            {
                if (charlist[i] == null)
                    continue;
                else if (charlist[i].isPriest())
                    ++num[0];
                else
                    ++num[1];
            }
            if (is_priest)
                return num[0];
            else
                return num[1];
        }

        public bool isRight()
        {
            return is_right;
        }

        public void reset()
        {
            move_script.Reset();
            if (!isRight())
                Move();
            charlist = new MyCharacterController[2];
        }

        public void getOnBoat(MyCharacterController cha)
        {
            for(int i = 0; i < 2; ++i)
                if(charlist[i] == null)
                {
                    charlist[i] = cha;
                    return;
                }
            return;
        }

        public MyCharacterController getOffBoat(string charlist_name)
        {
            for (int i = 0; i < charlist.Length; i++)
            {
                if (charlist[i] != null && charlist[i].getName() == charlist_name)
                {
                    MyCharacterController cha = charlist[i];
                    charlist[i] = null;
                    return cha;
                }
            }
            return null;
        }

        public Vector3 getEmptyPosition()
        {
            Vector3 pos = new Vector3();
            int index = -1;
            for(int i = 0; i < charlist.Length; i++)
            {
                if(charlist[i] == null)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
                return pos;

            if (is_right)
                pos = right_positions[index];
            else
                pos = left_positions[index];
            return pos;
        }
    }
}