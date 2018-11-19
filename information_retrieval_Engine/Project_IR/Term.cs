using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IR
{
    public class Term
    {
        private string m_name; // name of the term   
        private int m_df; // the number documents that the term appear in.
        private int m_tf; // number of shows in all the Docs.
        ArrayList m_list_docs; // the Docs that the term appear in.
        Dictionary<string, int> m_dic_apperance; // dicionary of dic appear in
        // the constructor of the term
        public Term(string name)
        {
            m_name = name;
            m_tf = 0;
            m_list_docs = new ArrayList();
            m_df = 0;
            m_dic_apperance = new Dictionary<string, int>();
        }
        // the name of the term
        public string NAME
        {
            get { return m_name; }
            set { m_name = value; }
        }
        // the TF of the term
        public int TF
        {
            get { return m_tf; }
            set { m_tf = value; }
        }

        //the list of the doc that the term appear in 
        public ArrayList List_DOC
        {
            get { return m_list_docs; }
            set { m_list_docs = value; }
        }

        //the DF of the term
        public int DF
        {
            get { return m_df; }
            set { m_df = value; }
        }

        //get the dic apearce
        public Dictionary<string, int> DIC_APPERANCE
        {
            get { return m_dic_apperance; }
            set { m_dic_apperance = value; }
        }

    }
}
