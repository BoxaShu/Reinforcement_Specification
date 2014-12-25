using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App = Autodesk.AutoCAD.ApplicationServices;
using Db = Autodesk.AutoCAD.DatabaseServices;

namespace boxashu
{
    class _tablRow
    {
                // block property
        private int _type; // 1- погонаж, 2- детали, 3- стержни, 4- сборочные единицы
        private int _position;
        private int _diameter;
        private double _length;
        private int _counte;
        private List<Db.ObjectId> _objIDList;

        public _tablRow()
        {
            _type=1;
            _objIDList = new List<Db.ObjectId>();
            _position = 0;
            _diameter = 0;
            _length = 0;
            _counte = 0;
        }
        public _tablRow(int type, int position, int diameter, double length, int counte)
        {
            _type = type;
            _objIDList = new List<Db.ObjectId>();
            _position = position;
            _diameter = diameter;
            _length = length;
            _counte = counte;
        }

        public int type
        {
            get { return _type; }
            set { _type = value; }
        }


        public int position
        {
            get { return _position; }
            set { _position = value; }
        }

        public int diameter
        {
            get { return _diameter; }
            set { _diameter = value; }
        }

        public double length
        {
            get { return _length; }
            set { _length = value; }
        }

        public int counte
        {
            get { return _counte; }
            //set { _counte = value; }
        }

        public List<Db.ObjectId> ObjIDList
        {
            get { return _objIDList; }
            //set { _counte = value; }
        }

        public void AddCounte(int value)
        {
           _counte = _counte+ value; 
        }

        public void RemoveCounte(int value)
        {
             _counte = _counte - value;
        }

        public void AddObjID(Db.ObjectId value)
        {
            _objIDList.Add(value);
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
