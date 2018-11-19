using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IR
{
    class MergeConnectedWords
    {

        // merge the connected words
        public Dictionary<string,ArrayList> MergeSort(string path_SaveFiles, string path_file3, string file4, bool DoStemming)
        {
            int lastFile = MergeInEnd(path_file3); // get the last file that was merged
            MergeLinesAndWrite(path_file3, path_SaveFiles, lastFile, DoStemming); // check the lines and move to the savingFIles filder
            return getTheDic(path_SaveFiles, DoStemming);
        }
        // merge to one file
        public int MergeInEnd(string path_file)
        {
            int num1 = 1;
            int num2 = 2;
            int count = Directory.GetFiles(path_file).Length;
            int num3 = count + 1; // 25
            // <50
            while (Directory.GetFiles(path_file).Length != 1)
            {
                Merge(path_file, num1, num2, num3);
                num1 = num1 + 2;
                num2 = num2 + 2;
                num3++;
            }
            return (num3 - 1);
        }

        //merge the files
        public void Merge(string path_file, int num1, int num2, int num3)
        {
            FileStream fs1 = new FileStream(path_file + "/" + num1.ToString(), FileMode.Open, FileAccess.Read);
            StreamReader sr1 = new StreamReader(fs1);
            FileStream fs2 = new FileStream(path_file + "/" + num2.ToString(), FileMode.Open, FileAccess.Read);
            StreamReader sr2 = new StreamReader(fs2);
            FileStream fs3 = new FileStream(path_file + "/" + num3.ToString(), FileMode.Create);
            StreamWriter sw3 = new StreamWriter(fs3);
            string line1 = "";
            string line2 = "";
            line1 = sr1.ReadLine();
            line2 = sr2.ReadLine();
            while (line1 != null && line2 != null)
            {
                string st1 = line1.Substring(0, line1.IndexOf("/"));
                string st2 = line2.Substring(0, line2.IndexOf("/"));
                if (st1.CompareTo(st2) == 0)
                {
                    sw3.WriteLine(MergeRow(line1, line2));
                    line1 = sr1.ReadLine();
                    line2 = sr2.ReadLine();
                }

                else if (st1.CompareTo(st2) < 0) // str1 < str2 . str1 is ENTERED
                {
                    sw3.WriteLine(line1);
                    line1 = sr1.ReadLine();
                }
                else // str1 > str2 . str2 is ENTERED
                {
                    sw3.WriteLine(line2);
                    line2 = sr2.ReadLine();
                }
            }

            //write all the files
            if (line1 == null)
            {
                while (line2 != null)
                {
                    sw3.WriteLine(line2);
                    line2 = sr2.ReadLine();

                }
            }
            else if (line2 == null)
            {
                while (line1 != null)
                {
                    sw3.WriteLine(line1);
                    line1 = sr1.ReadLine();
                }
            }
            sr1.Close();
            sr2.Close();
            sw3.Close();
            fs1.Close();
            fs2.Close();
            File.Delete(path_file + "/" + num1.ToString());
            File.Delete(path_file + "/" + num2.ToString());
            fs3.Close();
        }

        // merge the row if there are the same term
        public string MergeRow(string line1, string line2)
        {
            // the name of term
            int index1, index2;
            index1 = line1.IndexOf("/");
            index2 = line2.IndexOf("/");
            // the name
            string st1 = line1.Substring(0, line1.IndexOf("/"));
            string new_line = st1;

            //// get all the doc & times
            line1 = line1.Substring(index1 + 1, line1.Length - index1 - 1);
            line2 = line2.Substring(index2 + 1, line2.Length - index2 - 1);
            new_line = new_line + "/" + line1 + line2;
            return new_line;
        }

        //merge the lines and write to file
        private void MergeLinesAndWrite(string path_file3, string path_SaveFiles, int lastFile, bool DoStemming)
        {
            FileStream fs1 = new FileStream(path_file3 + "/" + lastFile.ToString(), FileMode.Open, FileAccess.Read);
            StreamReader sr1 = new StreamReader(fs1);
            FileStream fs3;
            if (DoStemming == false)
            {
                fs3 = new FileStream(path_SaveFiles + "/ConnectedWords", FileMode.Create);
            }
            else
            {
                fs3 = new FileStream(path_SaveFiles + "/ConnectedWordsStemming", FileMode.Create);                
            }

            ;
            StreamWriter sw3 = new StreamWriter(fs3);
            string line1 = "";
            string line2 = "";
            line1 = sr1.ReadLine();
            line2 = sr1.ReadLine();

            while (line1 != null && line2 != null)
            {
                string stl1 = line1.Substring(0, line1.IndexOf("/"));
                string stl2 = line2.Substring(0, line2.IndexOf("/"));

                if (stl1.CompareTo(stl2) == 0)
                {
                    sw3.WriteLine(MergeRow(line1, line2));
                    line1 = sr1.ReadLine();
                    //if sr4 line 1 read the last line in the doc 
                    if (line1 != null)
                    {
                        line2 = sr1.ReadLine();
                    }
                }
                else if (stl1.CompareTo(stl2) < 0) // str1 < str2 . str1 is ENTERED
                {
                    sw3.WriteLine(line1);
                    line1 = sr1.ReadLine();
                }
                else if (stl1.CompareTo(stl2) > 0)// str1 > str2 . str2 is ENTERED 
                {
                    sw3.WriteLine(line2);
                    line2 = sr1.ReadLine();
                }
            }

            //write last the files
            if (line1 == null)
            {
                sw3.WriteLine(line2);
            }
            else if (line2 == null)
            {
                sw3.WriteLine(line1);
            }

            sr1.Close();
            sw3.Close();
            fs1.Close();
            fs3.Close();
            File.Delete(path_file3 + "/" + lastFile.ToString());
            Directory.Delete(path_file3);
        }
        // get the dicioary of top 5 for the term
        public Dictionary<string, ArrayList> getTheDic(string path_SaveFiles, bool DoStemming)
        {           
            Dictionary<string, ArrayList> dec_ConnectedWords = new Dictionary<string, ArrayList>();
            ArrayList connectedWords = new ArrayList();
            FileStream fs;
            if (DoStemming==false)
            {
                fs = new FileStream(path_SaveFiles + "/ConnectedWords", FileMode.Open, FileAccess.Read);
            }
            else
            {
                fs = new FileStream(path_SaveFiles + "/ConnectedWordsStemming", FileMode.Open, FileAccess.Read);
            }
            StreamReader sr = new StreamReader(fs);
            string line = "";
            string first_name_before = "";
            for (int i = 0; i < 96; i++ )
            {
                line = sr.ReadLine();
            }
                line = sr.ReadLine();
            while (line != null)
            {
                string name = line.Substring(0, line.IndexOf("/")).ToLower();
                string first_name = line.Substring(0, line.IndexOf(" ")).ToLower();
               
                string info = line.Substring(line.IndexOf("/")+1);
                Term t = new Term(name);
                while(info!="")
                { // get all the info from the line
                    string name_doc = info.Substring(0, info.IndexOf(","));
                    t.List_DOC.Add(name_doc);
                    t.DF++;
                    int index1=info.IndexOf(",");
                    int index2=info.IndexOf(";");
                    string number = info.Substring(index1 + 1, index2 - index1-1);
                    int i1;
                    if (Int32.TryParse(number, out i1))
                    {                      
                            t.TF = t.TF + i1;                      
                    }
                    info = info.Substring(index2 + 1);
                }
                // its exist already in the dicionary
                if (first_name == first_name_before)
                {
                    dec_ConnectedWords[first_name].Add(t);
                }
                else
                { // its not exist in the dic
                    if (dec_ConnectedWords.Count>0)
                    {
                        checkArrayList(dec_ConnectedWords, first_name_before); // get the top 5 in array list
                    }
                    if (!dec_ConnectedWords.ContainsKey(first_name))
                    {
                        dec_ConnectedWords.Add(first_name, new ArrayList());
                        dec_ConnectedWords[first_name].Add(t);
                       
                    }
                    first_name_before = first_name;
                }

                  line = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            WriteTop5(dec_ConnectedWords, path_SaveFiles, DoStemming);
            return dec_ConnectedWords;         
        }
        // check the arraylist of the term
        private void checkArrayList(Dictionary<string, ArrayList> dec_ConnectedWords, string first_name)
        {
            ArrayList list = dec_ConnectedWords[first_name];
            ArrayList Tfs =new ArrayList();
            ArrayList Final = new ArrayList();
           foreach (Term item in list)
            {
                Tfs.Add(item.TF);
            }

           Tfs.Sort();
           Tfs.Reverse();

            int count=0;
           foreach (int tf in Tfs)
           {
              if (count < 5)
              {
                  foreach (Term term in list)
                  { 
                      if (term.TF==tf)
                      {
                          Final.Add(term);
                          count++;
                          list.Remove(term);
                          break;
                      }
                  }
              }
           }
            // get just the top 5 of the terms connected
            dec_ConnectedWords[first_name] = Final;
           
       }
        // write the top 5 to the file
        private void WriteTop5(Dictionary<string, ArrayList> dec_ConnectedWords, string path_SaveFiles, bool DoStemming)
        {
         
            FileStream fs1;
            if (DoStemming == false)
            {               
                fs1 = new FileStream(path_SaveFiles + "/ConnectedWordsNEW", FileMode.Create);
                File.Delete(path_SaveFiles + "/ConnectedWords");
            }
            else
            {
                fs1 = new FileStream(path_SaveFiles + "/ConnectedWordsStemmingNEW", FileMode.Create);
                File.Delete(path_SaveFiles + "/ConnectedWordsStemming");
            }
            StreamWriter sw = new StreamWriter(fs1);
            foreach (string name in dec_ConnectedWords.Keys)
            {               
                    sw.WriteLine(name); // write the name of the first_NAME
                    ArrayList list = dec_ConnectedWords[name];
                    foreach (Term term in list)
                    {
                        sw.WriteLine(term.NAME + "/" + term.TF + "/" + term.DF);
                    }                           
            }
            
            sw.Close();
            fs1.Close();
        }
        // get all the terms and top 5 from the connected words
        public Dictionary<string, ArrayList> ReadTop5(string path_SaveFiles, bool DoStemming)
        {
            Dictionary<string, ArrayList> Dic_Top5 = new Dictionary<string, ArrayList>();
            FileStream fs1;
            if (DoStemming == false)
            {
                fs1 = new FileStream(path_SaveFiles + "/ConnectedWordsNEW", FileMode.Open, FileAccess.Read);
            }
            else
            {
                fs1 = new FileStream(path_SaveFiles + "/ConnectedWordsStemmingNEW", FileMode.Open, FileAccess.Read);
            }
            StreamReader sr = new StreamReader(fs1);
            string line = "";
            line = sr.ReadLine();
            while (line!=null)
            {                          
                    if (!line.Contains("/") )
                {
                    Dic_Top5.Add(line, new ArrayList());
                }
                else
                { // get the info of the term - the te and df
                    string first_name = line.Substring(0, line.IndexOf(" ")).ToLower();
                    string name = line.Substring(0, line.IndexOf("/")).ToLower();
                    Term t = new Term(name);
                    int index1= line.IndexOf("/");
                    line = line.Substring(index1 + 1);
                    int index2= line.IndexOf("/");
                    string tf = line.Substring(0, index2);
                    int i1,i2;
                    int tf_n,df_n;
                    if (Int32.TryParse(tf, out i1))
                    {
                        tf_n = i1;
                        t.TF = tf_n;
                    }
                    line = line.Substring(index2+1);
                    string df = line;
                     if (Int32.TryParse(df, out i2))
                     {
                         df_n = i2;
                         t.DF = df_n;
                     }                

                      Dic_Top5[first_name].Add(t);
                }
               
                line = sr.ReadLine();         
               
            }     

            sr.Close();
            fs1.Close();
            return Dic_Top5;
        }

    }
}
