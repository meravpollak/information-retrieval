using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LAIR;
using LAIR.ResourceAPIs.WordNet;///////////////////////////////////////////////////////////////
using LAIR.Collections.Generic;////////////////////
using System.Collections;
using System.Threading;

namespace Project_IR
{
    public class Ranker
    {
       // Dictionary<string, double> Dic_Docmunets_Ranked_BM25;
        Dictionary<int, string[]> m_query_list;
        Dictionary<string, Term> m_terms_dictionary;
        Dictionary<string, Document> m_doc_dic;
        int m_average_lenght_doc;
        string m_path_posting;
        int num_query;
        Dictionary<string, RankedDoc> m_Top50_ranked_BM25;
        Dictionary<string, LinkedList<WordsConnected>> m_query_in_RelevantDoc;
        Dictionary<string, LinkedList<WordsConnected>> m_Semantic_query;
        double query_length;
        ArrayList m_list_languages_pressed;
        Dictionary<string, int> m_words_in_query;
        string m_path_queries_ranked;
        Dictionary<string, double> dic_top50;
        Dictionary<string, int> doc_stasrt10;

        // constructor of the ranker
        public Ranker(Dictionary<string, Term> terms_dictionary, Dictionary<string, Document> doc_dic, Dictionary<int, string[]> query_list, int average_lenght_doc, bool Dostemming, string path_posting, ArrayList list_languages_pressed, string path_queries_ranked)
        {
            m_list_languages_pressed = list_languages_pressed;
            int query_length = 0;
            num_query=0;
            m_query_list = query_list;
            m_terms_dictionary=terms_dictionary;
            m_doc_dic = doc_dic;
            m_average_lenght_doc = average_lenght_doc;
            m_path_posting = path_posting;
            m_Top50_ranked_BM25 = new Dictionary<string, RankedDoc>();
            m_query_in_RelevantDoc = new Dictionary<string, LinkedList<WordsConnected>>();
            m_Semantic_query = new Dictionary<string, LinkedList<WordsConnected>>();
            m_words_in_query = new Dictionary<string, int>();
            m_path_queries_ranked = path_queries_ranked;
            dic_top50 = new Dictionary<string, double>();
            doc_stasrt10 = new Dictionary<string, int>();
            foreach (int num_q in query_list.Keys)
            {
                query_length = query_list[num_q].Length;
                num_query = num_q;
            }
            string[] words_Query_f = new string[query_length];
            words_Query_f = query_list[num_query];
            Array.Sort(words_Query_f);////////////////////////////////
            ArrayList a_list = new ArrayList();
            // sort and check if there isnt twice of this word
            for (int i = 0; i < words_Query_f.Length; i++)
            {
                if (!a_list.Contains(words_Query_f[i]))
                    a_list.Add(words_Query_f[i].ToLower());
            }
            // convert to array back
            string[] final_array_query = (string[])a_list.ToArray(typeof(string));
            //update the array_query_list
            m_query_list[num_query] = final_array_query;

            // find semantic words of each word in the query
            Dictionary<int, string[]> Semantic_Words = GetSemanticFromQuery(m_query_list);

            // ranked the documents
            m_query_in_RelevantDoc = getTheDocument(query_list, Dostemming, path_posting);

            //calculate the cosSim
            Dictionary<string, double> dic_CosSim = calculatingDocumentsCosSim();
            Dictionary<string, double> dic_BM25 = calculatingDocumentsBM25(m_query_in_RelevantDoc);
            // calculate the start 10
            getStart10();

            m_words_in_query = new Dictionary<string, int>();

            // ranked the semantic words
            m_Semantic_query = getTheDocument(Semantic_Words, Dostemming, path_posting);

            //calculate the final ranking
            dic_top50 = calcualteFinalRanking(GetTheRelevantDoc_Title(query_list), dic_BM25, calculatingDocumentsBM25(m_Semantic_query), dic_CosSim);

            //new Thread(delegate()
            //{
                WriteToFile_rankedDoc(path_queries_ranked, dic_top50);
         //   }).Start();
                        
        }

        // get over all the words in the query and get the semantic words for them
        private Dictionary<int, string[]> GetSemanticFromQuery(Dictionary<int, string[]> query_list)
        {

            string[] list_words_query = new string[1];
            Dictionary<int, string []>  final_semantic_words = new Dictionary<int, string []>();
            foreach (int num_q in query_list.Keys)
            {
                list_words_query = query_list[num_q];
            }           
            for (int i = 0; i < list_words_query.Length; i++)
            {
                // get the semanticWords for a word in the query
                Dictionary<int, string[]> temp_semanticWords = GetSemantic(list_words_query[i]);
                // add to the final semantic_words
                foreach (var t in temp_semanticWords.Keys)
                {
                    string[] temp = temp_semanticWords[t];
                    final_semantic_words.Add(i,temp);
                }             
               
            }

            return final_semantic_words;
        }
        // get the semantic words
        private Dictionary<int,string []> GetSemantic(string word)
        {
            ArrayList Semantic_words = new ArrayList();
            WordNetEngine wn = new WordNetEngine(@"../../", false);
            char[] delimiters = { ' ', ',' };
            // get the nouns
            int count = 0;
            Set<SynSet> syns_noun = wn.GetSynSets(word, WordNetEngine.POS.Noun);
            foreach (SynSet syn in syns_noun)
            {
                string syn_s = syn.ToString();
                int index = syn_s.IndexOf(":");
                syn_s = syn_s.Substring(0, index);// remove until the ":"
                syn_s = syn_s.Substring(1, syn_s.Length - 2);
                string[] words = syn_s.Split(delimiters);
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i] != "" && !Semantic_words.Contains(words[i]))
                    {
                        if (count<2)
                        {
                            Semantic_words.Add(words[i]);
                            count++;
                        }
                       
                    }
                }
            }

            // get the verb
            Set<SynSet> syns_verb = wn.GetSynSets(word, WordNetEngine.POS.Verb);
            foreach (SynSet syn in syns_verb)
            {
                string syn_s = syn.ToString();
                int index = syn_s.IndexOf(":");
                syn_s = syn_s.Substring(0, index);// remove until the ":"
                syn_s = syn_s.Substring(1, syn_s.Length - 2);
                string[] words = syn_s.Split(delimiters);
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i] != "" && !Semantic_words.Contains(words[i]))
                    {
                        if (count < 4)
                        {
                            Semantic_words.Add(words[i]);
                           count++;
                        }
                    }
                }
            }

            string[] Semantic_final = (string[])Semantic_words.ToArray(typeof(string));
            Dictionary<int, string[]> Semantic_dic = new Dictionary<int, string[]>();
            Semantic_dic.Add(900, Semantic_final);
            return Semantic_dic;
        }

        // LInked list of - [ for each document : words + count ]
        public Dictionary<string, LinkedList<WordsConnected>> getTheDocument(Dictionary<int, string[]> query_list, bool Dostemming, string path_posting)
        {
            //number of doc + average of the length of all doc 
            Dictionary<string, int> words_in_query = new Dictionary<string, int>();
            int number_of_doc = m_doc_dic.Count;
            double average = m_average_lenght_doc / number_of_doc;
            Dictionary<int, List<string>> list_of_relevet = new Dictionary<int, List<string>>();
            Dictionary<string, LinkedList<WordsConnected>> doc_to_calculate = new Dictionary<string, LinkedList<WordsConnected>>();

            FileStream fs1, fs2;
            if (Dostemming == true)/////////////////////////////////////////////
            {
                fs1 = new FileStream(path_posting + "/DictionaryStemming", FileMode.Open, FileAccess.Read);
                fs2 = new FileStream(path_posting + "/PostingFileStemming", FileMode.Open, FileAccess.Read);

            }
            else
            {
                fs1 = new FileStream(path_posting + "/Dictionary", FileMode.Open, FileAccess.Read);
                fs2 = new FileStream(path_posting + "/PostingFile", FileMode.Open, FileAccess.Read);

            }
            StreamReader sr1 = new StreamReader(fs1);//read dictionary
            StreamReader sr2 = new StreamReader(fs2);//read posting 
            //get the parameters to the BM25
            foreach (int query_number in query_list.Keys)
            {
                string[] wordsQuery = query_list[query_number];
                int length_query = wordsQuery.Length;
                Array.Sort(wordsQuery); // check if sort //////////////////////////////

                //count the times of each word in the query
                for (int i = 0; i < wordsQuery.Length; i++)
                {
                    if (!words_in_query.ContainsKey(wordsQuery[i]))
                    {
                        words_in_query[(wordsQuery[i])] = 1;
                    }
                    else
                    {
                        words_in_query[(wordsQuery[i])]++;
                    }
                }

                m_words_in_query = words_in_query;
                foreach (string word in words_in_query.Keys)
                {
                    //int qfi = words_in_query[word];
                    //int word_df = dic_to_use[word].DF;


                    //find the posting of the word
                    string line1 = sr1.ReadLine();//read dictionary
                    string line2 = sr2.ReadLine(); //read posting 
                    int index;
                    string posting;

                    while (line1 != null && line2 != null)
                    {

                        index = line1.IndexOf("/");
                        line1 = line1.Substring(0, index);

                        //find the word
                        if (line1 == word)
                        {
                            posting = line2;
                            while (posting.Contains(";"))
                            {
                                index = posting.IndexOf(",");
                                string doc_number = posting.Substring(0, index);
                                posting = posting.Substring(index + 1);
                                index = posting.IndexOf(";");
                                string appear_number = posting.Substring(0, index);

                                int i3;
                                int appear = 0;
                                if (Int32.TryParse(appear_number, out i3))
                                {
                                    appear = i3;
                                }
                                posting = posting.Substring(index + 1);
                                WordsConnected to_add = new WordsConnected(word, appear);
                                // check if the key alreday exist

                                if (doc_to_calculate.ContainsKey(doc_number))
                                {                                   
                                        doc_to_calculate[doc_number].AddFirst(to_add); 
                                }
                                else // not exist
                                {
                                    if (m_list_languages_pressed.Contains(m_doc_dic[doc_number].LANGUAGE.ToLower()) || m_list_languages_pressed.Count == 0)
                                    {
                                        doc_to_calculate.Add(doc_number, new LinkedList<WordsConnected>());
                                        doc_to_calculate[doc_number].AddFirst(to_add); 
                                    }                                   
                                }
                               
                            }

                            line1 = null;
                            line2 = null;

                        }
                        else
                        {
                            line1 = sr1.ReadLine();
                            line2 = sr2.ReadLine();
                        }
                    }
                    // reset the postion of the line to the start
                    sr1.Close();
                    sr2.Close();
                    fs1.Close();
                    fs2.Close();

                    if (Dostemming == true)/////////////////////////////////////////////
                    {
                        fs1 = new FileStream(path_posting + "/DictionaryStemming", FileMode.Open, FileAccess.Read);
                        fs2 = new FileStream(path_posting + "/PostingFileStemming", FileMode.Open, FileAccess.Read);

                    }
                    else
                    {
                        fs1 = new FileStream(path_posting + "/Dictionary", FileMode.Open, FileAccess.Read);
                        fs2 = new FileStream(path_posting + "/PostingFile", FileMode.Open, FileAccess.Read);

                    }
                    sr1 = new StreamReader(fs1);
                    sr2 = new StreamReader(fs2);
             
                }

            }

            sr1.Close();
            sr2.Close();
            fs1.Close();
            fs2.Close();

       //     Dictionary<string, double> doc_rank = new Dictionary<string, double>();
         //   Dictionary<string, double> doc_rank_sim = new Dictionary<string, double>();


           // m_query_in_RelevantDoc = doc_to_calculate; <= was before
            return doc_to_calculate;


           // doc_rank_sim = calculatingDocumentsCosSim();
           //doc_rank = calculatingDocumentsBM25(average_lenght_doc);

          //  return doc_rank;
        }

        // rank for eachDoc
        private Dictionary<string, double> calculatingDocumentsBM25(Dictionary<string, LinkedList<WordsConnected>> dic)
        {

            Dictionary<string, double> array_doc = new Dictionary<string, double>();
            double average_doc = m_average_lenght_doc / m_doc_dic.Count;
            int numberOfDoc = m_doc_dic.Count;


            foreach (string doc_name in dic.Keys)
            {
                LinkedList<WordsConnected> Word_D = dic[doc_name];
                int countWord = 0;
                double doc_final = 0;
                double K = 0;
                double k1 = 1.1;
                double b = 0;
                double k2 = 1000;

                K = k1 * ((1 - b) + b * (m_doc_dic[doc_name].DOC_LENGTH));
                double part1 =0;
                double part2=0; 
                double part3 = 0;
         
                while (countWord < Word_D.Count)
                {   
                    WordsConnected first = Word_D.First();
                    Word_D.RemoveFirst();
                    part1 = (0.5 / 0.5) / ((m_terms_dictionary[first.NAME].DF + 0.5) / (numberOfDoc - m_terms_dictionary[first.NAME].DF + 0.5));
                    part1 = Math.Log(part1);
                    part2 = ((k1 + 1) * first.LOCATION) / (K + first.LOCATION);
                    part3 = ((k2 + 1) * m_words_in_query[first.NAME]) / (k2 + m_words_in_query[first.NAME]);
                    doc_final = doc_final + (part1 * part2 * part3);
                    part1 = 0;
                    part2 = 0;
                    part3 = 0;
                    countWord++;
                    Word_D.AddLast(first);
                }

                array_doc.Add(doc_name, doc_final);
                doc_final = 0;
            }

            return array_doc;
        }
      
       
        //check for the relvant in the Title of the Doc
        private Dictionary<string, int> GetTheRelevantDoc_Title(Dictionary<int, string[]> query_list)
        {
            Dictionary<string, int> query_in_Title = new Dictionary<string, int>();

            ArrayList query = new ArrayList();

            foreach (int num in query_list.Keys)
            {
                query.AddRange(query_list[num]);
            }

            Dictionary<string, LinkedList<WordsConnected>> temp_relevantDoc= m_query_in_RelevantDoc;
            // get over all the documents that have the query and check for the title
            foreach (string doc in temp_relevantDoc.Keys)
            {            
         
                // check if the query exist
                foreach (string word in query)
                {
                    string title = m_doc_dic[doc].TITLE; // get the title
                    //check if the title contains the word
                    if (title.Contains(word))
                    {
                        //count for the relevant words
                        if (query_in_Title.ContainsKey(doc))
                        {
                            query_in_Title[doc]++;
                        }
                        // this is the first one
                        else
                            query_in_Title.Add(doc, 1);
                    }

                }
               
            }
            return query_in_Title;           
        }

       // CosSim
        private Dictionary<string, double> calculatingDocumentsCosSim()
        { 
            Dictionary<string,double> array_doc = new Dictionary<string,double>();
            Dictionary<string, double> idf_words  = new Dictionary<string, double>();
            double wcalcu = 0;
            int number_doc = m_doc_dic.Count();

            foreach(string word in m_words_in_query.Keys)
            {
                if (m_terms_dictionary.ContainsKey(word))
                {
                    wcalcu = number_doc / m_terms_dictionary[word].DF;
                    if (!idf_words.ContainsKey(word))
                    {
                        idf_words[word] = wcalcu;
                    }
                }
            }

            double cosSimD = 0;
            foreach (string doc in m_query_in_RelevantDoc.Keys)
            {
                LinkedList<WordsConnected> Word_D = m_query_in_RelevantDoc[doc];
                int countWord = 0;

                while (countWord < Word_D.Count)
                {
                    WordsConnected first = Word_D.First();
                    Word_D.RemoveFirst();
                    cosSimD = cosSimD + idf_words[first.NAME];
                    Word_D.AddLast(first);
                    countWord++;
                }
                cosSimD = cosSimD / ((m_words_in_query.Count) * (m_doc_dic[doc].DOC_LENGTH));
              //  cosSimD = cosSimD / (Math.Sqrt(m_words_in_query.Count) * Math.Sqrt(m_doc_dic[doc].DOC_LENGTH));
                array_doc.Add(doc, cosSimD);
                cosSimD = 0;
            }
            return array_doc;
        }
   
        
        //calculate the ranking for the 50top
        private Dictionary<string, double> calcualteFinalRanking(Dictionary<string, int> Doc_Title_Ranking, Dictionary<string, double> Doc_BM25_Top50, Dictionary<string, double> Doc_Semantic,Dictionary<string, double> cosSim )
        {
            Dictionary<string, double> final_ranked_doc = new Dictionary<string, double>();
            double Bm25=0;
            double title;
            double cosSim_c = 0;
            double ranking;
            double m_semantic = 0;
            int m_start10 = 0;
            foreach (string doc in Doc_BM25_Top50.Keys)
            {
                Bm25 = Doc_BM25_Top50[doc];
                cosSim_c = cosSim[doc];
                if (!Doc_Title_Ranking.ContainsKey(doc))
                    title = 0;
                else
                {
                    double mone = ((double)Doc_Title_Ranking[doc]);
                    double mechane = ((double)m_words_in_query.Count);
                    title = mone / mechane;
                    if (mone >= 2)
                    {
                        title = 10;
                    }
                }

                if (Doc_Semantic.ContainsKey(doc))
                {
                    m_semantic = Doc_Semantic[doc];
                }

                if (doc_stasrt10.ContainsKey(doc))
                {
                    m_start10 = doc_stasrt10[doc];
                }
                //calculate the 
                ranking = title * 0.1 + Bm25 * 0.8 + 0.1 * cosSim_c + 0 * m_semantic + 0.2*m_start10;////0.1 0.8 0.1 0 0.2
                final_ranked_doc.Add(doc, ranking);
                title = 0;
            }


            Dictionary<string, double> Dic_top_50 = new Dictionary<string, double>();

            //sort the dicinary by the ranking. and return the best 50
            int count = 0;
            foreach (var rankedDoc in final_ranked_doc.OrderByDescending(p => p.Value))
            {
                if (count < 50)
                {
                    Dic_top_50.Add(rankedDoc.Key, rankedDoc.Value);
                    count++;
                }
            }

            return Dic_top_50;
        }

        // write the ranked to file
        private void WriteToFile_rankedDoc(string path_ranked_file, Dictionary<string, double> relevant_doc)
        {          
                FileStream FileS = new FileStream(path_ranked_file, FileMode.Append);
                StreamWriter sw = new StreamWriter(FileS);
                foreach (string doc in relevant_doc.Keys)
                {
                    sw.WriteLine(num_query + " 0 " + doc + " " + "1 " + "2.5 mt");// => 11 0 FBIS3-1008 0 2.5 [the last one is floating number]
                }
                sw.Close();
                FileS.Close();                
        }

        //get the arraylist of docs
        public Dictionary<string, double> TOP50DOCS
        {
            get { return dic_top50; }
        }

        // get the query number
        public int QUERY_NUM
        {
            get { return num_query; }
        }
      
        //get the top 10
        public void getStart10()
        {               
             Dictionary<string,int> start10=new Dictionary<string,int>(); 
             foreach (string doc in m_query_in_RelevantDoc.Keys)
            {
                start10.Add(doc, 0);
                string top10 = m_doc_dic[doc].START10;
                foreach (string words in m_words_in_query.Keys)
                {
                    if (top10.Contains(words))
                    {
                        start10[doc]++;
                    }
                }
                
            }

             doc_stasrt10 = start10;
        }




    } 
   
}
