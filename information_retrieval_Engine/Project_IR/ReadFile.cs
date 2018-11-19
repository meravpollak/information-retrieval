using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IR
{
    public class ReadFile
    {
        private HashSet<string> Dic_languages; // dicionaey of languagess
        private int NumDOcs; // number of docs

        // the readFile
        public ReadFile()
        {
            Dic_languages = new HashSet<string>();
            NumDOcs = 0;
        }
        // language of the doc
        public HashSet<string> LANGUAGES
        {
            get { return Dic_languages; }
            set { Dic_languages = value; }
        }
        // the num of the doc
        public int NUMDOCS
        {
            get { return NumDOcs; }
            set { NumDOcs = value; }
        }
        // read the files
        public void readFile(string path, string path_stopWords, string path_docs, string path_posting, bool Dostemming)
        {
            Dictionary<int, string[]> Dic = new Dictionary<int, string[]>();
            int name_newFile = 0;
            int count_fileToSent = 0;
            string[] files = Directory.GetFiles(path);

            ArrayList AllFiles = new ArrayList();
            // add all the files
            for (int i = 0; i < files.Count(); i++)
            {
                if (files[i] != (path + @"\stop_words.txt") && files[i] != (path + @"\doc"))
                {
                    AllFiles.Add(files[i]);
                }
            }
            for (int x = 0; x < AllFiles.Count; x++)
            {
                string file = AllFiles[x].ToString();

                //open the file
                if (File.Exists(file))
                {
                    FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs);
                    string name;
                    string content = "";
                    string line = "";
                    int FileNumber;
                    string FileName = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "<DOC>")
                        {
                            line = sr.ReadLine();
                            name = line;
                            FileName = name.Substring(7, (name.Length - 7));
                            FileName = FileName.Substring(0, FileName.Length - 8);
                            if (FileName.Substring(0, 1) == " ")
                            {
                                FileName = FileName.Substring(1, FileName.Length - 1);
                            }
                            if (FileName.Substring(FileName.Length - 1, 1) == " ")
                            {
                                FileName = FileName.Substring(0, FileName.Length - 1);
                            }
                            name = name.Substring(14, (name.Length - 23));
                            FileNumber = Int32.Parse(name); // get the number of file
                            Dic[FileNumber] = new string[4];
                            Dic[FileNumber][0] = "NoLanguage"; // the language of the doc

                            line = sr.ReadLine();
                            ////title
                            while (!line.Contains("<H3> <TI>") && line != "<TEXT>" && !line.Contains("<F P=105>"))
                            {
                                line = sr.ReadLine();
                            }

                            string title = "NoTitle";

                            if (line.Contains("<H3> <TI>"))
                            {
                                title = line;

                                while (!title.Contains("</TI></H3>"))
                                {
                                    string line2 = sr.ReadLine();

                                    title = title + line2;
                                }

                                //cut the begining
                                title = title.Substring(9);
                                //cut the end
                                title = title.Substring(0, title.Length - 11);

                                //No title to the doc
                                if (title == "")
                                {
                                    title = "NoTitle";
                                }

                                //cut space from the begining
                                while (title.Substring(0, 1) == " ")
                                {
                                    title = title.Substring(1, title.Length - 1);
                                }
                                //cut space from the end
                                while (title.Substring(title.Length - 1, 1) == " ")
                                {
                                    title = title.Substring(0, title.Length - 2);
                                }

                            }

                            Dic[FileNumber][3] = title;

                            while (line != "<TEXT>" && !line.Contains("<F P=105>")) // see the content or the language
                            {
                                line = sr.ReadLine();
                            }

                            if (line == "<TEXT>")
                            {
                                //the next line is start content 
                                line = sr.ReadLine();
                                //see the language or the title
                                if (line.Contains("<F P=105>") || line.Contains("</TI></H3>"))
                                {
                                    if (line.Contains("<F P=105>"))
                                    {
                                        int index = line.IndexOf("5") + 2;
                                        string n = line.Substring(index, line.Length - 5 - index);
                                        string leng = line.Substring(index, line.Length - 5 - index);

                                        while (leng.Substring(0, 1) == " ")
                                        {
                                            leng = leng.Substring(1, leng.Length - 1);
                                        }

                                        //end
                                        while (leng.Substring(leng.Length - 1, 1) == " ")
                                        {
                                            leng = leng.Substring(0, leng.Length - 2);
                                        }
                                        Dic[FileNumber][0] = leng;
                                        if (!Dic_languages.Contains(leng))
                                        {
                                            Dic_languages.Add(leng);
                                        }
                                        line = sr.ReadLine();
                                    }
                                    else if (line.Contains("</TI></H3>"))
                                    {
                                        title = line;

                                        while (!title.Contains("</TI></H3>"))
                                        {
                                            string line2 = sr.ReadLine();

                                            title = title + line2;
                                        }
                                        //cut the begining
                                        title = title.Substring(9);
                                        //cut the end
                                        title = title.Substring(0, title.Length - 11);

                                        //No title to the doc
                                        if (title == "")
                                        {
                                            title = "NoTitle";
                                        }

                                        //cut space from the begining
                                        while (title.Substring(0, 1) == " ")
                                        {
                                            title = title.Substring(1, title.Length - 1);
                                        }
                                        //cut space from the end
                                        while (title.Substring(title.Length - 1, 1) == " ")
                                        {
                                            title = title.Substring(0, title.Length - 2);
                                        }
                                        Dic[FileNumber][3] = title;
                                    }

                                }
                                else // this is a regular row
                                {
                                    content = content + line;
                                    line = sr.ReadLine();
                                }
                            }

                            else if (line.Contains("<F P=105>")) // its a language
                            {
                                int index = line.IndexOf("5") + 3;
                                string n = line.Substring(index, line.Length - 5 - index);
                                string leng = line.Substring(index, line.Length - 5 - index);

                                while (leng.Substring(0, 1) == " ")
                                {
                                    leng = leng.Substring(1, leng.Length - 1);
                                }

                                //end
                                while (leng.Substring(leng.Length - 1, 1) == " ")
                                {
                                    leng = leng.Substring(0, leng.Length - 2);
                                }
                                Dic[FileNumber][0] = leng;
                                if (!Dic_languages.Contains(leng))
                                {
                                    Dic_languages.Add(leng);
                                }

                                line = sr.ReadLine();
                                while (line != "<TEXT>")
                                {
                                    line = sr.ReadLine();
                                }
                            }

                            //read all the content from the file
                            while (line != "</TEXT>")
                            {

                                if (line.Contains("<F P=105>") || line.Contains("</TI></H3>"))
                                {
                                    if (line.Contains("<F P=105>"))
                                    {
                                        int index = line.IndexOf("5") + 2;
                                        string n = line.Substring(index, line.Length - 5 - index);
                                        string leng = line.Substring(index, line.Length - 5 - index);

                                        while (leng.Substring(0, 1) == " ")
                                        {
                                            leng = leng.Substring(1, leng.Length - 1);
                                        }

                                        //end
                                        while (leng.Substring(leng.Length - 1, 1) == " ")
                                        {
                                            leng = leng.Substring(0, leng.Length - 2);
                                        }
                                        Dic[FileNumber][0] = leng;
                                        if (!Dic_languages.Contains(leng))
                                        {
                                            Dic_languages.Add(leng);
                                        }
                                        //   line = sr.ReadLine();
                                    }
                                    else if (line.Contains("</TI></H3>"))
                                    {
                                        title = line;

                                        while (!title.Contains("</TI></H3>"))
                                        {
                                            string line2 = sr.ReadLine();

                                            title = title + line2;
                                        }
                                        //cut the begining
                                        title = title.Substring(9);
                                        //cut the end
                                        title = title.Substring(0, title.Length - 11);

                                        //No title to the doc
                                        if (title == "")
                                        {
                                            title = "NoTitle";
                                        }

                                        //cut space from the begining
                                        while (title.Substring(0, 1) == " ")
                                        {
                                            title = title.Substring(1, title.Length - 1);
                                        }
                                        //cut space from the end
                                        while (title.Substring(title.Length - 1, 1) == " ")
                                        {
                                            title = title.Substring(0, title.Length - 2);
                                        }
                                        Dic[FileNumber][3] = title;
                                    }

                                }
                                else // this is a regular row
                                {
                                    content = content + " " + line;
                                    //line = sr.ReadLine();
                                }
                                //content = content + " " + line;
                                line = sr.ReadLine();
                            }

                            Dic[FileNumber][1] = content;
                            Dic[FileNumber][2] = FileName;
                            NumDOcs++;
                            name = "";
                            content = "";
                        }
                    }
                    count_fileToSent++;
                    sr.Close();
                    fs.Close();
                    //send 10 docs to the parse
                    if (count_fileToSent == 10)
                    {
                        name_newFile++;
                        string path_new_file = path_posting + "/posting/" + name_newFile.ToString();
                        FileStream fs1 = new FileStream(path_new_file, FileMode.Create);
                        fs1.Close();
                        string path_wordsConnected = path_posting + "/ToConnectWords/" + name_newFile.ToString();
                        FileStream fs2 = new FileStream(path_wordsConnected, FileMode.Create);
                        fs2.Close();
                        Parse p = new Parse();
                        p.parse(Dic, path_stopWords, path_new_file, path_docs, Dostemming, true, path_wordsConnected, 0, new Dictionary<string, Document>(), new Dictionary<string, Term>(), new ArrayList(), "");
                        Dic = new Dictionary<int, string[]>();
                        count_fileToSent = 0;
                    }

                }
                // if it is the end
            }

            if (count_fileToSent < 10 && count_fileToSent != 0)
            {
                name_newFile++;
                string path_new_file = path_posting + "/posting/" + name_newFile.ToString();
                FileStream fs1 = new FileStream(path_new_file, FileMode.Create);
                fs1.Close();
                string path_wordsConnected = path_posting + "/ToConnectWords/" + name_newFile.ToString();
                FileStream fs2 = new FileStream(path_wordsConnected, FileMode.Create);
                fs2.Close();
                Parse p = new Parse();
                p.parse(Dic, path_stopWords, path_new_file, path_docs, Dostemming, true, path_wordsConnected, 0, new Dictionary<string, Document>(), new Dictionary<string, Term>(), new ArrayList(), "");
            }

        }
    }

}
