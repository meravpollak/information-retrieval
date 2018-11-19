using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IR
{
    public class Parse
    {
        ArrayList m_rel_doc;
        int m_query_num;
        // parse the terms 
        public void parse(Dictionary<int, string[]> d, string path_stopWords, string path_newFile, string path_docs, bool Dostemming, bool Doindexer, string path_wordsConnected, int total_lenght_doc, Dictionary<string, Document> doc_dic, Dictionary<string, Term> terms_dictionary, ArrayList list_languages_pressed, string path_queries_ranked)
        {

            Dictionary<int, string[]> dicOfTerms = new Dictionary<int, string[]>();
            Dictionary<string, ArrayConnectedwords> final_connectWords = new Dictionary<string, ArrayConnectedwords>();
            LinkedList<WordsConnected> list_wordsCOnnected = new LinkedList<WordsConnected>();
            ArrayList documents = new ArrayList(); // create a list of documnets
            char[] delimiters = { ' ' };
            HashSet<string> stopWords = new HashSet<string>();
            stopWords = StopWords(path_stopWords);

            //create month dictionary
            Dictionary<string, string> MonthDic = new Dictionary<string, string>();
            MonthDic = CreateDic(MonthDic);

            //int how = 1;
            foreach (int NumOfFile in d.Keys)
            {
                string[] words = d[NumOfFile][1].Split(delimiters);

                string[] AfterParse = new String[words.Length * 2];


                //delete from the end the ; . , : ) * 
                for (int i = 0; i < words.Length; i++)
                {
                    if (((words[i] != "") && (words[i].ToLower() != "u.s.")) && ((words[i].Substring(words[i].Length - 1, 1) == "-" || words[i].Substring(words[i].Length - 1, 1) == "!" || words[i].Substring(words[i].Length - 1, 1) == "?" || words[i].Substring(words[i].Length - 1, 1) == "\"" || words[i].Substring(words[i].Length - 1, 1) == ";" || words[i].Substring(words[i].Length - 1, 1) == "." || words[i].Substring(words[i].Length - 1, 1) == "," || words[i].Substring(words[i].Length - 1, 1) == ":" || words[i].Substring(words[i].Length - 1, 1) == ")" || words[i].Substring(words[i].Length - 1, 1) == "*")))
                    {
                        words[i] = words[i].Substring(0, words[i].Length - 1);
                    }
                }

                int counterWords = 0;
                int counterAfterParse = 0;
                double i1;
                double i2;
                bool CheckIFEnter = false;

                while (counterWords < words.Length)
                {

                    //delete the , and the .  , ; * ) : from the end of the string
                    while ((words[counterWords] != "" && (words[counterWords].ToLower() != "u.s.")) && (words[counterWords].Substring(words[counterWords].Length - 1, 1) == "]" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "{" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "(" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "," || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "'" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "|" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "`" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "\"" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "." || words[counterWords].Substring(words[counterWords].Length - 1, 1) == ";" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "*" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "?" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == ")" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == ":"))
                    {
                        words[counterWords] = words[counterWords].Substring(0, words[counterWords].Length - 1);
                    }

                    //delete * " ( from the beginning
                    while ((words[counterWords] != "") && ((words[counterWords].Substring(0, 1) == "(") || words[counterWords].Substring(0, 1) == "\\" || words[counterWords].Substring(0, 1) == "}" || words[counterWords].Substring(0, 1) == ")" || words[counterWords].Substring(0, 1) == "@" || words[counterWords].Substring(0, 1) == ";" || words[counterWords].Substring(0, 1) == ":" || words[counterWords].Substring(0, 1) == "!" || words[counterWords].Substring(0, 1) == "%" || words[counterWords].Substring(0, 1) == "=" || words[counterWords].Substring(0, 1) == "]" || words[counterWords].Substring(0, 1) == ")" || words[counterWords].Substring(0, 1) == "+" || words[counterWords].Substring(0, 1) == "|" || words[counterWords].Substring(0, 1) == "'" || (words[counterWords].Substring(0, 1) == ".") || words[counterWords].Substring(0, 1) == "`" || (words[counterWords].Substring(0, 1) == ",") || (words[counterWords].Substring(0, 1) == "?") || (words[counterWords].Substring(0, 1) == "&") || (words[counterWords].Substring(0, 1) == "[") || words[counterWords].Substring(0, 1) == "/" || words[counterWords].Substring(0, 1) == "_" || (words[counterWords].Substring(0, 1) == "\"") || (words[counterWords].Substring(0, 1) == "-") || (words[counterWords].Substring(0, 1) == "*")))
                    {
                        words[counterWords] = words[counterWords].Substring(1, words[counterWords].Length - 1);
                    }


                    // delete foe the next word .  , ; * ) : from the end of the string
                    if ((counterWords + 1 < words.Length))
                    {
                        counterWords++;
                        //delete the , and the .  , ; * ) : from the end of the string
                        while ((words[counterWords] != "" && (words[counterWords].ToLower() != "u.s.")) && (words[counterWords].Substring(words[counterWords].Length - 1, 1) == "]" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "," || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "'" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "|" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "`" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "\"" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "." || words[counterWords].Substring(words[counterWords].Length - 1, 1) == ";" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "*" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == "?" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == ")" || words[counterWords].Substring(words[counterWords].Length - 1, 1) == ":"))
                        {
                            words[counterWords] = words[counterWords].Substring(0, words[counterWords].Length - 1);
                        }
                        counterWords--;
                    }

                    if (Dostemming == true)
                    {
                        Stemmer s = new Stemmer();
                        words[counterWords] = s.stemTerm(words[counterWords]);
                    }

                    // check if it is  date - [16TH]
                    if ((words[counterWords] != "") && (words[counterWords].Length == 4 && words[counterWords].Substring(2).ToLower() == "th") && (Double.TryParse(words[counterWords].Substring(0, 2), out i1)))
                    {
                        words[counterWords] = words[counterWords].Substring(0, 2);
                    }
                    // check if it is  date - [9TH]
                    if ((words[counterWords] != "") && (words[counterWords].Length == 3 && words[counterWords].Substring(1).ToLower() == "th") && (Double.TryParse(words[counterWords].Substring(0, 1), out i1)))
                    {
                        words[counterWords] = words[counterWords].Substring(0, 2);
                    }
                    //check if the term is number in the end
                   if ((words[counterWords] != "") && (Double.TryParse(words[counterWords], out i2)) && (words.Length==counterWords+1))
                   {
                       AfterParse[counterAfterParse] = words[counterWords];
                       CheckIFEnter = true;
                       counterAfterParse++;
                   }   //check if it is NUMBER
                    else if ((words[counterWords] != "") && (Double.TryParse(words[counterWords], out i2) && !MonthDic.ContainsKey(words[counterWords + 1])))
                    {   
                        double number = Double.Parse(words[counterWords]);


                        // if the string is up to miliion
                        if (number > 999999)
                        {
                            //the last word in the array
                            if ((counterWords + 1) == words.Length)
                            {
                                AfterParse[counterAfterParse] = IfNumber(Double.Parse(words[counterWords]), "");
                                CheckIFEnter = true;
                            }
                            else
                            {
                                // word in the middle
                                AfterParse[counterAfterParse] = IfNumber(Double.Parse(words[counterWords]), words[counterWords + 1]);
                                counterAfterParse++;
                                CheckIFEnter = true;
                                if (Dostemming == true)
                                {
                                    Stemmer s = new Stemmer();
                                    words[counterWords + 1] = s.stemTerm(words[counterWords + 1]);
                                }
                                if (words[counterWords + 1].ToLower() == "million" || words[counterWords + 1].ToLower() == "billion" || words[counterWords + 1].ToLower() == "trillion")
                                {
                                    counterWords++;
                                }
                            }
                        }

                        else // the string is less than million [ num < MILLION]
                        {
                            //the last word in the array
                            if ((counterWords + 1) == words.Length)
                            {
                                AfterParse[counterAfterParse] = words[counterWords];
                                CheckIFEnter = true;
                            }
                            //if the string is SHEVER [ LIKE 3/4 ] 
                            else if (words[counterWords + 1].Contains("/"))
                            {
                                int i = words[counterWords + 1].IndexOf("/");
                                double i3, i4;
                                if (Double.TryParse(words[counterWords + 1].Substring(0, i), out i3) && Double.TryParse(words[counterWords + 1].Substring(i + 1, words[counterWords + 1].Length - i - 1), out i4))
                                {
                                    double new_num = i3 / i4;
                                    i2 = i2 + new_num;
                                    AfterParse[counterAfterParse] = i2.ToString();
                                    counterWords++;
                                    counterAfterParse++;
                                    CheckIFEnter = true;
                                }
                                //AfterParse[counterAfterParse] = words[counterWords] + " " + words[counterWords + 1];

                            }
                            else // the string is in the middle
                            {
                                //check if there is enough words to end
                                if (counterWords + 1 < words.Length)
                                {
                                    //this month
                                    if (MonthDic.ContainsKey(words[counterWords + 1].ToLower()))
                                    {
                                        // check if the string is not in the end
                                        if (counterWords + 2 < words.Length)
                                        {
                                            // this is NUMBER MONTH NUMBER [ LIKE 14 MAY 1991]
                                            if (Double.TryParse(words[counterWords + 2], out i1))
                                            {
                                                // this is 2 DIGIT NUMBER [LIKE 91]
                                                if (words[counterWords + 2].Length == 2)
                                                {
                                                    if (i2 < 10)
                                                    {
                                                        words[counterWords] = "0" + words[counterWords];
                                                    }
                                                    AfterParse[counterAfterParse] = "19" + words[counterWords + 2] + "-" + MonthDic[words[counterWords + 1].ToLower()] + "-" + words[counterWords];
                                                    CheckIFEnter = true;
                                                    counterAfterParse++;
                                                    counterWords = counterWords + 2;
                                                }

                                                // this is 4 DIGITS NUMBER [LIKE 1991]
                                                else if (words[counterWords + 2].Length == 4)
                                                {
                                                    if (i2 < 10)
                                                    {
                                                        words[counterWords] = "0" + words[counterWords];
                                                    }
                                                    AfterParse[counterAfterParse] = words[counterWords + 2] + "-" + MonthDic[words[counterWords + 1].ToLower()] + "-" + words[counterWords];
                                                    CheckIFEnter = true;
                                                    counterAfterParse++;
                                                    counterWords = counterWords + 2;
                                                }
                                            }
                                            // this is NUMBER MONTH [ 14 MAY ]
                                            else
                                            {
                                                double ii;
                                                // check if the number is small than 10. than change to 03
                                                if (Double.TryParse(words[counterWords], out ii))
                                                {
                                                    // check if the number is small than 10. than change to 03
                                                    if (ii < 10)
                                                    {
                                                        words[counterWords] = "0" + words[counterWords];
                                                    }
                                                }
                                                AfterParse[counterAfterParse] = MonthDic[words[counterWords + 1].ToLower()] + "-" + words[counterWords];
                                                CheckIFEnter = true;
                                                counterAfterParse++;
                                                counterWords++;
                                            }

                                        }
                                        // this is only NUMBER MONTH in the end of the string! [14 MAY]
                                        else
                                        {
                                            double ii;
                                            // check if the number is small than 10. than change to 03
                                            if (Double.TryParse(words[counterWords], out ii))
                                            {
                                                // check if the number is small than 10. than change to 03
                                                if (ii < 10)
                                                {
                                                    words[counterWords] = "0" + words[counterWords];
                                                }
                                            }
                                            AfterParse[counterAfterParse] = MonthDic[words[counterWords + 1].ToLower()] + "-" + words[counterWords];
                                            CheckIFEnter = true;
                                            counterAfterParse++;
                                            counterWords++;

                                        }

                                    }
                                    else //check if this is a Million/Trillion/billion
                                    {
                                        if (Dostemming == true)
                                        {
                                            Stemmer s = new Stemmer();
                                            words[counterWords + 1] = s.stemTerm(words[counterWords + 1]);
                                        }
                                        if (words[counterWords + 1].ToLower() == "million" || words[counterWords + 1].ToLower() == "billion" || words[counterWords + 1].ToLower() == "trillion")
                                        {
                                            AfterParse[counterAfterParse] = IfNumber(i2, words[counterWords + 1]);
                                            counterAfterParse++;
                                            counterWords++;
                                        }
                                    }
                                }
                                else // the number is at the end of the array
                                {
                                    AfterParse[counterAfterParse] = words[counterWords];
                                    CheckIFEnter = true;
                                    counterAfterParse++;
                                }

                            }
                        }

                    }

                    //check if in the phrase is BETWEEN
                    if ((words[counterWords] != "") && words[counterWords].ToLower() == "between")
                    {
                        if (counterWords + 3 < words.Length)

                            //check if it BETWEEN NUMBER and NUMBER

                            if (Double.TryParse(words[counterWords + 1], out i1) || double.TryParse(words[counterWords + 3], out i1))
                            {
                                AfterParse[counterAfterParse] = words[counterWords] + " " + words[counterWords + 1] + " " + words[counterWords + 2] + " " + words[counterWords + 3];
                                CheckIFEnter = true;
                                counterAfterParse++;
                                counterWords = counterWords + 3;
                            }
                            else // not contain a numberic variable
                            {
                                AfterParse[counterAfterParse] = words[counterWords];
                                CheckIFEnter = true;
                                counterAfterParse++;
                            }
                    }

                    //check if it is percenct and add %
                    if ((words[counterWords] != "") && (words[counterWords].ToLower() == "percent" || words[counterWords].ToLower() == "percentage"))
                    {
                        if (counterWords != 0)
                        {
                            // this in number in front of him [ like 9 % ]
                            if ((words[counterWords - 1] != "") && (Double.TryParse((AfterParse[counterAfterParse - 1]), out i1)))
                            {
                                AfterParse[counterAfterParse - 1] = AfterParse[counterAfterParse - 1] + "%";

                                CheckIFEnter = true;
                            }
                        }

                    }

                    //check if it is dollar and add DOLLARS WITH STEMMING
                    if (Dostemming == true)
                    {
                        if (words[counterWords] != "" && (words[counterWords].ToLower() == "dollar"))
                        {
                            if (Double.TryParse(words[counterWords - 1], out i2) || words[counterWords - 1].Contains("/"))
                            {
                                AfterParse[counterAfterParse - 1] = AfterParse[counterAfterParse - 1] + " Dollars";
                                CheckIFEnter = true;
                            }
                        }
                    }

                    //check if it is dollar and add DOLLARS
                    if (Dostemming == false)
                    {
                        if (words[counterWords] != "" && (words[counterWords].ToLower() == "dollars"))
                        {
                            if (Double.TryParse(words[counterWords - 1], out i2) || words[counterWords - 1].Contains("/"))
                            {
                                AfterParse[counterAfterParse - 1] = AfterParse[counterAfterParse - 1] + " Dollars";
                                CheckIFEnter = true;
                            }
                        }
                    }

                    //check if it is contain $ and add DOLLARS
                    if (words[counterWords] != "" && words[counterWords].Substring(0, 1) == ("$"))
                    {
                        words[counterWords] = words[counterWords].Substring(1);
                        //check that it is a NUMBER
                        if (Double.TryParse(words[counterWords], out i1))
                        {
                            // more than 999999
                            if (i1 > 999999)
                            {
                                AfterParse[counterAfterParse] = IfNumber(i1, "") + " Dollars";
                                CheckIFEnter = true;
                                counterAfterParse++;
                            }
                            else // less than 999999 ( i1 < 999999 )
                            {
                                if (Dostemming == true)
                                {
                                    Stemmer s = new Stemmer();
                                    words[counterWords + 1] = s.stemTerm(words[counterWords + 1]);
                                }
                                // the next word is million or billion or trillion
                                if (words[counterWords + 1].ToLower() == "million" || words[counterWords + 1].ToLower() == "trillion" || words[counterWords + 1].ToLower() == "billion")

                                {
                                    AfterParse[counterAfterParse] = IfNumber(i1, words[counterWords + 1]) + " Dollars";
                                    CheckIFEnter = true;
                                    counterWords++;
                                    counterAfterParse++;
                                }
                                else // NO next word 
                                {
                                    AfterParse[counterAfterParse] = words[counterWords] + " Dollars";
                                    CheckIFEnter = true;
                                    counterAfterParse++;
                                }

                            }
                        }

                    }

                    //check for tha MONTH
                    if (words[counterWords] != "" && MonthDic.ContainsKey(words[counterWords].ToLower()))
                    {
                        if (counterWords + 2 < words.Length)
                        {
                            // LIKE APRIL 28, 1990
                            if (Double.TryParse(words[counterWords + 2], out i2) && (Double.TryParse(words[counterWords + 1], out i1)))
                            {
                                // ADD 0 if the number is under 10
                                if (i1 < 10)
                                {
                                    words[counterWords + 1] = "0" + words[counterWords + 1];
                                }

                                AfterParse[counterAfterParse] = words[counterWords + 2] + "-" + MonthDic[words[counterWords].ToLower()] + "-" + words[counterWords + 1].Substring(0, words[counterWords + 1].Length - 1);
                                CheckIFEnter = true;
                                counterAfterParse++;
                                counterWords = counterWords + 2;
                            }

                            // LIKE APRIL 28
                            else
                            {
                                if (Double.TryParse(words[counterWords + 1], out i2))
                                {
                                    // MONTH DD [ APRIL 4 ]
                                    if (i2 < 32)
                                    {
                                        // ADD 0 if the number is under 10
                                        if (i2 < 10)
                                        {
                                            words[counterWords + 1] = "0" + words[counterWords + 1];
                                        }
                                        AfterParse[counterAfterParse] = MonthDic[words[counterWords].ToLower()] + "-" + words[counterWords + 1];
                                        CheckIFEnter = true;
                                        counterAfterParse++;
                                        counterWords = counterWords++;
                                    }
                                    // if it is MONTH YEAR [ APRIL 1991]
                                    else
                                    {
                                        // ADD 0 if the number is under 10
                                        if (words[counterWords + 1] != "")
                                        {
                                            AfterParse[counterAfterParse] = words[counterWords + 1] + "-" + MonthDic[words[counterWords].ToLower()];
                                            CheckIFEnter = true;
                                            counterAfterParse++;
                                            counterWords++;
                                        }

                                    }
                                }

                            }

                        }
                        // the string is in the end! [LIKE APRIL 28]
                        else if (counterWords + 1 < words.Length)
                        {
                            if (Double.TryParse(words[counterWords + 1], out i2))
                            {
                                // MONTH DD [ APRIL 4 ]
                                if (i2 < 32)
                                {
                                    // ADD 0 if the number is under 10
                                    if (i2 < 10)
                                    {
                                        words[counterWords + 1] = "0" + words[counterWords + 1];
                                    }
                                    AfterParse[counterAfterParse] = MonthDic[words[counterWords].ToLower()] + "-" + words[counterWords + 1];
                                    CheckIFEnter = true;
                                    counterAfterParse++;
                                    counterWords = counterWords++;
                                }
                                // if it is MONTH YEAR [ APRIL 1991]
                                else
                                {
                                    AfterParse[counterAfterParse] = words[counterWords + 1] + "-" + MonthDic[words[counterWords].ToLower()];
                                    CheckIFEnter = true;
                                    counterAfterParse++;
                                    counterWords++;
                                }
                            }
                        }

                        else // the MONTH is in the end of the string
                        {
                            AfterParse[counterAfterParse] = words[counterWords];
                            CheckIFEnter = true;
                            counterAfterParse++;
                        }
                    }

                    // if the case is 100bn DOLLARS
                    if ((words[counterWords] != "" && words[counterWords].Length > 1))
                    {
                        if (words[counterWords].Substring(words[counterWords].Length - 2, 2).ToLower() == "bn")
                        {
                            if (Double.TryParse(words[counterWords].Substring(0, words[counterWords].Length - 2), out i1))
                            {
                                double num = i1 * 1000;
                                if (Dostemming == true)
                                {
                                    Stemmer s = new Stemmer();
                                    words[counterWords + 1] = s.stemTerm(words[counterWords + 1]);
                                    if (words[counterWords + 1].ToLower() == "dollar")
                                    {
                                        AfterParse[counterAfterParse] = num.ToString() + " M Dollars";
                                        CheckIFEnter = true;
                                    }
                                }

                                else if (words[counterWords + 1].ToLower() == "dollars")
                                {
                                    AfterParse[counterAfterParse] = num.ToString() + " M Dollars";
                                    CheckIFEnter = true;
                                }

                            }

                        }

                    }

                    // for case 100 billion U.S dollars
                    if ((words[counterWords] != "" && counterWords + 2 < words.Length))
                    {
                        if (Dostemming == true)
                        {
                            Stemmer s = new Stemmer();
                            words[counterWords + 1] = s.stemTerm(words[counterWords + 1]);
                            if (words[counterWords].ToLower() == "u.s." && words[counterWords + 1].ToLower() == "dollar")
                            {
                                AfterParse[counterAfterParse - 1] = AfterParse[counterAfterParse - 1] + " Dollars";
                                CheckIFEnter = true;
                            }

                        }
                        else if (words[counterWords].ToLower() == "u.s." && words[counterWords + 1].ToLower() == "dollars")
                        {
                            AfterParse[counterAfterParse - 1] = AfterParse[counterAfterParse - 1] + " Dollars";
                            CheckIFEnter = true;
                        }
                    }

                    // case of Price UP to Million = 20.6m Dollars
                    if ((words[counterWords] != "" && counterWords + 1 < words.Length))
                    {
                        if (Dostemming == true)
                        {
                            Stemmer s = new Stemmer();
                            words[counterWords + 1] = s.stemTerm(words[counterWords + 1]);
                            if (words[counterWords].Substring(words[counterWords].Length - 1, 1).ToLower() == "m" && Double.TryParse(words[counterWords].Substring(0, words[counterWords].Length - 1), out i1) && words[counterWords + 1].ToLower() == "dollar")
                            {
                                AfterParse[counterAfterParse] = i1.ToString() + " M Dollars";
                                CheckIFEnter = true;
                                counterAfterParse++;
                                counterWords++;
                            }
                        }
                        else if (words[counterWords].Substring(words[counterWords].Length - 1, 1).ToLower() == "m" && Double.TryParse(words[counterWords].Substring(0, words[counterWords].Length - 1), out i1) && words[counterWords + 1].ToLower() == "dollars")
                        {
                            AfterParse[counterAfterParse] = i1.ToString() + " M Dollars";
                            CheckIFEnter = true;
                            counterAfterParse++;
                            counterWords++;
                        }
                    }

                    // if it is just a REGULAR STRING
                    if (words[counterWords] != "" && CheckIFEnter == false)
                    {
                        if (words[counterWords].Contains("/"))
                        {
                            int index = words[counterWords].IndexOf("/");
                            if (!Double.TryParse(words[counterWords].Substring(0, index), out i1))
                            {
                                AfterParse[counterAfterParse] = words[counterWords].Substring(0, index);
                                counterAfterParse++;

                                //contain more than 1 /
                                string temp_string = words[counterWords].Substring(index + 1, words[counterWords].Length - index - 1);
                                //delete * " ( from the beginning
                                while ((temp_string != "") && ((temp_string.Substring(0, 1) == "(") || temp_string.Substring(0, 1) == "}" || temp_string.Substring(0, 1) == ")" || temp_string.Substring(0, 1) == "[" || temp_string.Substring(0, 1) == "/" || temp_string.Substring(0, 1) == "|" || temp_string.Substring(0, 1) == "]" || temp_string.Substring(0, 1) == ";" || temp_string.Substring(0, 1) == ":" || temp_string.Substring(0, 1) == "_" || temp_string.Substring(0, 1) == "@" || temp_string.Substring(0, 1) == "=" || temp_string.Substring(0, 1) == "+" || temp_string.Substring(0, 1) == "!" || temp_string.Substring(0, 1) == "%" || temp_string.Substring(0, 1) == "|" || temp_string.Substring(0, 1) == "'" || (temp_string.Substring(0, 1) == ".") || temp_string.Substring(0, 1) == "`" || (temp_string.Substring(0, 1) == ",") || (temp_string.Substring(0, 1) == "?") || (temp_string.Substring(0, 1) == "&") || (temp_string.Substring(0, 1) == "[") || (temp_string.Substring(0, 1) == "\"") || (temp_string.Substring(0, 1) == "-") || (temp_string.Substring(0, 1) == "*")))
                                {
                                    temp_string = temp_string.Substring(1, temp_string.Length - 1);
                                }
                                while (temp_string.Contains("/"))
                                {

                                    index = temp_string.IndexOf("/");
                                    AfterParse[counterAfterParse] = temp_string.Substring(0, index);
                                    counterAfterParse++;
                                    temp_string = temp_string.Substring(index + 1, temp_string.Length - index - 1);
                                }

                                AfterParse[counterAfterParse] = temp_string;
                                counterAfterParse++;
                            }
                        }
                        else
                        {
                            // check if it is not contain the stop words
                            if (!stopWords.Contains(words[counterWords].ToLower()))
                            {
                                if (words[counterWords].Substring(0, 1) != "-")
                                {
                                    AfterParse[counterAfterParse] = words[counterWords];
                                    counterAfterParse++;
                                    CheckIFEnter = false;
                                    // add the list the words connected
                                    list_wordsCOnnected.AddLast(new WordsConnected(words[counterWords], counterAfterParse - 1));
                                }
                            }
                        }
                    }

                    counterWords++;
                    CheckIFEnter = false;

                }
                if (Doindexer == true)
                {
                    Document doc = new Document(d[NumOfFile][2], AfterParse, counterAfterParse-1, d[NumOfFile][0], d[NumOfFile][3]);
                    documents.Add(doc);
                    while (list_wordsCOnnected.Count >= 2)
                    {
                        WordsConnected obj1 = ((WordsConnected)list_wordsCOnnected.First());
                        list_wordsCOnnected.RemoveFirst();
                        WordsConnected obj2 = ((WordsConnected)list_wordsCOnnected.First());
                        list_wordsCOnnected.RemoveFirst();
                        // check if the string is connected - one string is after the other
                        if (obj1.LOCATION + 1 == obj2.LOCATION)
                        {
                            string key = obj1.NAME + " " + obj2.NAME;
                            // check if it is in the dictionary
                            if (final_connectWords.ContainsKey(key))
                            {
                                if (final_connectWords[key].DICTIONARY.ContainsKey(d[NumOfFile][2]))// if contains name of file
                                {
                                    final_connectWords[key].DICTIONARY[d[NumOfFile][2]]++;// add the num of apperance
                                }
                            }
                            //not in the dictionary
                            else
                            {
                                final_connectWords.Add(key, new ArrayConnectedwords(d[NumOfFile][2]));
                                final_connectWords[key].DICTIONARY.Add(d[NumOfFile][2], 1);
                            }
                        }
                        list_wordsCOnnected.AddFirst(obj2);

                    }
                }
                else // enter the string after the parsing
                {
                    d[NumOfFile] = AfterParse;
                    ArrayList arrayList = new ArrayList();
                    for (int i = 0; i < AfterParse.Length; i++)
                    {
                        if (AfterParse[i]!=null)
                        {
                            arrayList.Add(AfterParse[i]);
                        }                    
                    }
                    string[] final_after_parsing = new string[arrayList.Count];
                    int j = 0;
                    foreach (string val in arrayList)
                    {
                        final_after_parsing[j] = val;
                        j++;
                    }
                    // create the final dictionary for queries
                    Dictionary<int, string[]> final_dic_query = new Dictionary<int, string[]>();
                    // get the num of the query
                    foreach (int num_query in d.Keys)
                    {
                        final_dic_query.Add(num_query, final_after_parsing);
                    }
                    //sent to the Ranker!                     
                    Ranker r = new Ranker(terms_dictionary, doc_dic, final_dic_query, total_lenght_doc, Dostemming, path_newFile, list_languages_pressed, path_queries_ranked);
                    ArrayList list_rel = new ArrayList();
                    m_rel_doc = new ArrayList();
                    Dictionary<string, double> temp_dic = new Dictionary<string, double>();
                    temp_dic = r.TOP50DOCS;
                    foreach (string doc in temp_dic.Keys)
                    {
                        list_rel.Add(doc);
                    }                    
                    m_rel_doc = list_rel;
                    m_query_num = r.QUERY_NUM; ////////////////////////////////
                    return;             
                }
            }

            //for the documents
            if (Doindexer == true)
            {
                // string chaining = "";
                Dictionary<string, ArrayConnectedwords> Dic_connect_new = new Dictionary<string, ArrayConnectedwords>();

                foreach (var name in final_connectWords.OrderBy(i => i.Key))
                {
                    Dic_connect_new[name.Key] = name.Value;
                }

                FileStream fs = new FileStream(path_wordsConnected, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                foreach (string term in Dic_connect_new.Keys)
                {
                    sw.Write(term + "/");
                    foreach (string nameDoc in Dic_connect_new[term].DICTIONARY.Keys)
                    {
                        sw.Write(nameDoc + "," + Dic_connect_new[term].DICTIONARY[nameDoc] + ";");
                    }
                    sw.WriteLine();
                }
                sw.Close();
                fs.Close();
                Indexer indexer = new Indexer(documents, path_newFile, path_docs);
            }

            // for the queries
            else
            {

            }

        }


        // up from Million 
        string IfNumber(double number, string nextTerm)
        {
            if (nextTerm != "")
            {

                if (nextTerm.ToLower().Equals("million"))
                {
                    string word = number.ToString() + "M";
                    return word;
                }
                if (nextTerm.ToLower().Equals("billion"))
                {
                    number = number * 1000;
                    string word = number.ToString() + "M";
                    return word;
                }
                if (nextTerm.ToLower().Equals("trillion"))
                {
                    number = number * 1000000;
                    string word = number.ToString() + "M";
                    return word;
                }
                if (number % 1000000 == 0)
                {
                    number = number / 1000000;
                    string word = number.ToString() + "M";
                    return word;
                }
                else if (number >= 1000000)
                {
                    double numberC;
                    numberC = ((double)number / 1000000);
                    string word = numberC.ToString() + "M";
                }
            }
            return number.ToString();

        }

        //return the relevant docs
        public ArrayList RELDOCS
        {
            get { return m_rel_doc; }
        }

        //return the relevant docs
        public int QUERY_NUMBER
        {
            get { return m_query_num; }
        }

        //create arrayLIst of STOP WORDS
        HashSet<string> StopWords(string path)
        {
            HashSet<string> stopWordList = new HashSet<string>();
            //ArrayList stopWordList = new ArrayList();
            StreamReader sr = new StreamReader(path);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                stopWordList.Add(line); //

            }
            sr.Close();
            return stopWordList;
        }
        //create the dictionary
        Dictionary<string, string> CreateDic(Dictionary<string, string> MonthDic)
        {
            MonthDic.Add("jan", "01");
            MonthDic.Add("january", "01");
            MonthDic.Add("feb", "02");
            MonthDic.Add("february", "02");
            MonthDic.Add("mar", "03");
            MonthDic.Add("march", "03");
            MonthDic.Add("apr", "04");
            MonthDic.Add("april", "04");
            MonthDic.Add("may", "05");
            MonthDic.Add("jun", "06");
            MonthDic.Add("june", "06");
            MonthDic.Add("july", "07");
            MonthDic.Add("jul", "07");
            MonthDic.Add("august", "08");
            MonthDic.Add("aug", "08");
            MonthDic.Add("september", "09");
            MonthDic.Add("sep", "09");
            MonthDic.Add("october", "10");
            MonthDic.Add("oct", "10");
            MonthDic.Add("november", "11");
            MonthDic.Add("nov", "11");
            MonthDic.Add("december", "12");
            MonthDic.Add("dec", "12");
            return MonthDic;

        }


    }

}
