using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using App = Autodesk.AutoCAD.ApplicationServices;
using cad = Autodesk.AutoCAD.ApplicationServices.Application;
using Db = Autodesk.AutoCAD.DatabaseServices;
using Ed = Autodesk.AutoCAD.EditorInput;
using Gem = Autodesk.AutoCAD.Geometry;
using Rtm = Autodesk.AutoCAD.Runtime;
using Win = Autodesk.AutoCAD.Windows;

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;


[assembly: Rtm.CommandClass(typeof(boxashu.Commands))]

namespace boxashu
{
    public class Commands
    {
        private const double length_planed_reinforcement = 11700;


        private static Dictionary<string, _tablRow> pogonTab;
        private static Dictionary<string, _tablRow> detalTab;
        private static Dictionary<string, _tablRow> linTab;

        private static Dictionary<string, _tablRow> krTab;
        private static Dictionary<string, _tablRow> zdTab;

        //создаю палитры
        public static Win.PaletteSet _ps = null;
      

        [Rtm.CommandMethod("bx_armsp")]
        static public void bx_armsp()
        {
            
            //// Списки арматуры:
            //Пгонажная арматура
            List<_lin> pogon = new List<_lin>();
            // Детали, гнутые стержни, хомуты, шпильки
            List<_lin> detal = new List<_lin>();
            //Прямая арматура
            List<_lin> lin = new List<_lin>();
            // Сборочные единицы
            // - Каркасы
            List<_kr> kr = new List<_kr>();
            // - Закладные детали
            List<_zd> zd = new List<_zd>();

            
            // Получение текущего документа и базы данных
            App.Document acDoc = App.Application.DocumentManager.MdiActiveDocument;
            Db.Database acCurDb = acDoc.Database;
            Ed.Editor acEd = acDoc.Editor;


            ////////Секция предварительного выбора
            //////Ed.PromptSelectionResult acSSPrompt = acEd.SelectImplied();
            //////Ed.SelectionSet acSSet = null;

            //////// Если статус запроса OK, объекты были выбраны перед запуском команды
            //////if (acSSPrompt.Status == Ed.PromptStatus.OK)
            //////    acSSet = acSSPrompt.Value;

            //////Dictionary<Db.ObjectId, string> ObjID_Dic = new Dictionary<Db.ObjectId, string>();

            
            
            
            //Ed.PromptEntityOptions EntOpt = new Ed.PromptEntityOptions("\n Select block:");
            //EntOpt.SetRejectMessage("\n Entity must be a block.");
            //EntOpt.AddAllowedClass(typeof(Db.BlockReference), false);
            //EntOpt.AllowObjectOnLockedLayer = true;
            //Ed.PromptEntityResult EntRes = acEd.GetEntity(EntOpt);

            //if (EntRes.Status != Ed.PromptStatus.OK)
            //{
            //    acEd.WriteMessage("\n Cencel.");
            //    return;
            //}

                Db.TypedValue[] acTypValAr = new Db.TypedValue[1];
                acTypValAr.SetValue(new Db.TypedValue((int)Db.DxfCode.Start, "INSERT"), 0);
                Ed.SelectionFilter acSelFtr = new Ed.SelectionFilter(acTypValAr);


                Ed.PromptSelectionResult acSSPrompt = acDoc.Editor.GetSelection(acSelFtr);
                if (acSSPrompt.Status != Ed.PromptStatus.OK)
                {
                    return;
                }

            String acBlockName = "0";

            // старт транзакции
            // Ищу истенное имя выбранного блока и читаю его атрибуты
            using (Db.Transaction acTrans = acCurDb.TransactionManager.StartOpenCloseTransaction())
            {
                // Открытие таблицы Блоков для чтения
                Db.BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                Db.OpenMode.ForRead) as Db.BlockTable;
                // Открытие записи таблицы Блоков пространства Модели для записи
                Db.BlockTableRecord acBlkTblRecMS = acTrans.GetObject(acBlkTbl[Db.BlockTableRecord.ModelSpace],
                                                Db.OpenMode.ForRead) as Db.BlockTableRecord;

                    Ed.SelectionSet acSSet = acSSPrompt.Value;
                    foreach (Ed.SelectedObject acSSObj in acSSet)
                    {
                        if (acSSObj != null)
                        {
                            Db.Entity acEnt = acTrans.GetObject(acSSObj.ObjectId,
                                                     Db.OpenMode.ForRead) as Db.Entity;
                            if (acEnt != null)
                            {
                                if (acEnt is Db.BlockReference)
                                {
                                    Db.BlockReference acBlock = (Db.BlockReference)acEnt;

                //Получаю выбранный блок
               // Db.BlockReference acBlock = acTrans.GetObject(EntRes.ObjectId, Db.OpenMode.ForRead) as Db.BlockReference;
                //Получаю определение блока в таблице блоков
                Db.BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlock.BlockTableRecord, Db.OpenMode.ForRead)
                    as Db.BlockTableRecord;


                // Получаю определение блока в таблице динамических блоков
                // Запоминаю истинное имя блока
                acBlockName = acBlock.Name;

                if (acBlock.IsDynamicBlock)
                {
                    acBlkTblRec = acTrans.GetObject(acBlock.DynamicBlockTableRecord, Db.OpenMode.ForRead) as Db.BlockTableRecord;
                    Db.BlockTableRecord blr_nam = acTrans.GetObject(acBlkTblRec.ObjectId, Db.OpenMode.ForRead) as Db.BlockTableRecord;
                    acBlockName = blr_nam.Name;

                    if (acBlkTblRec.HasAttributeDefinitions)
                    {
                        bool isPogon = isPogonazh(acBlock.ObjectId);
                        if (isPogon)
                        {
                            pogon.Add(new _lin(acBlock.ObjectId, acBlockName));
                         }
                        else
                        {
                            switch (acBlockName)
                            {
                                //Прямая арматура
                                case "Arm_zone_v001":
                                case "Arm_zone_v002":
                                case "Arm_wall_v002_2":
                                case "Arm_wall_v002_1":
                                case "Arm_unit_v001":
                                    lin.Add(new _lin(acBlock.ObjectId, acBlockName));
                                    break;
                                //Гнутые детали
                                case "Arm_zone_geshka_v002":
                                case "Arm_zone_geshka_v001":
                                case "Arm_zagagulina_v001":
                                case "Arm_homut_v002":
                                case "Arm_homut_v001":
                                    detal.Add(new _lin(acBlock.ObjectId, acBlockName));
                                    break;
                                
                                //Каркас
                                case "Arm_wall_v002_4":
                                case "Arm_wall_v002_3":
                                case "Arm_wall_v002_17":
                                    kr.Add(new _kr(acBlock.ObjectId, acBlockName));
                                    break;

                                //Каркас
                                case "Arm_zd":
                                    zd.Add(new _zd(acBlock.ObjectId, acBlockName));
                                    break;

                                default:
                                    //Console.WriteLine("Default case");
                                    break;

                            } // end switch
                        } // end if isPogon
                    } //end if HasAttributeDefinitions
                } //end if IsDynamicBlock

                                }
                            }
                        }

            }

                //Я в этой транзакции ничего не меняю, Все открываю только для чтения.
                //соответственно и вносить изменения не нужно.
                acTrans.Commit();
            }


            //тут начинаем формировать таблицу для вывода
            //Проходим по поганажу
            //List<_tablRow> pogonTab = new List<_tablRow>();

            pogonTab = new Dictionary<string, _tablRow>();
            detalTab = new Dictionary<string, _tablRow>();
            linTab = new Dictionary<string, _tablRow>();

            krTab = new Dictionary<string, _tablRow>();
            zdTab = new Dictionary<string, _tablRow>();
            

            foreach (_lin i in pogon)
            {
                int d = i.diameter;
                double l = i.length;
                int c = i.counte;

                _tablRow row = new _tablRow(1,0, d, 1, (int)Math.Ceiling(l * c));
                string key = d.ToString();
                if (pogonTab.ContainsKey(row.diameter.ToString()))
                {
                    pogonTab[key].AddCounte(row.counte);
                    pogonTab[key].AddObjID(i.id);
                }
                else
                {
                    pogonTab.Add(key, row);
                    pogonTab[key].AddObjID(i.id);
                }
            }


            foreach (_lin i in detal)
            {
                int d = i.diameter;
                double l = i.length;
                int c = i.counte;

                _tablRow row = new _tablRow(2, 0, d, l, c);
                string key = d + "_" + l + " " + i.length_1 + " " + i.length_2;
                if (detalTab.ContainsKey(key))
                {
                    detalTab[key].AddCounte(row.counte);
                    detalTab[key].AddObjID(i.id);

                }
                else
                {
                    detalTab.Add(key, row);
                    detalTab[key].AddObjID(i.id);
                }
            }


            foreach (_lin i in lin)
            {
                int d = i.diameter;
                double l = i.length;
                int c = i.counte;

                _tablRow row = new _tablRow(3, 0, d, l, c);
                string key = d + " " + l;
                if (linTab.ContainsKey(key))
                {
                    linTab[key].AddCounte(row.counte);
                    linTab[key].AddObjID(i.id);
                }
                else
                {
                    linTab.Add(key, row);
                    linTab[key].AddObjID(i.id);
                }
            }

            foreach (_kr i in kr)
            {
                int d = i.diameter;
                double l = i.length;
                int c = i.counte;

                _tablRow row = new _tablRow(4, 0, d, l, c);
                string key = d + "_" + l + " " + i.wall_width;
                if (krTab.ContainsKey(key))
                {
                    krTab[key].AddCounte(row.counte);
                    krTab[key].AddObjID(i.id);

                }
                else
                {
                    krTab.Add(key, row);
                    krTab[key].AddObjID(i.id);
                }
            }

            foreach (_zd i in zd)
            {
                //zdTab
                int d = 0;
                double l = 0;
                int c = 0;
                _tablRow row = new _tablRow(4, 0, d, l, c);

            }

            // приступаем к нумеровке
            acEd.WriteMessage("\nАрматура в п.м.:");
            int poz = 1; // начальная позиция

            var items = from pair in pogonTab
                        orderby pair.Value.diameter ascending
                        select pair;

            //foreach (KeyValuePair<string, _tablRow> i in pogonTab.OrderBy(i => i.Value.diameter))
            foreach (KeyValuePair<string, _tablRow> i in items)
            {
                pogonTab[i.Key].position = poz;
                foreach (Db.ObjectId j in pogonTab[i.Key].ObjIDList)
                {
                    Commands.SetAttrProperty(j, "ПОЗ", poz.ToString());
                }
                //Выплюнем в консль, то что получилось.
                acEd.WriteMessage("\n{0} - {1} - {2} - {3}",
                    pogonTab[i.Key].position,
                    pogonTab[i.Key].diameter,
                    pogonTab[i.Key].length,
                    pogonTab[i.Key].counte);

                poz = poz + 1;
            }


            acEd.WriteMessage("\nДетали:");

            items = from pair in detalTab
                        orderby pair.Value.diameter ascending, pair.Value.length ascending
                        select pair;

            //foreach (KeyValuePair<string, _tablRow> i in pogonTab.OrderBy(i => i.Value.diameter))
            foreach (KeyValuePair<string, _tablRow> i in items)
            {
                detalTab[i.Key].position = poz;
                foreach (Db.ObjectId j in detalTab[i.Key].ObjIDList)
                {
                    Commands.SetAttrProperty(j, "ПОЗ", poz.ToString());
                }
                //Выплюнем в консль, то что получилось.
                acEd.WriteMessage("\n{0} - {1} - {2} - {3}",
                    detalTab[i.Key].position,
                    detalTab[i.Key].diameter,
                    detalTab[i.Key].length,
                    detalTab[i.Key].counte);

                poz = poz + 1;
            }

            acEd.WriteMessage("\nАрматурные стержни:");

            items = from pair in linTab
                    orderby pair.Value.diameter ascending, pair.Value.length ascending
                    select pair;

            //foreach (KeyValuePair<string, _tablRow> i in pogonTab.OrderBy(i => i.Value.diameter))
            foreach (KeyValuePair<string, _tablRow> i in items)
            {
                linTab[i.Key].position = poz;
                foreach (Db.ObjectId j in linTab[i.Key].ObjIDList)
                {
                    Commands.SetAttrProperty(j, "ПОЗ", poz.ToString());
                }
                //Выплюнем в консль, то что получилось.
                acEd.WriteMessage("\n{0} - {1} - {2} - {3}",
                    linTab[i.Key].position,
                    linTab[i.Key].diameter,
                    linTab[i.Key].length,
                    linTab[i.Key].counte);

                poz = poz + 1;
            }



            acEd.WriteMessage("\nСборочные единицы");
            poz = 1; // начальная позиция

            items = from pair in krTab
                    orderby pair.Key
                        select pair;

            //foreach (KeyValuePair<string, _tablRow> i in pogonTab.OrderBy(i => i.Value.diameter))
            foreach (KeyValuePair<string, _tablRow> i in items)
            {
                krTab[i.Key].position = poz;

                foreach (Db.ObjectId j in krTab[i.Key].ObjIDList)
                {
                    Commands.SetAttrProperty(j, "ПОЗ", poz.ToString());
                }

                //Выплюнем в консль, то что получилось.
                acEd.WriteMessage("\n{0} - {1} - {2} - {3}",
                    krTab[i.Key].position,
                    krTab[i.Key].diameter,
                    krTab[i.Key].length,
                    krTab[i.Key].counte);   
                poz = poz + 1;
            }

            makepalette();




        } // end static public void bx_armsp

        public static List<Object> tablRowList()
        {
            List<Object> _tablRowList = new List<Object>();
                //_tablRowList.Add((object)"Арматура в п.м.:");
                _tablRowList.AddRange(pogonTab.Values.ToList());
               // _tablRowList.Add((object)"Детали:");
                _tablRowList.AddRange(detalTab.Values.ToList());
                //_tablRowList.Add((object)"Арматурные стержни:");
                _tablRowList.AddRange(linTab.Values.ToList());
                //_tablRowList.Add((object)"Сборочные единицы:");
                _tablRowList.AddRange(krTab.Values.ToList());
                _tablRowList.AddRange(zdTab.Values.ToList());
                return _tablRowList; 
        }


        private static void makepalette()
        {
            //_ps = null;
            ////тут выведим полученные таблицы в консоль
            //if (_ps == null)
            //{
               
                // Create the palette set

                _ps = new Win.PaletteSet("ArmSP");
                _ps.Size = new Size(400, 600);
                _ps.DockEnabled =
                  (Win.DockSides)((int)Win.DockSides.Left + (int)Win.DockSides.Right);

                // Create our first user control instance and
                // host it on a palette using AddVisual()


                //UserControl1 uc = new UserControl1();
                //_ps.AddVisual("AddVisual", uc);

                //uc.main_grig.ItemsSource = tablRowList();

                // Create our second user control instance and
                // host it in an ElementHost, which allows
                // interop between WinForms and WPF

                UserControl1 uc2 = new UserControl1();
                ElementHost host = new ElementHost();
                host.AutoSize = true;
                host.Dock = DockStyle.Fill;
                host.Child = uc2;
                _ps.Add("Add ElementHost", host);

            //}


            // Display our palette set
            //_ps.KeepFocus = true;
            _ps.Visible = true;

        }


        private static bool isPogonazh(Db.ObjectId objID)
        {

            string attr = GetAttrProperty(objID, "КОММЕНТ");
            double dl = GetDynamicProperty(objID, "Длина");

            //Костыли от старой ошибки в создании блока.
            if (dl == -1)
            {
                dl = GetDynamicProperty(objID, "Длинна");
            }


            if (dl >= length_planed_reinforcement | attr == ".")
            {
                return true;
                
            }
            return false;
        }


        public static double GetDynamicProperty(Db.ObjectId objID, string PropName)
        {

            // Получение текущего документа и базы данных
            App.Document acDoc = App.Application.DocumentManager.MdiActiveDocument;
            Db.Database acCurDb = acDoc.Database;

            using (Db.Transaction acTrans = acCurDb.TransactionManager.StartOpenCloseTransaction())
            {
                //Получаю выбранный блок
                Db.BlockReference acBlock = acTrans.GetObject(objID, Db.OpenMode.ForRead) as Db.BlockReference;
                //Получаю определение блока в таблице блоков
                Db.BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlock.BlockTableRecord, Db.OpenMode.ForRead)
                    as Db.BlockTableRecord;

                if (acBlock.IsDynamicBlock)
                {
                    Db.DynamicBlockReferencePropertyCollection acBlockDynProp =
                        acBlock.DynamicBlockReferencePropertyCollection;
                    if (acBlockDynProp != null)
                    {
                        foreach (Db.DynamicBlockReferenceProperty obj in acBlockDynProp)
                        {
                            if (obj.PropertyName.ToUpper() == PropName.ToUpper())
                            {
                                //double dl = Math.Round(Double.Parse(obj.Value.ToString()), 0);
                                return Math.Round(Double.Parse(obj.Value.ToString()), 0);
                            }
                        }
                    }
                }
                acTrans.Commit();
            }
            return -1;
        }


        public static string GetAttrProperty(Db.ObjectId objID, string PropName)
        {

            // Получение текущего документа и базы данных
            App.Document acDoc = App.Application.DocumentManager.MdiActiveDocument;
            Db.Database acCurDb = acDoc.Database;

            using (Db.Transaction acTrans = acCurDb.TransactionManager.StartOpenCloseTransaction())
            {
                //Получаю выбранный блок
                Db.BlockReference acBlock = acTrans.GetObject(objID, Db.OpenMode.ForRead) as Db.BlockReference;
                //Получаю определение блока в таблице блоков
                Db.BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlock.BlockTableRecord, Db.OpenMode.ForRead)
                    as Db.BlockTableRecord;

                if (acBlkTblRec.HasAttributeDefinitions)
                {

                    Db.AttributeCollection attrCol = acBlock.AttributeCollection;
                    if (attrCol.Count > 0)
                    {
                        foreach (Db.ObjectId AttID in attrCol)
                        {
                            Db.AttributeReference acAttRef = acTrans.GetObject(AttID,
                                                    Db.OpenMode.ForRead) as Db.AttributeReference;

                            if (acAttRef.Tag.ToUpper() == PropName.ToUpper())
                            {
                                return acAttRef.TextString;
                                // Otm_n = Double.Parse(acAttRef.TextString);
                                //Double.TryParse(acAttRef.TextString, Otm_n);
                                // Double.TryParse(acAttRef.TextString.Replace(',','.'), out Otm_n);

                            }
                        }
                    }
                }
                acTrans.Commit();
                return String.Empty;
            }
        }




        public static void SetAttrProperty(Db.ObjectId objID, string PropName, string value)
        {

            // Получение текущего документа и базы данных
            App.Document acDoc = App.Application.DocumentManager.MdiActiveDocument;
            Db.Database acCurDb = acDoc.Database;

            using (Db.Transaction acTrans = acCurDb.TransactionManager.StartOpenCloseTransaction())
            {
                //Получаю выбранный блок
                Db.BlockReference acBlock = acTrans.GetObject(objID, Db.OpenMode.ForRead) as Db.BlockReference;
                //Получаю определение блока в таблице блоков
                Db.BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlock.BlockTableRecord, Db.OpenMode.ForRead)
                    as Db.BlockTableRecord;

                if (acBlkTblRec.HasAttributeDefinitions)
                {

                    Db.AttributeCollection attrCol = acBlock.AttributeCollection;
                    if (attrCol.Count > 0)
                    {
                        foreach (Db.ObjectId AttID in attrCol)
                        {
                            Db.AttributeReference acAttRef = acTrans.GetObject(AttID,
                                                    Db.OpenMode.ForRead) as Db.AttributeReference;

                            if (acAttRef.Tag.ToUpper() == PropName.ToUpper())
                            {
                                acAttRef.UpgradeOpen();
                                acAttRef.TextString = value;
                                acAttRef.DowngradeOpen();
                                break;
                               
                            }
                        }
                    }
                }
                //return String.Empty;
                acTrans.Commit();
            }
            
        }

    }
}
