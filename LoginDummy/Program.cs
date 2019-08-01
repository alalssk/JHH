using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginDummy
{

    public class User:GlobalObject
    {
        public string UserName;
        public int UserIdx;

        public User(int _x, int _y)
        {
            m_ObjectType = ObjectType.User;
            name = "P";
            x = _x;
            y = _y;
        }

    }
    public class GlobalObject
    {
        public enum ObjectType
        {
            None = 0,
            User,
            Monster,
            Some,
        }
        public enum vector
        {
            None = 0,
            Left,
            Right,
            Up,
            Down,
            R_up,
            R_down,
            L_up,
            L_down
        }
        public ObjectType m_ObjectType;
        public string name;
        public int x, y;
        public GlobalObject() { }
        public GlobalObject(int _x, int _y)
        {
            name = ".";
            x = _x;
            y = _y;
        }
        public virtual void SetObject() { }
        //public void spawn(ObjectType _type, )
        public bool move(vector _v, ref GlobalObject[,] _map)
        {
            switch(_v)
            {
                case vector.Left:
                    x--;
                    break;
                case vector.Right:
                    x++;
                    break;
                case vector.Up:
                    y++;
                    break;
                case vector.Down:
                    y--;
                    break;
                case vector.R_up:
                    x++; y++;
                    break;
                case vector.R_down:
                    x++; y--;
                    break;
                case vector.L_up:
                    x--; y++;
                    break;
                case vector.L_down:
                    x--; y--;
                    break;
            }
            return true;
        }
    }
    public class Map
    {
        GlobalObject[,] m_location;
        //HashSet<GlobalObject>[,] m_loca;
        int MAX_x;
        int MAX_y;
        public Map(int _maxX, int _maxY)
        {
            MAX_x = _maxX;
            MAX_y = _maxY;
            m_location = new GlobalObject[MAX_x, MAX_y];

            //m_loca[0, 0] = new HashSet<GlobalObject>();
            for (int i = 0; i < MAX_x; i++)
            {
                for (int j = 0; j < MAX_y; j++)
                {
                    m_location[i, j] = new GlobalObject(i, j);
                }
            }
        }
        public void SetObject(GlobalObject _obj, int _x, int _y)
        {
            m_location[_x, _y] = _obj;
        }

    }
    class Program
    {
        public const int MaxX = 50;
        public const int MaxY = 50;
        static void Main(string[] args)
        {
            //GlobalObject[,] map = new GlobalObject[MaxX, MaxY];

            //for (int i = 0; i < MaxY; i++)
            //{
            //    for (int j = 0; j < MaxY; j++)
            //    {
            //        map[i, j] = new GlobalObject(i,j);
            //    }
            //}
            Map map1 = new Map(50, 50);

            User NewUser = new GlobalObject(3, 4) as User;
            NewUser.UserIdx = 1010;
            NewUser.UserName = "alalssk";

            map1.SetObject(NewUser, 3, 4);

            for (int i = 0; i < MaxY; i++)
            {
                for (int j = 0; j < MaxY; j++)
                {
                    Console.Write("{0} ", map[i, j].name);
                    //Console.WriteLine("name:{0}, x:{1}, y:{2}", m[i, j].name, m[i, j].x, m[i, j].y);
                }
                Console.WriteLine();

            }

            //GlobalObject user = new GlobalObject(3, 4);
            //user.name = "y";

            //map[user.x, user.y] = user;



        }
    }
}
