using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IR
{
    class Indexer
    {
        // do the index to the files
        public Indexer(ArrayList docs, string path_posting, String path_docs)
        {
            Dictionary<string, Term> Dic_Terms = new Dictionary<string, Term>(); // dictionary of terms
            Dictionary<string, int> Dic_For_Doc = new Dictionary<string, int>(); // dictionary of documents
            FileStream fs1 = new FileStream(path_docs, FileMode.Append);
            StreamWriter sw1 = new StreamWriter(fs1);

            int index = 0;
            int countUniq = 0;
            int countCommon = 0;
            string commonTerm = "NoCommon";

            foreach (Document d in docs)
            {
                while (index < d.DOC_LENGTH) // check if the term isnt ""&& d.LIST_TERMS[index] != ""
                {
                    string term = d.LIST_TERMS[index].ToLower();
                    Term new_term = new Term(term);
                    if (!Dic_Terms.ContainsKey(term))
                    {
                        Dic_Terms.Add(term, new_term); // add <DOC, [] >
                    }
                    Dic_Terms[term].TF++; // add to number of DF
                    if (!Dic_Terms[term].List_DOC.Contains(d.NAME))
                    {
                        Dic_Terms[term].DF++;
                    }
                    Dic_Terms[term].List_DOC.Add(d.NAME); // add to list of appearance

                    if (!Dic_Terms[new_term.NAME].DIC_APPERANCE.ContainsKey(d.NAME))
                    {
                        Dic_Terms[new_term.NAME].DIC_APPERANCE.Add(d.NAME, 1);
                    }
                    else
                    {
                        Dic_Terms[new_term.NAME].DIC_APPERANCE[d.NAME]++; // num of apperance
                    }

                    index++;

                    //check the number of times term apper in a doc
                    if (!Dic_For_Doc.ContainsKey(term))
                    {
                        Dic_For_Doc[term] = 1;
                    }
                    else
                    {
                        Dic_For_Doc[term]++;
                    }

                }

                //get the first 10 terms in the doc
                string start = "";
                start = d.LIST_TERMS[0].ToLower();
                for (int i = 1; i < 10; i++)
                {
                    start = start + "/" + d.LIST_TERMS[i].ToLower();

                }
                d.START10 = start;

                    d.COUNT_UNIQE = Dic_For_Doc.Count;
                //check the number of uniq word and the max_tf of the common word
                foreach (string term in Dic_For_Doc.Keys)
                {
                    if (Dic_For_Doc[term] == 1)
                    {
                        countUniq++;
                    }
                    else if (Dic_For_Doc[term] > countCommon)
                    {
                        countCommon = Dic_For_Doc[term];
                        commonTerm = term;
                    }
                }
                d.COUNT_UNIQE = countUniq;
                d.COUNT_COMMON_TERM = countCommon;
                d.COMMON_TERM = commonTerm;
                //write to the file of docs
                sw1.WriteLine(d.NAME + ";" + d.LIST_TERMS.Length / 2 + ";" + d.LANGUAGE + ";" + d.COUNT_COMMON_TERM + ";" + d.COUNT_UNIQE + ";" + d.COMMON_TERM + ";" + d.TITLE.ToLower() + ";" + d.START10);

                countUniq = 0;
                countCommon = 0;
                index = 0;
                Dic_For_Doc = new Dictionary<string, int>();
            }

            sw1.Close();
            fs1.Close();

            Dictionary<string, Term> Dic_Terms_new = new Dictionary<string, Term>();

            foreach (var item in Dic_Terms.OrderBy(i => i.Key))
            {
                Dic_Terms_new[item.Key] = item.Value;
            }

            RemoveUnecceceryTabs(Dic_Terms_new);

            FileStream fs = new FileStream(path_posting, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            //write to the file the DOC
            foreach (string term in Dic_Terms_new.Keys)
            {
                string doc = "";

                foreach (string t in Dic_Terms[term].DIC_APPERANCE.Keys)
                {
                    doc = doc + t + "," + Dic_Terms[term].DIC_APPERANCE[t] + ";";
                }
                sw.WriteLine(term + "/" + Dic_Terms_new[term].DF + "/" + Dic_Terms_new[term].TF + "/" + doc);
            }
            sw.Close();
            fs.Close();
        }

        //remove the unecceryTabs
        public void RemoveUnecceceryTabs(Dictionary<string, Term> Dic_Terms)
        {
            if (Dic_Terms.ContainsKey(""))
            {
                Dic_Terms.Remove("");
            }
            if (Dic_Terms.ContainsKey("$"))
            {
                Dic_Terms.Remove("$");
            }
            if (Dic_Terms.ContainsKey("-"))
            {
                Dic_Terms.Remove("-");
            }
            if (Dic_Terms.ContainsKey("/"))
            {
                Dic_Terms.Remove("/");
            }
            if (Dic_Terms.ContainsKey("\""))
            {
                Dic_Terms.Remove("\"");
            }
            if (Dic_Terms.ContainsKey("--"))
            {
                Dic_Terms.Remove("--");
            }
            if (Dic_Terms.ContainsKey("+"))
            {
                Dic_Terms.Remove("+");
            }
            if (Dic_Terms.ContainsKey("<"))
            {
                Dic_Terms.Remove("<");
            }

            if (Dic_Terms.ContainsKey(">"))
            {
                Dic_Terms.Remove(">");
            }
            if (Dic_Terms.ContainsKey("="))
            {
                Dic_Terms.Remove("=");
            }

            if (Dic_Terms.ContainsKey("#"))
            {
                Dic_Terms.Remove("#");
            }

            if (Dic_Terms.ContainsKey("##"))
            {
                Dic_Terms.Remove("##");
            }
        }

    }
}
