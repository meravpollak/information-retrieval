using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IR
{
    public class Document
    {
        private string m_name; // name of the document
        private string[] m_list_of_term;// all the terms in the doc.
        private int m_CountCommonTerm; // the count of the common term
        private string m_language; // the language of the documents
        private int m_count_unique; // the count of the unique term
        private int m_doc_length; // the doc length
        private string m_CommonTerm; // the most common term in the doc
        private string m_title; // title of the doc
        string m_start10;//10 words in the begining 
        /// <summary>
        /// constructor of the document details aboue the docmunt
        /// </summary>
        /// <param name="name"> name of the documnet</param>
        /// <param name="list_terms">list of the terms</param>
        /// <param name="doc_lenght">length of the document</param>
        /// <param name="language">the language of the doc</param>
        public Document(string name, string[] list_terms, int doc_lenght, string language, string title)
        {
            m_name = name;
            m_list_of_term = list_terms;
            m_CountCommonTerm = 0;
            m_doc_length = doc_lenght;
            m_language = language;
            m_count_unique = 0;
            m_CommonTerm = "NoCommon";
            m_title = title;
            m_start10 = "";

        }
        //the name of the doc
        public string NAME
        {
            get { return m_name; }
            set { m_name = value; }
        }
        //list of the terms
        public string[] LIST_TERMS
        {
            get { return m_list_of_term; }
            set { m_list_of_term = value; }
        }
        // the common term
        public string COMMON_TERM
        {
            get { return m_CommonTerm; }
            set { m_CommonTerm = value; }
        }
        //the count of the common term
        public int COUNT_COMMON_TERM
        {
            get { return m_CountCommonTerm; }
            set { m_CountCommonTerm = value; }
        }
        // the loength of the doc
        public int DOC_LENGTH
        {
            get { return m_doc_length; }
            set { m_doc_length = value; }
        }
        //the language of the doc
        public string LANGUAGE
        {
            get { return m_language; }
            set { m_language = value; }
        }
        // count of the unique term
        public int COUNT_UNIQE
        {
            get { return m_count_unique; }
            set { m_count_unique = value; }
        }

        //get the title of the term
        public string TITLE
        {
            get { return m_title; }
            set { m_title = value; }
        }
        //the 10 word in the start
        public string START10
        {
            get { return m_start10; }
            set { m_start10 = value; }
        }
    }
}
