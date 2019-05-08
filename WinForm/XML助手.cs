using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace XMLEditer
{
    class XMLHelper
    {
        static string Path = "DataBase.xml";
        static XDocument xdocument = XDocument.Load(Path);

        public static void CreateXmlFile()
        {
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), CreateXElement());
            xdoc.Save(Path);
        }

        private static XElement CreateXElement()
        {
            XElement root = new XElement("Root");
            return root;
        }

        public static List<Ticket> ReSearch(int ID)
        {
            List<Ticket> ObjList = new List<Ticket>();
            if (ID == 0)
            {
                var Users = from userInfo in xdocument.Element("Root").Elements() select new {
                    ID = userInfo.Element("UserID").Value,
                    Total = userInfo.Element("Total").Value ,
                    Day = userInfo.Element("Day").Value };
                foreach (var item in Users)
                {
                    Ticket Obj = new Ticket(Convert.ToInt16(item.ID), Convert.ToDouble(item.Total), Convert.ToDateTime(item.Day));
                    ObjList.Add(Obj);
                }
            }
            else
            {
                var UserForWhere = from userInfo in xdocument.Element("Root").Elements() where Convert.ToInt32(userInfo.Element("UserID").Value) == ID select new {
                    ID = userInfo.Element("UserID").Value,
                    Total = userInfo.Element("Total").Value,
                    Day = userInfo.Element("Day").Value
                };
                foreach (var item in UserForWhere)
                {
                    Ticket Obj = new Ticket(Convert.ToInt16(item.ID), Convert.ToDouble(item.Total), Convert.ToDateTime(item.Day));
                    ObjList.Add(Obj);
                }
            }
            return ObjList;
        }

        public static List<Ticket> ReSearch(DateTime Day)
        {
            List<Ticket> ObjList = new List<Ticket>();
            var UserForWhere = from userInfo in xdocument.Element("Root").Elements()
                               where Convert.ToDateTime(userInfo.Element("Day").Value) == Day
                               select new
                               {
                                   ID = userInfo.Element("UserID").Value,
                                   Total = userInfo.Element("Total").Value,
                                   Day = userInfo.Element("Day").Value
                               };
            foreach (var item in UserForWhere)
            {
                Ticket Obj = new Ticket(Convert.ToInt16(item.ID), Convert.ToDouble(item.Total), Convert.ToDateTime(item.Day));
                ObjList.Add(Obj);
            }
            return ObjList;
        }

        public static void Insert(Ticket Obj)
        {
            XElement InsertRoot = new XElement("ID", new XElement("UserID", Obj.Id), new XElement("Total", Obj.Total), new XElement("Day", Obj.Day));
            xdocument.Element("Root").Add(InsertRoot);
            xdocument.Save(Path);
        }

        public static void Change(int ID,Ticket Obj)
        {
            XElement UserUpdate = (from userInfo in xdocument.Element("Root").Elements() where Convert.ToInt32(userInfo.Element("UserID").Value) == ID select userInfo).SingleOrDefault();
            if (UserUpdate != null)
            {
                UserUpdate.Element("UserID").Value = Obj.Id.ToString();
                UserUpdate.Element("Total").Value = Obj.Total.ToString();
                UserUpdate.Element("Day").Value = Obj.Day.ToString();
                xdocument.Save(Path);
            }
        }

        public static void Del(int ID)
        {
            XElement UserDelete = (from userInfo in xdocument.Element("Root").Elements() where Convert.ToInt32(userInfo.Element("UserID").Value) == ID select userInfo).SingleOrDefault();
            if (UserDelete != null)
            {
                UserDelete.Remove();
                xdocument.Save(Path);
            }
        }
    }
}
