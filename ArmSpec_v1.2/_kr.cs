using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App = Autodesk.AutoCAD.ApplicationServices;
using Db = Autodesk.AutoCAD.DatabaseServices;

namespace boxashu
{
    class _kr
    {
        // block property
        private Db.ObjectId _objID;
        private string _name;


        //DynimicProperty
        private double _length;
        private double _width;

        //Attribut
        private int _position;
        private int _diameter;
        private int _increment;
        private int _counte;
        private string _comment;

        private int _wall_width;

        ////case "Arm_wall_v002_4":
        ////case "Arm_wall_v002_3":
        ////case "Arm_wall_v002_17":


        public _kr(Db.ObjectId objID, string name)
        {
            _objID = objID;
            _name = name;

            _position = int.Parse(Commands.GetAttrProperty(objID,"ПОЗ"));
            _diameter = int.Parse(Commands.GetAttrProperty(objID, "ДИАМ"));

            string temp_str = Commands.GetAttrProperty(objID, "ШАГ");
            if (temp_str == string.Empty)
            {
                _increment = 100;
            }
            else
            {
                _increment = int.Parse(temp_str);
            }
            
            // Предварительное назначение
            _counte = int.Parse(Commands.GetAttrProperty(objID, "КОЛ"));
            
            _length = Commands.GetDynamicProperty(objID, "длина");

            GetWidth(objID, name);
            GetCount(objID, name);

            _wall_width = int.Parse(Commands.GetAttrProperty(objID, "т.стены"));

        }

        public Db.ObjectId id
        {
            get { return _objID; }
            //set { _position = value; }
        }
        public int position
        {
            get { return _position; }
            //set { _position = value; }
        }

        public int diameter
        {
            get { return _diameter; }
            //set { _diameter = value; }
        }
        public double length
        {
            get { return _length; }
            //set { _length = value; }
        }


        public int counte
        {
            get { return _counte; }
            //set { _counte = value; }
        }

        public int wall_width
        {
            get { return _wall_width; }
            //set { _counte = value; }
        }

        private void GetCount(Db.ObjectId objID, string name)
        {

            switch (name)
            {
                case "Arm_wall_v002_4":
                    _counte = int.Parse(Commands.GetAttrProperty(objID, "КОЛ"));
                    break;
                default:
                    _counte = (int)Math.Ceiling(_width / _increment) + 1;
                    break;
            }
        }

        private void GetWidth(Db.ObjectId objID, string name)
        {

            switch (name)
            {
                case "Arm_wall_v002_4":
                    _width = Math.Abs(_increment * (_counte - 1));
                    //Console.WriteLine("Case 1");
                    break;
                default:
                    _width = Commands.GetDynamicProperty(objID, "ширина");
                    break;
            }
        }
    }
}
