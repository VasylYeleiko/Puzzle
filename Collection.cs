using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Puzzle
{
    [Serializable]
    public class MyCollection
    {
        public static ObservableCollection<Puzz> collection = new ObservableCollection<Puzz>();

        public MyCollection() { }


        public static void Save(Puzz item)
        {
            var check = true;
            foreach(var i in collection)
            {
                if (i.path == item.path) check = false;
            }

            if (check)
            {
                collection.Add(item);
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Puzz>));
                using (FileStream s = new FileStream("PuzzleCollectionSave.xml", FileMode.OpenOrCreate))
                {
                    serializer.Serialize(s, collection);
                }
            }
        }

        public static ObservableCollection<Puzz> GetCollection()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Puzz>));
            FileInfo fileInfo = new FileInfo("PuzzleCollectionSave.xml");
            if (fileInfo.Exists)
            {
                using (FileStream s = new FileStream("PuzzleCollectionSave.xml", FileMode.Open))
                {
                    return collection = (ObservableCollection<Puzz>)serializer.Deserialize(s);
                }
            }
            else return null;

        }
    }
}
