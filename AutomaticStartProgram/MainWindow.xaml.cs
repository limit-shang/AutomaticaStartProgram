using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Xml;
using System.Data;

namespace AutomaticStartProgram
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 成员变量

        /// <summary>
        /// 工作信息DataTable私有变量
        /// </summary>
        private DataTable dataTableJobs;  

        /// <summary>
        /// 工作信息DataTable的Property
        /// </summary>
        public DataTable DataTableJobs
        {
            get { return dataTableJobs; }
            set { dataTableJobs = value; }
        }


        
        #endregion 成员变量

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Initialization();
        }
        #endregion 构造函数

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialization()
        {

            btnStart.Click += new RoutedEventHandler(btnStart_Click);
            btnSave.Click += new RoutedEventHandler(btnSave_Click);
            buttonAddProgram.Click += new RoutedEventHandler(buttonAddProgram_Click);
            buttonAddJob.Click += new RoutedEventHandler(buttonAddJob_Click);
            // 在屏幕中央显示主窗体
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // 拖放文件到RichTextBox中显示路径事件添加
            richTextBoxProgramPath.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(RichTextBox_DragOver), true);
            richTextBoxProgramPath.AddHandler(RichTextBox.DropEvent, new DragEventHandler(RichTextBox_Drop), true);

            dataGridPathsViewMenu.PreviewMouseDown += new MouseButtonEventHandler(dataGridPathsViewMenu_PreviewMouseDown);
            //dataGridPathsViewMenuItemDelete.Click += new RoutedEventHandler(dataGridPathsViewMenuItemDelete_Click);
            //dataGridJobsView.CurrentCellChanged += new EventHandler<EventArgs>(dataGridJobsView_CurrentCellChanged);
            
            BindPathsData();
            BindJobsData();
            
            if (!System.IO.File.Exists(System.Environment.CurrentDirectory + @"\Data.xml"))
            {
                XMLHelper.CreatePathsXmlDocument();
            }
        }

        void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show("11");
        }

        /// <summary>
        /// 查询子控件
        /// </summary>
        /// <typeparam name="childItem">子控件类型</typeparam>
        /// <param name="obj">母控件</param>
        /// <returns>查到的第一个子控件</returns>
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        

        #endregion 初始化

        #region 事件

        /// <summary>
        /// 启动按钮的点击事件
        /// </summary>
        /// <param name="sender">btnStart</param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartPrograms();

            Application.Current.Shutdown();
            //this.Close();
        }

        /// <summary>
        /// btnSave按钮的点击事件  暂时没用
        /// </summary>
        /// <param name="sender">btnSave</param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<ComboBox> collection = GetChildObjects<ComboBox>(dataGridJobsView, "comboBoxJobState");
            for (int i = 0; i < collection.Count; i++)
            {
                collection[i].SelectionChanged += new SelectionChangedEventHandler(cb_SelectionChanged);
            }
            MessageBox.Show(collection.Count.ToString());
        }
        
        /// <summary>
        /// 将程序拖动到richTextBoxProgramPath边界上发生
        /// </summary>
        /// <param name="sender">richTextBoxProgramPath</param>
        /// <param name="e">拖放事件</param>
        private void RichTextBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = false;
        }

        /// <summary>
        /// 拖放到RichTextBox中放下时发生
        /// </summary>
        /// <param name="sender">richTextBoxProgramPath</param>
        /// <param name="e">拖放事件</param>
        private void RichTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                RichTextBox rtb = (RichTextBox)sender;
                rtb.Document.Blocks.Clear();// 清除内容
                Paragraph p = new Paragraph();
                Run run1 = new Run();
                run1.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();// 获取拖动文件的路径
                p.Inlines.Add(run1);
                rtb.Document.Blocks.Add(p);
            }
        }

        /// <summary>
        /// 添加按钮点击事件
        /// </summary>
        /// <param name="sender">buttonAddProgram</param>
        /// <param name="e"></param>
        private void buttonAddProgram_Click(object sender, RoutedEventArgs e)
        {
            TextRange text = new TextRange(richTextBoxProgramPath.Document.ContentStart, richTextBoxProgramPath.Document.ContentEnd);
            if (ValidatePath(text.Text.Trim()))
            {
                XMLHelper.InsertPathIntoXmlDocument(text.Text.Trim());
                BindPathsData();
                richTextBoxProgramPath.Document.Blocks.Clear();
            }
            else
            {
                MessageBox.Show("当前输入应用程序路径不正确");
            }
        }

        /// <summary>
        /// 添加工作按钮点击事件
        /// </summary>
        /// <param name="sender">buttonAddJob</param>
        /// <param name="e"></param>
        private void buttonAddJob_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxJob.Text.Trim().Length == 0 || textBoxJob.Text.Trim() == "" || textBoxJob.Text == null)
            {
                MessageBox.Show("请输入工作内容信息");
                return;
            }
            XMLHelper.InsertJobIntoXmlDocument(textBoxJob.Text.Trim());
            BindJobsData();
            textBoxJob.Text = "";
        }

        /// <summary>
        /// dataGridPathsViewMenu鼠标点击事件
        /// </summary>
        /// <param name="sender">dataGridPathsViewMenu</param>
        /// <param name="e"></param>
        private void dataGridPathsViewMenu_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
                        
        }

        /// <summary>
        /// 点击删除应用程序路径事件
        /// </summary>
        /// <param name="sender">dataGridPathsViewMenuItem</param>
        /// <param name="e"></param>
        private void dataGridPathsViewMenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            string str = "";
            int count = dataGridPathsView.SelectedItems.Count;
            int[] pathsIndexes = new int[count];

            for (int i = 0; i < count; i++)
            {
                str += dataGridPathsView.SelectedIndex;
                pathsIndexes[i] = dataGridPathsView.SelectedIndex;
                DataGridRow row = (DataGridRow)dataGridPathsView.ItemContainerGenerator.ContainerFromIndex(dataGridPathsView.SelectedIndex);
                row.IsSelected = false;
            }

            MessageBox.Show(str);
            Array.Sort(pathsIndexes);
            XMLHelper.DeletePathsInXmlDocument(pathsIndexes);
            BindPathsData();
        }

        /// <summary>
        /// 点击删除工作信息事件
        /// </summary>
        /// <param name="sender">dataGridJobsViewMenuItem</param>
        /// <param name="e"></param>
        private void dataGridJobsViewMenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            string str = "";
            int count = dataGridJobsView.SelectedItems.Count;
            int[] jobsIndexes = new int[count];

            for (int i = 0; i < count; i++)
            {
                str += dataGridJobsView.SelectedIndex;
                jobsIndexes[i] = dataGridJobsView.SelectedIndex;
                DataGridRow row = (DataGridRow)dataGridJobsView.ItemContainerGenerator.ContainerFromIndex(dataGridJobsView.SelectedIndex);
                row.IsSelected = false;
            }

            MessageBox.Show(str);
            Array.Sort(jobsIndexes);
            XMLHelper.DeleteJobsInXmlDocument(jobsIndexes);
            BindJobsData();
        }

        /// <summary>
        /// 工作状态改变触发事件（此处效率不行，在刚选中一个ComboBox时也触发此事件，此时还没有改变Selection）
        /// </summary>
        /// <param name="sender">comboBoxJobState</param>
        /// <param name="e">事件</param>
        private void comboBoxJobState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBox cb = (ComboBox)sender;
            string state ;
            try
            {
                object obj = (object)e.AddedItems;// 获取下拉列表中所有选定项的列表（集合）
                //str = Convert.ToString(((System.Data.DataRowView)(((object[])(obj))[0])).Row.ItemArray[1]);
                state = (string)(((object[])(obj))[0]);// 把当前选中的项转换成原来的类型 此处原为string
                //str = obj.ToString();
                if (state != null && state.Length > 0)
                {
                    XMLHelper.UpdateJobStateInXmlDocument(dataGridJobsView.SelectedIndex, state);
                }
            }
            catch(Exception)
            {
                
            }
        }

        #endregion 事件

        #region 方法
        /// <summary>
        /// 启动常用程序
        /// </summary>
        private void StartPrograms()
        {
            List<string> pathsList = XMLHelper.GetPaths();
            int i = 0;

            try
            {
                for (; i < pathsList.Count; i++)
                {
                    Process.Start(pathsList[i]);
                }
            }
            catch
            {
                MessageBox.Show("在错误路径下没有找到要打开的程序:" + pathsList[i]);
            }
        }
        
        /// <summary>
        /// 为dataGridPathsView绑定应用程序路径数据
        /// </summary>
        private void BindPathsData()
        {
            DataSet ds = XMLHelper.GetDataSetOfXmlDocument();
            try
            {
                dataGridPathsView.ItemsSource = ds.Tables["Path"].DefaultView;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 为dataGridJobsView绑定应用程序路径数据
        /// </summary>
        private void BindJobsData()
        {
            DataSet ds = XMLHelper.GetDataSetOfXmlDocument();

            try
            {
                DataTableJobs = ds.Tables["Job"];
                dataGridJobsView.ItemsSource = DataTableJobs.DefaultView;
            }
            catch
            {
            }
        }
        
        /// <summary>
        /// 验证richTextBoxProgramPath中的数据是否为应用程序的正确路径
        /// </summary>
        /// <param name="path">应用程序路径</param>
        /// <returns>true：该路径正确；false：该路径不正确</returns>
        private bool ValidatePath(string path)
        {
            if (path.Trim().Length == 0 || path.Trim() == "" || path == null || !System.IO.File.Exists(path.Trim()))
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// 获取控件中的同名控件集合
        /// </summary>
        /// <typeparam name="T">要获取的控件类型</typeparam>
        /// <param name="obj">目标控件</param>
        /// <param name="name">要获取的控件名字</param>
        /// <returns>控件集合</returns>
        public List<T> GetChildObjects<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name && string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child, ""));
            }
            return childList;
        }

        #endregion 方法
    }
}
