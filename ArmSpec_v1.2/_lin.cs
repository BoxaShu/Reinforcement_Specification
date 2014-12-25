using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App = Autodesk.AutoCAD.ApplicationServices;
using Db = Autodesk.AutoCAD.DatabaseServices;

namespace boxashu
{
    class _lin
    {
        // block property
        private Db.ObjectId _objID;
        private string _name;
        
        //DynimicProperty
        private double _length;

        private double _length_SideOne;
        private double _length_SideTwo;

        private double _width;

        //Attribut
        private int _position;
        private int _diameter;
        private int _increment;
        private int _counte;
        private string _comment;
        

        public _lin(Db.ObjectId objID, string name)
        {
            _objID = objID;
            _name = name;

            _position = int.Parse(Commands.GetAttrProperty(objID,"ПОЗ"));
            _diameter = int.Parse(Commands.GetAttrProperty(objID, "ДИАМ"));
            
            //TODO поверить как эта строка будет работать с "Arm_unit_v001"
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

            GetLength(objID, name);
            GetWidth(objID, name);
            GetCount(objID, name);
            //Пропишим вычесленные значения количества в аттрибуты блока.
            Commands.SetAttrProperty(objID, "КОЛ", counte.ToString());

        }

        public Db.ObjectId id
        {
            get { return _objID; }
            //set { _position = value; }
        }
        public int position
        {
            get { return _position; }
            set { 
                _position = value;
                Commands.SetAttrProperty(id, "ПОЗ", value.ToString());
            }
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


        public double length_1
        {
            get { return _length_SideOne; }
            //set { _length = value; }
        }
        public double length_2
        {
            get { return _length_SideTwo; }
            //set { _length = value; }
        }


        public int counte
        {
            get { return _counte; }
            //set { _counte = value; }
        }

        private void GetCount(Db.ObjectId objID, string name)
        {

            switch (name)
            {
                case "Arm_unit_v001":
                case "Arm_zagagulina_v001":
                    _counte = int.Parse(Commands.GetAttrProperty(objID, "КОЛ"));
                    break;
                case "Arm_zone_v002":
                case "Arm_zone_geshka_v002":
                    _counte = (int)Math.Ceiling(_width / _increment) ;
                    break;
                case "Arm_wall_v002_2":
                    _counte = (int.Parse(Commands.GetAttrProperty(objID, "КОЛ")))*2;
                    break;
                default:
                    _counte = (int)Math.Ceiling(_width / _increment) +1;
                    break;
            }

        }

        private void GetWidth(Db.ObjectId objID, string name)
        {

            switch (name)
            {
                case "Arm_unit_v001":
                case "Arm_zagagulina_v001":
                case "Arm_wall_v002_2":
                    _width = Math.Abs(_increment * (_counte-1));
                    //Console.WriteLine("Case 1");
                    break;
                default:
                    _width = Commands.GetDynamicProperty(objID, "ширина");
                    break;
            }
        }


        private void GetLength(Db.ObjectId objID, string name)
        {
            switch (name)
            {
                case "Arm_zone_geshka_v001":
                case "Arm_zone_geshka_v002":
                    _length_SideOne = Commands.GetDynamicProperty(objID, "загиб");
                    _length_SideTwo = 0;
                    _length = Commands.GetDynamicProperty(objID, "длина") + _length_SideOne;
                    //Console.WriteLine("Case 1");
                    break;
                case "Arm_zagagulina_v001":
                    _length_SideOne = Commands.GetDynamicProperty(objID, "загиб_0");
                    _length_SideTwo = Commands.GetDynamicProperty(objID, "загиб_1");
                    _length = boxashu.Commands.GetDynamicProperty(objID, "длина") + _length_SideOne + _length_SideTwo;
                    //Console.WriteLine("Case 2");
                    break;
                case "Arm_unit_v001":
                    _length = double.Parse(Commands.GetAttrProperty(objID, "длина"));
                    _length_SideOne = 0;
                    _length_SideTwo = 0;
                    //Console.WriteLine("Case 2");
                    break;
                case "Arm_homut_v002":
                case "Arm_homut_v001":
                    _length_SideOne = Commands.GetDynamicProperty(objID, "длина стороны");
                    _length_SideTwo = 0;
                    _length = Commands.GetDynamicProperty(objID, "длина");
                    //Console.WriteLine("Case 2");
                    break;
                default:
                    _length = Commands.GetDynamicProperty(objID, "длина");
                    _length_SideOne = 0;
                    _length_SideTwo = 0;
                    break;
            }
        }


    
    
    }


    //public class Employee
    //{
    //    private string name;
    //    public string Name
    //    {
    //        get { return name; }
    //        set { name = value; }
    //    }
    //}
}
