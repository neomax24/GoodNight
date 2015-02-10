using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace GoodNight_Test_0
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GoodNightPage : Page
    {
        public class data_checklist_time_peroid_struct
        {
            public string name { get; set; }
            public int time_peroid { get; set; }

            public string time_peroid_string { get; set; }
            public bool is_work { get; set; }

        }
        List<data_checklist_time_peroid_struct> data_checklist_time_peroid=new List<data_checklist_time_peroid_struct>();

        public GoodNightPage()
        {
            this.InitializeComponent();

        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ///Bling测试
            
            for (int i = 0; i < 10; i++)
            {
                data_checklist_time_peroid_struct data_checklist_time_peroid_struct_item = new data_checklist_time_peroid_struct();
                data_checklist_time_peroid_struct_item.name = "test" + i.ToString();
                data_checklist_time_peroid_struct_item.time_peroid = i;
                data_checklist_time_peroid_struct_item.time_peroid_string = i.ToString();
                data_checklist_time_peroid_struct_item.is_work = true;
                data_checklist_time_peroid.Add(data_checklist_time_peroid_struct_item);
            }
            checklist_time_peroid.ItemsSource = data_checklist_time_peroid;
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ListPickerFlyout_ItemsPicked(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {

        }


    }
}
