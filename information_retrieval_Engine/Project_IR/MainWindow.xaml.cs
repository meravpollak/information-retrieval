using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using LAIR.ResourceAPIs.WordNet;///////////////////////////////////////////////////////////////
using LAIR.Collections.Generic;////////////////////

namespace Project_IR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, Term> terms_dictionary_stemming;
        public Dictionary<string, ArrayList> terms_connectedWords_stemming;
        public Dictionary<string, ArrayList> terms_connectedWords_NOstemming;
        public Dictionary<string, Term> terms_dictionary;
        public Dictionary<string, string> languages;
        public Dictionary<int, string[]> queries;
        public HashSet<string> HashSet_languages;
        ArrayList languages_Pressed_LIstBox;
        public static bool STOP;
        public Dictionary<string, Document> doc_dic;
        int CountUniqueTermTotal;
        int NumbersOfDoc;
        bool isFinished;
        bool isFInieshed_doc_Rel;
        string path_corpus;
        string path_posting;
        int total_lenght_doc;
        public int number_Query;
        public string path_queries_ranked;
        public Dictionary<int,ArrayList> m_dic_relDocs_For_query;
        public MainWindow()
        {
            InitializeComponent();
            //
            //ArrayList semantic_words = GetSemantic("goverment");
            //
            terms_dictionary_stemming = new Dictionary<string, Term>();
            terms_dictionary = new Dictionary<string, Term>();
            terms_connectedWords_NOstemming = new Dictionary<string, ArrayList>();
            terms_connectedWords_stemming = new Dictionary<string, ArrayList>();
            languages = new Dictionary<string, string>();
            queries = new Dictionary<int, string[]>();
            languages_Pressed_LIstBox = new ArrayList();
            CountUniqueTermTotal = 0;
            NumbersOfDoc = 0;
            isFinished = false;
            isFInieshed_doc_Rel = false;
            path_corpus = "";
            path_posting = "";
            path_queries_ranked = "";
            HashSet_languages = new HashSet<string>();
            doc_dic = new Dictionary<string, Document>();
            total_lenght_doc = 0;
            number_Query = 500;
            m_dic_relDocs_For_query = new Dictionary<int, ArrayList>();
        }

        // get the semantic words
        private ArrayList GetSemantic(string word)
        {
            ArrayList Semantic_words = new ArrayList();
            WordNetEngine wn = new WordNetEngine(@"../../", false);
            char[] delimiters = { ' ', ',' };
            // get the nouns
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
                        Semantic_words.Add(words[i]);
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
                        Semantic_words.Add(words[i]);
                    }
                }
            }
            return Semantic_words;

        }

        //click on the browse button to curpus
        private void browseDoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path_corpus;
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                DialogResult result = fbd.ShowDialog();
                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    path_corpus = fbd.SelectedPath;
                    TextBoxCorpus.Text = path_corpus;
                }
                else
                {
                    System.Windows.MessageBox.Show("The path of the Files and StopWords is empty!");
                }
            }
            catch (Exception e2)
            {
                System.Windows.MessageBox.Show(e2.Message);
            }

        }

        //click on the browse button to posting
        private void broewsePosting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path_posting;
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                DialogResult result = fbd.ShowDialog();
                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    path_posting = fbd.SelectedPath;
                    TextBoxPosting.Text = path_posting;
                }
                else
                {
                    System.Windows.MessageBox.Show("The path of the Posting and Dictionary is empty!");
                }
            }
            catch (Exception e1)
            {
                System.Windows.MessageBox.Show(e1.Message);
            }
        }

        // click on the start to build the posing and the dictionary
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBoxLanguages.Items.Clear();
                string path_corpus = TextBoxCorpus.Text;
                string path_posting = TextBoxPosting.Text;
                string path_docs = path_corpus + "/" + "doc";
                FileStream fs1 = new FileStream(path_docs, FileMode.Create);
                fs1.Close();
                if (path_corpus != "" && path_posting != "")
                {
                    System.Windows.MessageBox.Show("Start to process!");
                    string path_stopWords = path_corpus + "/stop_words.txt";
                    string path_folder = path_posting + "/posting";
                    string path_folder1 = path_posting + "/post";
                    string path_folder_connectedWords = path_posting + "/ToConnectWords";
                    string path_folder1_connectedWords = path_posting + "/FinalConnectWords";
                    string path_coonectWords_folder = path_posting + "/ToConnectWords";
                    // open folder for files Type FB3
                    if (!System.IO.Directory.Exists(path_folder))
                    {
                        System.IO.Directory.CreateDirectory(path_folder);
                    }
                    if (!System.IO.Directory.Exists(path_coonectWords_folder))
                    {
                        System.IO.Directory.CreateDirectory(path_coonectWords_folder);
                    }
                    ReadFile r = new ReadFile();
                    Parse p = new Parse();
                    MergeFile mf = new MergeFile();
                    MergeConnectedWords mf1 = new MergeConnectedWords();
                    Stopwatch Watch = new Stopwatch();
                    System.Windows.Threading.DispatcherTimer disTimer = new System.Windows.Threading.DispatcherTimer();
                    disTimer.Tick += new EventHandler(disTimer_Tick);
                    disTimer.Interval = new TimeSpan(0, 0, 1);
                    disTimer.Start();
                    isFinished = false;
                    if (checkBoxStem.IsChecked == true) // with stemming
                    {
                        new Thread(delegate ()
                        {
                            Watch.Start();
                            r.readFile(path_corpus, path_stopWords, path_docs, path_posting, true);
                            terms_dictionary_stemming = mf.MergeSort(path_posting, path_folder, path_folder1, true);
                            //
                            terms_connectedWords_stemming = mf1.MergeSort(path_posting, path_folder_connectedWords, path_folder1_connectedWords, true);/////
                            Watch.Stop();
                            isFinished = true;
                            Thread.Sleep(1000);
                            disTimer.Stop();
                            NumbersOfDoc = r.NUMDOCS;
                            TimeSpan timespan = Watch.Elapsed;
                            string FinalTime = String.Format("{0:00}:{1:00}:{2:00}", timespan.Hours, timespan.Minutes, timespan.Seconds);
                            string seconds = ((timespan.Minutes * 60) + timespan.Seconds).ToString();
                            System.Windows.MessageBox.Show("Total docs Indexed: " + NumbersOfDoc + " Total Time: " + FinalTime + " [" + seconds + " seconds] " + " Total unique words: " + terms_dictionary_stemming.Count);
                        }).Start();
                    }
                    else// without stemming
                    {
                        new Thread(delegate ()
                        {
                            Watch.Start();
                            r.readFile(path_corpus, path_stopWords, path_docs, path_posting, false);
                            terms_dictionary = mf.MergeSort(path_posting, path_folder, path_folder1, false);
                            //
                            terms_connectedWords_NOstemming = mf1.MergeSort(path_posting, path_folder_connectedWords, path_folder1_connectedWords, false);///////
                            Watch.Stop();
                            isFinished = true;
                            Thread.Sleep(1000);
                            disTimer.Stop();
                            NumbersOfDoc = r.NUMDOCS;
                            TimeSpan timespan = Watch.Elapsed;
                            string FinalTime = String.Format("{0:00}:{1:00}:{2:00}", timespan.Hours, timespan.Minutes, timespan.Seconds);
                            string seconds = ((timespan.Minutes * 60) + timespan.Seconds).ToString();
                            System.Windows.MessageBox.Show("Total docs Indexed: " + NumbersOfDoc + " Total Time: " + FinalTime + " [" + seconds + " seconds] " + " Total unique words: " + terms_dictionary.Count);
                        }).Start();
                    }

                }
                else
                {
                    System.Windows.MessageBox.Show("The path for Save Or Load Files is empty!");
                }
            }
            catch (Exception e3)
            {
                System.Windows.MessageBox.Show(e3.Message);
            }

        }

        // wait for uploading and show the languages options
        private void disTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                path_corpus = TextBoxCorpus.Text;
                if (isFinished == true)
                {
                    // load the lagnuages
                    FileStream fs3 = new FileStream(path_corpus + "/doc", FileMode.Open, FileAccess.Read);
                    StreamReader sr3 = new StreamReader(fs3);
                    string line3 = "";
                    int index3;
                    line3 = sr3.ReadLine();
                    string name, lenght, leng, times_commen_term, uniqe, d_commen_term, title;
                    while (line3 != null)
                    {

                        index3 = line3.IndexOf(";");
                        name = line3.Substring(0, index3);/////////

                        line3 = line3.Substring(index3 + 1, line3.Length - index3 - 1);
                        index3 = line3.IndexOf(";");
                        lenght = line3.Substring(0, index3);/////////

                        line3 = line3.Substring(index3 + 1, line3.Length - index3 - 1);
                        index3 = line3.IndexOf(";");
                        leng = line3.Substring(0, index3);
                        string left = line3;
                        line3 = line3.Substring(0, index3);
                        line3 = line3.ToLower();

                        left = left.Substring(index3 + 1, left.Length - index3 - 1);
                        index3 = left.IndexOf(";");
                        times_commen_term = left.Substring(0, index3);

                        left = left.Substring(index3 + 1, left.Length - index3 - 1);
                        index3 = left.IndexOf(";");
                        uniqe = left.Substring(0, index3);

                        left = left.Substring(index3 + 1, left.Length - index3 - 1);
                        index3 = left.IndexOf(";");
                        d_commen_term = left.Substring(0, index3);

                        left = left.Substring(index3 + 1, left.Length - index3 - 1);
                        /////title = left;
                        int index4 = left.IndexOf(";");
                        title = left.Substring(0, index4);
                        left = left.Substring(index4 + 1);
                        string start = left;

                        line3 = line3.ToLower();
                        double i1;
                        if (!languages.ContainsKey(line3) && line3 != "NoLanguage" && !line3.Contains(",") && !line3.Contains(" ") && !double.TryParse(line3, out i1) && !line3.Contains("hebrew3") && !line3.Contains("serbo-"))
                        {
                            languages.Add(line3, "");
                        }

                        int number = 0;
                        int i5, i6;
                        if (Int32.TryParse(lenght, out i5))
                        {
                            number = i5;
                            total_lenght_doc = total_lenght_doc + number;
                        }
                        string[] list = new string[0];
                        Document d = new Document(name, list, number, leng, title);
                        d.COMMON_TERM = d_commen_term;
                        if (Int32.TryParse(times_commen_term, out i6))
                        {
                            number = i6;
                        }
                        d.COUNT_COMMON_TERM = number;
                        d.START10 = start;
                        doc_dic[name] = d;

                        line3 = sr3.ReadLine();

                    }
                    sr3.Close();
                    fs3.Close();
                    Dictionary<string, string> SoretedLanguage = new Dictionary<string, string>();
                    foreach (var item in languages.OrderBy(i => i.Key))
                    {
                        SoretedLanguage[item.Key] = item.Value; ;
                    }
                    languages = SoretedLanguage;
                    foreach (string language in languages.Keys)
                    {
                        ListBoxLanguages.Items.Add(language);
                    }
                    isFinished = false;
                }
            }
            catch (Exception e4)
            {
                System.Windows.MessageBox.Show(e4.Message);
            }

        }

        //reaset the diciotnary and the posting
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path_corpus = TextBoxCorpus.Text;
                string path_posting = TextBoxPosting.Text;

                if (path_corpus != "" && path_posting != "")
                {
                    if (!File.Exists(path_posting + "/PostingFile") || !File.Exists(path_posting + "/PostingFileStemming"))
                    {
                        System.Windows.MessageBox.Show("There is no file to delete! The posting isnt exist!");
                    }
                    else
                    {
                        File.Delete(path_posting + "/PostingFile");
                        File.Delete(path_posting + "/Dictionary");
                        File.Delete(path_posting + "/PostingFileStemming");
                        File.Delete(path_posting + "/DictionaryStemming");
                        File.Delete(path_corpus + "/doc");
                        File.Delete(path_posting + "/PostingFile");
                        File.Delete(path_posting + "/ConnectedWordsNEW");
                        File.Delete(path_posting + "/ConnectedWordsStemmingNEW");
                        File.Delete(path_posting + "/queries_ranked");////////////////////////////////////////////////////////
                        System.Windows.MessageBox.Show("The files was deleted Successfully!");
                    }
                    terms_dictionary_stemming = new Dictionary<string, Term>();
                    terms_dictionary = new Dictionary<string, Term>();
                    languages = new Dictionary<string, string>();
                    ListBoxLanguages.Items.Clear();
                }
                else
                {
                    System.Windows.MessageBox.Show("The path of the Files and StopWords is empty!");
                }
            }
            catch (Exception e5)
            {
                System.Windows.MessageBox.Show(e5.Message);
            }
        }

        //show the diciotnary
        private void ShowDic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBoxLanguages.Items.Clear();/////////////////
                string path_corpus = TextBoxCorpus.Text;
                string path_posting = TextBoxPosting.Text;
                Dictionary<string, Term> Temp_dic = new Dictionary<string, Term>();

                if (path_corpus != "" && path_posting != "")
                {
                    if (checkBoxStem.IsChecked == true) // with stemming
                    {
                        Temp_dic = terms_dictionary_stemming;
                    }
                    else // NO Stemming
                    {
                        Temp_dic = terms_dictionary;
                    }
                    //check if the dictionary is empty
                    if (Temp_dic.Count != 0)
                    {
                        Window1 window_SHowDic = new Window1(Temp_dic);
                        window_SHowDic.ShowDialog();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("The dictionary is Empty! You should load it first!");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("The path of the Save posting is empty! You should load it first!");
                }
            }
            catch (Exception e6)
            {
                System.Windows.MessageBox.Show(e6.Message);
            }
        }
        // load the dictionary from files
        private void ReloadDic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                terms_dictionary_stemming = new Dictionary<string, Term>();//////////////////
                terms_dictionary = new Dictionary<string, Term>();/////////////
                string path_posting = TextBoxPosting.Text;
                string path_corpus = TextBoxCorpus.Text;
                FileStream fs;
                StreamReader sr;
                if (path_posting != "")
                {

                    if (checkBoxStem.IsChecked == true) // with stemming
                    {
                        if (!File.Exists(path_posting + "/DictionaryStemming"))
                        {
                            System.Windows.MessageBox.Show("The File isnt exist!");
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Loading the dictionary!");
                            fs = new FileStream(path_posting + "/DictionaryStemming", FileMode.Open, FileAccess.Read);
                            sr = new StreamReader(fs);
                            string line = "";
                            int index = 0;
                            string name;
                            string df;
                            int i1;
                            line = sr.ReadLine();
                            while (line != null)
                            {
                                //get the name;
                                //-------->TERM/
                                index = line.IndexOf("/");
                                name = line.Substring(0, index); // TERM
                                Term t = new Term(name);
                                //-------->DF/TF/......
                                line = line.Substring(index + 1, line.Length - index - 1);
                                index = line.IndexOf("/");
                                df = line.Substring(0, index);
                                if (Int32.TryParse(df, out i1))
                                {
                                    t.DF = i1;
                                }
                                line = line.Substring(index + 1, line.Length - index - 1);
                                if (Int32.TryParse(line, out i1))
                                {
                                    t.TF = i1;
                                }
                                terms_dictionary_stemming.Add(name, t);
                                line = sr.ReadLine();
                            }
                            sr.Close();
                            fs.Close();
                        }

                    }
                    else// without stemming
                    {
                        if (!File.Exists(path_posting + "/Dictionary"))
                        {
                            System.Windows.MessageBox.Show("The File isnt exist!");
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Loading the dictionary!");
                            fs = new FileStream(path_posting + "/Dictionary", FileMode.Open, FileAccess.Read);
                            sr = new StreamReader(fs);
                            string line = "";
                            int index = 0;
                            string name;
                            string df;
                            int i1;
                            line = sr.ReadLine();
                            while (line != null)
                            {
                                //get the name;
                                //-------->TERM/
                                index = line.IndexOf("/");
                                name = line.Substring(0, index); // TERM
                                Term t = new Term(name);
                                //-------->DF/TF/......
                                line = line.Substring(index + 1, line.Length - index - 1);
                                index = line.IndexOf("/");
                                df = line.Substring(0, index);
                                if (Int32.TryParse(df, out i1))
                                {
                                    t.DF = i1;
                                }
                                line = line.Substring(index + 1, line.Length - index - 1);
                                if (Int32.TryParse(line, out i1))
                                {
                                    t.TF = i1;
                                }
                                terms_dictionary.Add(name, t);
                                line = sr.ReadLine();
                            }
                            sr.Close();
                            fs.Close();
                        }
                    }

                    // load the lagnuages
                    if (!File.Exists(path_corpus + "/doc"))
                    {
                        System.Windows.MessageBox.Show("Cant Load the Doc FIle because the File isnt Exist!");
                    }
                    else
                    {
                        FileStream fs1 = new FileStream(path_corpus + "/doc", FileMode.Open, FileAccess.Read);
                        StreamReader sr1 = new StreamReader(fs1);
                        string line1 = "";
                        int index1;
                        line1 = sr1.ReadLine();
                        string name, lenght, leng, times_commen_term, uniqe, d_commen_term, title;
                        while (line1 != null)
                        {
                            index1 = line1.IndexOf(";");
                            name = line1.Substring(0, index1);/////////

                            line1 = line1.Substring(index1 + 1, line1.Length - index1 - 1);
                            index1 = line1.IndexOf(";");
                            lenght = line1.Substring(0, index1);/////////

                            line1 = line1.Substring(index1 + 1, line1.Length - index1 - 1);
                            index1 = line1.IndexOf(";");
                            leng = line1.Substring(0, index1);
                            string left = line1;
                            line1 = line1.Substring(0, index1);
                            line1 = line1.ToLower();

                            left = left.Substring(index1 + 1, left.Length - index1 - 1);
                            index1 = left.IndexOf(";");
                            times_commen_term = left.Substring(0, index1);

                            left = left.Substring(index1 + 1, left.Length - index1 - 1);
                            index1 = left.IndexOf(";");
                            uniqe = left.Substring(0, index1);

                            left = left.Substring(index1 + 1, left.Length - index1 - 1);
                            index1 = left.IndexOf(";");
                            d_commen_term = left.Substring(0, index1);

                            left = left.Substring(index1 + 1, left.Length - index1 - 1);
                           // title = left;
                            int index4 = left.IndexOf(";");
                            title = left.Substring(0, index4);
                            left = left.Substring(index4 + 1);
                            string start = left;

                            double i1;
                            if (!languages.ContainsKey(line1) && line1 != "NoLanguage" && !line1.Contains(",") && !line1.Contains(" ") && !double.TryParse(line1, out i1) && !line1.Contains("hebrew3") && !line1.Contains("serbo-"))
                            {
                                languages.Add(line1, "");
                            }

                            int number = 0;
                            int i5, i6;
                            if (Int32.TryParse(lenght, out i5))
                            {
                                number = i5;
                                total_lenght_doc = total_lenght_doc + number;
                            }
                            string[] list = new string[0];
                            Document d = new Document(name, list, number, leng, title);
                            d.COMMON_TERM = d_commen_term;
                            if (Int32.TryParse(times_commen_term, out i6))
                            {
                                number = i6;
                            }
                            d.COUNT_COMMON_TERM = number;
                            d.START10 = start;
                            doc_dic[name] = d;

                            line1 = sr1.ReadLine();
                        }
                        sr1.Close();
                        fs1.Close();
                        Dictionary<string, string> SoretedLanguage = new Dictionary<string, string>();
                        foreach (var item in languages.OrderBy(i => i.Key))
                        {
                            SoretedLanguage[item.Key] = item.Value; ;
                        }
                        languages = SoretedLanguage;
                        foreach (string language in languages.Keys)
                        {
                            ListBoxLanguages.Items.Add(language);
                        }
                    }

                    // get the top5
                    if (checkBoxStem.IsChecked == true) // with stemming
                    {
                        if (!File.Exists(path_posting + "/ConnectedWordsStemmingNEW"))
                        {
                            System.Windows.MessageBox.Show("Cant Load the Doc FIle because the File isnt Exist!");
                        }
                        else
                        {
                            MergeConnectedWords mg = new MergeConnectedWords();
                            terms_connectedWords_stemming = mg.ReadTop5(path_posting, true);
                        }
                    }
                    else // no Stemming
                    {
                        if (!File.Exists(path_posting + "/ConnectedWordsNEW"))
                        {
                            System.Windows.MessageBox.Show("Cant Load the Doc FIle because the File isnt Exist!");
                        }
                        else
                        {
                            MergeConnectedWords mg = new MergeConnectedWords();
                            terms_connectedWords_NOstemming = mg.ReadTop5(path_posting, false);
                        }
                    }

                }
                else
                {
                    System.Windows.MessageBox.Show("The path of the Posting and Dictionary is empty!");
                }
            }
            catch (Exception e7)
            {
                System.Windows.MessageBox.Show(e7.Message);
            }

        }
        //the listbox is changed
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string path_corpus = TextBoxCorpus.Text;
                if (path_corpus != "")
                {
                    // get all the selected items
                    ArrayList languages = new ArrayList();
                    foreach (Object selecteditem in ListBoxLanguages.SelectedItems)
                    {
                        string language_item = selecteditem as String;
                        languages.Add(language_item);
                    }
                    // update the languages that was choose
                    languages_Pressed_LIstBox = languages;                   
                }
                else
                {
                    System.Windows.MessageBox.Show("The path of the doc is empty!");
                }
            }
            catch (Exception e8)
            {
                System.Windows.MessageBox.Show(e8.Message);
            }
        }
        // search for file of quries
        private void browseFileQueries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                path_corpus = TextBoxCorpus.Text;
                path_posting = TextBoxPosting.Text;
                if (path_corpus != "" && path_posting != "")
                {
                    if (checkBoxStem.IsChecked == true) // do stemming
                    {
                        if (terms_dictionary_stemming.Count == 0 || terms_connectedWords_stemming.Count == 0)
                        {
                            System.Windows.MessageBox.Show("You have to load the dictionary first!");
                        }

                        else
                        {
                           
                                readTheQueryAndSendToSearcher(true);                          
                        }
                    }
                    else // no stemming
                    {
                        if (terms_connectedWords_NOstemming.Count == 0 || terms_dictionary.Count == 0)
                        {
                            System.Windows.MessageBox.Show("You have to load the dictionary first!");
                        }

                        else
                        {
                          
                            readTheQueryAndSendToSearcher(false);

                        }
                    }
                }

                else
                {
                    System.Windows.MessageBox.Show("The path of the Posting and Dictionary is empty!");
                }

            }
            catch (Exception e2)
            {
                System.Windows.MessageBox.Show(e2.Message);
            }
        }
        //read The Query send to searcher
        private void readTheQueryAndSendToSearcher(bool DoStemming)
        {
            if (path_queries_ranked != "" && path_queries_ranked != "queries_ranked")
            {
                
                int i;               
                string string_num_query = "";
                int num_query = 0;
                string query = "";
                string path_quries;
                string line = "";
                System.Windows.Forms.OpenFileDialog Dialog = new System.Windows.Forms.OpenFileDialog();
                Dialog.Filter = "TXT files|*.txt";
                DialogResult result = Dialog.ShowDialog();
                if (Dialog.FileName != "")                
                {
                    queriesFileTextBox.Text = Dialog.FileName;
                    System.Windows.Threading.DispatcherTimer timer_showRelevantDoc = new System.Windows.Threading.DispatcherTimer();
                    timer_showRelevantDoc.Tick += new EventHandler(dis_Timer_docREl);
                    timer_showRelevantDoc.Interval = new TimeSpan(0, 0, 1);
                    timer_showRelevantDoc.Start();
                    isFInieshed_doc_Rel = false;

                    new Thread(delegate()
                    {
                                           

                    System.Windows.MessageBox.Show("Start Searching the relevant docs!");
                    path_quries = Dialog.FileName;
                    FileStream fs = new FileStream(path_quries, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs);
                    Dictionary<int, ArrayList> dic_relDocs_For_query = new Dictionary<int, ArrayList>();
                    while ((line = sr.ReadLine()) != null)
                    {
                        string_num_query = line.Substring(0, line.IndexOf("\t"));
                        if (Int32.TryParse(string_num_query, out i))
                        {
                            num_query = i;
                        }
                        query = line.Substring(line.IndexOf("\t") + 2);

                        string[] query_array = new string[2];
                        query_array[0] = "nothing";
                        query_array[1] = query;
                        queries.Clear();
                        queries.Add(num_query, query_array);
                        ArrayList list_languages_pressed = languages_Pressed_LIstBox;
                      
                        if (DoStemming==true)
                        {
                            Searcher searcher = new Searcher(terms_dictionary_stemming, queries, list_languages_pressed, total_lenght_doc, doc_dic, path_corpus + "/stop_words.txt", path_posting, path_corpus + "/doc", DoStemming, false, "", path_queries_ranked);
                            if (!dic_relDocs_For_query.ContainsKey(num_query))
                            {
                                dic_relDocs_For_query.Add(num_query, searcher.REL_DOCS);
                            }
                            else
                            {
                                Random r = new Random();
                                num_query = r.Next(1000, 2000);
                                if (!dic_relDocs_For_query.ContainsKey(num_query))
                                {
                                    dic_relDocs_For_query.Add(num_query, searcher.REL_DOCS);
                                    num_query++;
                                }
                            }       
                        }
                        else // no stemming
                        {
                            Searcher searcher = new Searcher(terms_dictionary, queries, list_languages_pressed, total_lenght_doc, doc_dic, path_corpus + "/stop_words.txt", path_posting, path_corpus + "/doc", DoStemming, false, "", path_queries_ranked);
                            if (!dic_relDocs_For_query.ContainsKey(num_query))
                            {
                                dic_relDocs_For_query.Add(num_query, searcher.REL_DOCS);
                                
                            }
                            else
                            { Random r = new Random();
                              num_query = r.Next(1000,2000);
                              if (!dic_relDocs_For_query.ContainsKey(num_query))
                              {
                                  dic_relDocs_For_query.Add(num_query, searcher.REL_DOCS);
                                  num_query++;
                              }
                            }
                        }
                       
                    }
                    m_dic_relDocs_For_query = dic_relDocs_For_query;
                    isFInieshed_doc_Rel = true;
                    Thread.Sleep(1000);
                    timer_showRelevantDoc.Stop();

                    }).Start();

                }
                else
                {
                    System.Windows.MessageBox.Show("The path of the File is empty!");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("You have to choose path for saving the queries!");
            }

        }

        //check if t hread finished rel_docs
        private void dis_Timer_docREl(object sender, EventArgs e)
        {
            try
            {                
                if (isFInieshed_doc_Rel == true)
                {
                    showRelevantdocs window_rel_docs = new showRelevantdocs(m_dic_relDocs_For_query);
                    window_rel_docs.ShowDialog();
                    isFInieshed_doc_Rel = false;
                }
            }
            catch (Exception e11)
            {
                System.Windows.MessageBox.Show(e11.Message);
            }

        }
        
        // search the query =that was typed=
        private void startQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<int, string[]> query_dic = new Dictionary<int, string[]>();
                path_corpus = TextBoxCorpus.Text;
                path_posting = TextBoxPosting.Text;
                string The_Query_typed = queryTextBox.Text;
                string[] query_array = new string[2];
                if (path_corpus != "" && path_posting != "" && path_queries_ranked != "" && queryTextBox.Text != "")
                {
                    System.Windows.MessageBox.Show("Start searching for relevant docs!");
                    query_array[0] = "nothing";
                    query_array[1] = The_Query_typed;
                    query_dic.Add(number_Query, query_array);
                    number_Query++;
                    ArrayList list_languages_pressed = languages_Pressed_LIstBox;
                    bool DoStemming;
                    if (checkBoxStem.IsChecked == true)
                    {
                        DoStemming = true;
                        Searcher s = new Searcher(terms_dictionary_stemming, query_dic, list_languages_pressed, total_lenght_doc, doc_dic, path_corpus + "/stop_words.txt", path_posting, path_corpus + "/doc", DoStemming, false, "", path_queries_ranked);
                        Dictionary<int, ArrayList> dic_temp = new Dictionary<int, ArrayList>();
                        dic_temp.Add(number_Query - 1, s.REL_DOCS);
                        showRelevantdocs window_rel_doc = new showRelevantdocs(dic_temp);
                        window_rel_doc.ShowDialog();
                    }
                    else
                    {
                        DoStemming = false;
                        Searcher s = new Searcher(terms_dictionary, query_dic, list_languages_pressed, total_lenght_doc, doc_dic, path_corpus + "/stop_words.txt", path_posting, path_corpus + "/doc", DoStemming, false, "", path_queries_ranked);
                        Dictionary<int, ArrayList> dic_temp = new Dictionary<int, ArrayList>();
                        dic_temp.Add(number_Query - 1, s.REL_DOCS);
                        showRelevantdocs window_rel_doc = new showRelevantdocs(dic_temp);
                        window_rel_doc.ShowDialog();
                    }
                   
                }
                else
                {
                    System.Windows.MessageBox.Show("The path of the posting and dictinary and saving files is empty!");
                }
            }
            catch (Exception e2)
            {
                System.Windows.MessageBox.Show(e2.Message);
            }
        }
        //check if the contentwords was changed
        private void contentWords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string path_corpus = TextBoxCorpus.Text;
                if (path_corpus != "")
                {
                    string connected_word = "";
                    // get the item from list
                    foreach (Object selecteditem in contentWordsList.SelectedItems)
                    {
                        connected_word = selecteditem as String;
                    }
                    if (connected_word != "")
                    {
                        //enter the choose of the word
                        queryTextBox.Text = connected_word;
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("The path of the doc is empty!");
                }
            }
            catch (Exception e8)
            {
                System.Windows.MessageBox.Show(e8.Message);
            }
        }

        // check the query if typed space
        private void queryTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e != null && e.Key == Key.Space)
                {
                    char[] delimiters = { ' ' };
                    string[] words_from_query = queryTextBox.Text.Split(delimiters);
                    if (words_from_query.Length < 2 && queryTextBox.Text.Length > 0)
                    {
                        //no stemming
                        if (checkBoxStem.IsChecked == false)
                        {
                            if (terms_connectedWords_NOstemming.Count != 0)
                            {
                                if (terms_connectedWords_NOstemming.ContainsKey(queryTextBox.Text))
                                {
                                    ArrayList all_connectrd_words = terms_connectedWords_NOstemming[queryTextBox.Text];
                                    for (int i = 0; i < 5 && i < all_connectrd_words.ToArray().Length; i++)
                                    {
                                        var t = all_connectrd_words.ToArray().GetValue(i);
                                        string name_connecected = ((Term)t).NAME;
                                        contentWordsList.Items.Add(name_connecected);
                                    }
                                }
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("You have to load the dictionry and posting without stemming!");
                            }

                        }
                        // with stemming
                        else
                        {
                            if (terms_connectedWords_stemming.Count != 0)
                            {
                                if (terms_connectedWords_stemming.ContainsKey(queryTextBox.Text))
                                {
                                    ArrayList all_connectrd_words = terms_connectedWords_stemming[queryTextBox.Text];
                                    for (int i = 0; i < 5 && i < all_connectrd_words.ToArray().Length; i++)
                                    {
                                        var t = all_connectrd_words.ToArray().GetValue(i);
                                        string name_connecected = ((Term)t).NAME;
                                        contentWordsList.Items.Add(name_connecected);
                                    }
                                }
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("You have to load the dictionry and posting with stemming!");
                            }
                        }
                        //end else
                    }

                    // conatins more than one word
                    else if (words_from_query.Length > 1)
                    {
                        contentWordsList.Items.Clear();
                    }
                }

                // clean the list
                else if (e != null && queryTextBox.Text == "")
                {
                    contentWordsList.Items.Clear();
                }
            }

            catch (Exception e8)
            {
                System.Windows.MessageBox.Show(e8.Message);
            }

        }

        //save the files for the queries
        private void SaveQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                path_corpus = TextBoxCorpus.Text;
                path_posting = TextBoxPosting.Text;
                if (path_corpus != "" && path_posting != "")
                {
                    if (checkBoxStem.IsChecked == true) // do stemming
                    {
                        if (terms_dictionary_stemming.Count == 0 || terms_connectedWords_stemming.Count == 0)
                        {
                            System.Windows.MessageBox.Show("You have to load the dictionary first!");
                        }

                        else
                        {
                            FolderBrowserDialog fbd = new FolderBrowserDialog();
                            DialogResult result = fbd.ShowDialog();
                            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                            {
                                path_queries_ranked = fbd.SelectedPath + "/queries_ranked_stemming.txt";
                                TextBoxQuriesPath.Text = path_queries_ranked;
                                if (!File.Exists(path_queries_ranked))
                                {
                                    FileStream fs = new FileStream(path_queries_ranked, FileMode.Create);
                                    fs.Close();
                                }                                
                                //File.Create(path_queries_ranked);
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("The path of the query file to save!");
                            }
                        }
                    }
                    else // no stemming
                    {
                        if (terms_connectedWords_NOstemming.Count == 0 || terms_dictionary.Count == 0)
                        {
                            System.Windows.MessageBox.Show("You have to load the dictionary first!");
                        }

                        else
                        {
                            FolderBrowserDialog fbd = new FolderBrowserDialog();
                            DialogResult result = fbd.ShowDialog();
                            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                            {
                                path_queries_ranked = fbd.SelectedPath + "/queries_ranked.txt";
                                TextBoxQuriesPath.Text = path_queries_ranked;
                                if (!File.Exists(path_queries_ranked))
                                {
                                    FileStream fs = new FileStream(path_queries_ranked, FileMode.Create);
                                    fs.Close();
                                }      
                                //File.Create(path_queries_ranked);
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("The path of the query file to save!");
                            }
                        }
                    }
                }

                else
                {
                    System.Windows.MessageBox.Show("The path of the Posting and Dictionary is empty!");
                }

            }
            catch (Exception e9)
            {
                System.Windows.MessageBox.Show(e9.Message);
            }
        }

    }
}
