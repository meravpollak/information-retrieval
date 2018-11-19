using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IR
{
    //ArrayConnectedwords
    class ArrayConnectedwords
    {
        private string m_name; // the name of the words
        private Dictionary<string, int> m_dic; // the dictionary of the word

        //constructor of ArrayConnectedwords
        public ArrayConnectedwords(string name)
        {
            m_name = name;
            m_dic = new Dictionary<string, int>();
        }

        //get the name
        public string NAME
        {
            get { return m_name; }
            set { m_name = value; }
        }

        //get the dicionary
        public Dictionary<string, int> DICTIONARY
        {
            get { return m_dic; }
            set { m_dic = value; }
        }
    }
}
