using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IR
{
    class Searcher
    {
        Dictionary<int, string[]> m_query; // the query
        string m_path_stopWords;
        string m_path_corpus;
        string m_path_docs;
        ArrayList m_languages_pressed;
        Dictionary<string, Document> m_doc_dic;
        Dictionary<string, Term> m_terms_dictionary;
        ArrayList m_relevant_docs;
        int m_num_query;
        // constructor of the searcher
        public Searcher(Dictionary<string, Term> terms_dictionary, Dictionary<int, string[]> queries, ArrayList list_languages_pressed, int total_lenght_doc, Dictionary<string, Document> doc_dic, string path_stopWords, string path_newFile, string path_docs, bool Dostemming, bool Doindexer, string path_wordsConnected, string path_queries_ranked)
        {
            m_relevant_docs = new ArrayList();
            m_num_query = 0;
            m_query = queries;
            m_path_stopWords = path_stopWords;
            m_path_corpus = path_newFile;
            m_path_docs = path_docs;
            m_languages_pressed = list_languages_pressed;
            m_doc_dic = doc_dic;
            m_terms_dictionary = terms_dictionary;
            try
            {
                SendToParser(Dostemming, total_lenght_doc, list_languages_pressed, path_queries_ranked);//sending to parse
            }
            catch (Exception e2)
            {
                System.Windows.MessageBox.Show(e2.Message);
            }
        }

        //sending the query to parsing
        private void SendToParser(bool Dostemming, int total_lenght_doc, ArrayList list_languages_pressed, string path_queries_ranked)
        {           
            Parse p = new Parse();
            p.parse(m_query, m_path_stopWords, m_path_corpus, m_path_docs, Dostemming, false, "", total_lenght_doc, m_doc_dic, m_terms_dictionary, list_languages_pressed, path_queries_ranked);
            m_relevant_docs = p.RELDOCS;
            m_num_query = p.QUERY_NUMBER;
        }

        //return the arraylist of relevant docs
        public ArrayList REL_DOCS
        {
            get { return m_relevant_docs; }
        }

        //return the number of the query
        public int NUM_QUERY
        {
            get { return m_num_query; }
        }
    }
}
