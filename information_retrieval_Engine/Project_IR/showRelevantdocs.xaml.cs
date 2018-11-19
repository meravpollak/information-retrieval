using System;
using System.Collections;
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
using System.Windows.Shapes;

namespace Project_IR
{
    /// <summary>
    /// Interaction logic for showRelevantdocs.xaml
    /// </summary>
    public partial class showRelevantdocs : Window
    {
        public showRelevantdocs(Dictionary<int, ArrayList> relevant_docs)
        {            
            InitializeComponent();
            
            foreach (int query in relevant_docs.Keys)
            {
                RelevantDocs.Items.Add("The query Num is : " + query);
                RelevantDocs.Items.Add("Count of Relevant docs: " + relevant_docs[query].Count);              
                foreach (string doc in relevant_docs[query])
                {
                    RelevantDocs.Items.Add(doc);
                }
            }
        }
    }
}
