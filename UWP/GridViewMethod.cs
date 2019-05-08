using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Router.Model;

namespace Router.Controller
{
    class GridViewMethod
    {
        /// <summary>
        /// 批量布置UWP列表控件
        /// </summary>
        /// <param name="gridView">需要布置的控件</param>
        /// <param name="Obj">对象列表（泛型集合）</param>
        public static void LayOutItem(GridView gridView,List<ViewDevModel> Obj)
        {
            foreach (var item in Obj)
            {
                gridView.Items.Add(new ViewDevModel { NetPort = item.NetPort, Type = item.Type, IPAddress = item.IPAddress,ImageUrl= item.ImageUrl });
                //请自行替换模型
            }
        }
    }
}
