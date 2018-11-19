using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IR
{
   public class WordsConnected
    {
        private string m_name;
        private int m_location;
        //private int m_count;
        //private string m_nameFile;

       //constructor of wordsConnected
        public WordsConnected(string name, int location)
        {
            m_name = name;
            m_location = location;
            // m_nameFile = nameFile;
            // m_count = 0;
        }
       // get the name
        public string NAME
        {
            get { return m_name; }
            set { m_name = value; }
        }
       // get the location
        public int LOCATION
        {
            get { return m_location; }
            set { m_location = value; }
        }
        //public int COUNT
        //{
        //    get { return m_count; }
        //    set { m_count = value; }
        //}

        //public string NAMEFILE
        //{
        //    get { return m_nameFile; }
        //    set { m_nameFile = value; }
        //}

    }
}
