using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IR
{
    class MergeFile
    {
        // merge the files
        public Dictionary<string, Term> MergeSort(string path_SaveFiles, string path_file3, string file4, bool DoStemming)
        {
            int lastFile = MergeFile3(path_file3); // get the last file that was merged

            MergeLinesAndWrite(path_file3, path_SaveFiles, lastFile); // check the lines and move to the savingFIles filder

            if (DoStemming == true) // with Stemming
            {
                return SeperateDicAndPosting(path_SaveFiles, DoStemming); // seperate the dic and the posting
            }
            else // NO STEMMING
            {
                return SeperateDicAndPosting(path_SaveFiles, DoStemming); // seperate the dic and the posting
            }
        }

        // merge the lines and write to the file
        private void MergeLinesAndWrite(string path_file3, string path_SaveFiles, int lastFile)
        {
            FileStream fs1 = new FileStream(path_file3 + "/" + lastFile.ToString(), FileMode.Open, FileAccess.Read);
            StreamReader sr1 = new StreamReader(fs1);
            FileStream fs3 = new FileStream(path_SaveFiles + "/Post", FileMode.Create);
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

        // merge all the docs of posting
        public int MergeFile3(string path_file)
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

        // merge the lines in the file
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

        // if there is simmillar term name , Merge the row
        public string MergeRow(string line1, string line2)
        {
            // the name of term
            int index1, index2;
            index1 = line1.IndexOf("/");
            index2 = line2.IndexOf("/");
            // the name of term
            string st1 = line1.Substring(0, line1.IndexOf("/"));
            string new_line = st1;
            // NDOC/DF/......
            line1 = line1.Substring(index1 + 1, line1.Length - index1 - 1);
            line2 = line2.Substring(index2 + 1, line2.Length - index2 - 1);
            string num1, num2;
            int i1, i2, i3;
            index1 = line1.IndexOf("/");
            index2 = line2.IndexOf("/");
            num1 = line1.Substring(0, index1);
            num2 = line2.Substring(0, index2);
            // get the DOC
            if (Int32.TryParse(num1, out i1) && Int32.TryParse(num2, out i2))
            {
                i3 = i1 + i2;
                new_line = new_line + "/" + i3.ToString();
            }
            //DF/......
            line1 = line1.Substring(index1 + 1, line1.Length - index1 - 1);
            line2 = line2.Substring(index2 + 1, line2.Length - index2 - 1);
            index1 = line1.IndexOf("/");
            index2 = line2.IndexOf("/");
            num1 = line1.Substring(0, index1);
            num2 = line2.Substring(0, index2);
            // get the TF
            if (Int32.TryParse(num1, out i1) && Int32.TryParse(num2, out i2))
            {
                i3 = i1 + i2;
                new_line = new_line + "/" + i3.ToString(); /////////////////
            }
            //// get all the doc & times
            line1 = line1.Substring(index1 + 1, line1.Length - index1 - 1);
            line2 = line2.Substring(index2 + 1, line2.Length - index2 - 1);
            new_line = new_line + "/" + line1 + line2;
            return new_line;
        }

        // sepearate the dictionary and the posting
        public Dictionary<string, Term> SeperateDicAndPosting(string path_SaveFiles, bool DoStemming)
        {
            int index;
            string lineD = "";
            FileStream FileS1, FileS2;
            StreamWriter StreamW1, StreamW2;

            if (DoStemming == false) // No stemming
            {
                FileS1 = new FileStream(path_SaveFiles + "/Dictionary", FileMode.Create);
                StreamW1 = new StreamWriter(FileS1);
                FileS2 = new FileStream(path_SaveFiles + "/PostingFile", FileMode.Create);
                StreamW2 = new StreamWriter(FileS2);
            }
            else // with stemming
            {
                FileS1 = new FileStream(path_SaveFiles + "/DictionaryStemming", FileMode.Create);
                StreamW1 = new StreamWriter(FileS1);
                FileS2 = new FileStream(path_SaveFiles + "/PostingFileStemming", FileMode.Create);
                StreamW2 = new StreamWriter(FileS2);
            }

            FileStream FileS3 = new FileStream(path_SaveFiles + "/post", FileMode.Open, FileAccess.Read);
            StreamReader StreamR = new StreamReader(FileS3);
            lineD = StreamR.ReadLine();
            string name;
            string tf;
            string df;
            int i1;
            Dictionary<string, Term> Dic_terms = new Dictionary<string, Term>();
            while (lineD != null)
            {
                //get the name;
                //-------->TERM/
                index = lineD.IndexOf("/");
                string st = lineD.Substring(0, index + 1); // ---> TERM/
                name = lineD.Substring(0, index); // TERM
                Term t = new Term(name);
                //-------->DF/TF/......
                lineD = lineD.Substring(index + 1, lineD.Length - index - 1);
                index = lineD.IndexOf("/");
                df = lineD.Substring(0, index);
                if (Int32.TryParse(df, out i1))
                {
                    t.DF = i1;
                }
                st = st + lineD.Substring(0, index + 1); // ---> TERM/DF
                lineD = lineD.Substring(index + 1, lineD.Length - index - 1);
                index = lineD.IndexOf("/");
                st = st + lineD.Substring(0, index); // ---> TERM/DF/TF
                tf = lineD.Substring(0, index); // TF
                if (Int32.TryParse(tf, out i1))
                {
                    t.TF = i1;
                }
                Dic_terms.Add(name, t);
                lineD = lineD.Substring(index + 1, lineD.Length - index - 1); // D1,5;
                StreamW1.WriteLine(st); // TERM/DF/TF
                index = lineD.IndexOf("/");
                lineD = lineD.Substring(index + 1, lineD.Length - index - 1); // ----->d1:5,d2:6
                StreamW2.WriteLine(lineD);
                lineD = StreamR.ReadLine();

            }
            StreamW1.Close();
            StreamW2.Close();
            StreamR.Close();
            FileS1.Close();
            FileS2.Close();
            FileS3.Close();
            File.Delete(path_SaveFiles + "/post");
            return Dic_terms;
        }
    }
}
