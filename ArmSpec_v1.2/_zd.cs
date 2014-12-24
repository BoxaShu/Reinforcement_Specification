using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App = Autodesk.AutoCAD.ApplicationServices;
using Db = Autodesk.AutoCAD.DatabaseServices;

namespace boxashu
{
    class _zd
    {
        // block property
        private Db.ObjectId _objID;
        private string _name;

        public _zd(Db.ObjectId objID, string name)
        {
            _objID = objID;
            _name = name;
        }
    }
}
