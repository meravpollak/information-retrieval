using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IR
{
   public class RankedDoc
    {
        private string m_name;
        private double m_rank;

        //constructor of wordsConnected
        public RankedDoc(string name, double location)
        {
            m_name = name;
            m_rank = location;
            // m_nameFile = nameFile;
            // m_count = 0;
        }
        // get the name of the doc
        public string NAME
        {
            get { return m_name; }
            set { m_name = value; }
        }
        // get the rank value
        public double RANK
        {
            get { return m_rank; }
            set { m_rank = value; }
        }



    }
}
