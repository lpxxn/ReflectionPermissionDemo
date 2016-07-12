using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReflectionPermissionDemo
{
    
    public partial class Main : Form
    {
        // 权限数据
        readonly DataTable _permissionDt = RemoteService.PermissionService.PermissionTable;
        // 可用权限动态生成的panel页面
        readonly Dictionary<int, FlowLayoutPanel> _pagePanels = new Dictionary<int, FlowLayoutPanel>();
        // 已加载过的页面
        private readonly Dictionary<string, Type> _formTypes = new Dictionary<string, Type>();
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {            
            var query = _permissionDt.Rows.Cast<DataRow>();
            var parentData = query.Where(x => int.Parse(x[RemoteService.PermissionService.ParentId].ToString()) == -1);
            SettingDllButtons(parentData);

        }

        /// <summary>
        /// 展示 dll的权限按钮
        /// </summary>
        /// <param name="dt"></param>
        private void SettingDllButtons(IEnumerable<DataRow> drs)
        {
            int width = 80, height = 30, x = 0, y = 0;
            foreach (var dataRow in drs)
            {
                var btn = new Button
                {
                    Text = dataRow[RemoteService.PermissionService.ModuleName].ToString(),
                    Size = new Size(width, height),
                    Location = new Point(x, y)        
                };
                var index = SettingPageButtons(dataRow);
                btn.Tag = index;
                btn.Click += btnDLL_Click;
                panelTop.Controls.Add(btn);
                x += width + 10;
            }            
        }
        /// <summary>
        /// 根据 datarow的父id去找到所有的子节点
        /// 加载到相应的页面上组织成按钮
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private int SettingPageButtons(DataRow dr)
        {
                        
            var index = _pagePanels.Count();

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Visible = false
            };
            panelBody.Controls.Add(panel); 
            _pagePanels[index] = panel;

            #region Btns
            var query = _permissionDt.Rows.Cast<DataRow>();
            var data =
                query.Where(
                    x =>
                        int.Parse(x[RemoteService.PermissionService.ParentId].ToString()) ==
                        int.Parse(dr[RemoteService.PermissionService.ModuleId].ToString()));
            if (!data.Any())
                return index;

            int width = 80, height = 30;
            foreach (var dataRow in data)
            {
                var btn = new Button
                {
                    Text = dataRow[RemoteService.PermissionService.ModuleName].ToString(),
                    Size = new Size(width, height),
                    Tag = dataRow[RemoteService.PermissionService.PermissioniNameSpace]
                };
                btn.Click += btnPage_Click;             
                panel.Controls.Add(btn);                
            }
            #endregion
            return index;
        }       

        /// <summary>
        /// 显示相应的页面元素
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDLL_Click(object sender, EventArgs e)
        {
            var index = int.Parse(((Button) sender).Tag.ToString());
            foreach (KeyValuePair<int, FlowLayoutPanel> flowLayoutPanel in _pagePanels)
            {
                flowLayoutPanel.Value.Visible = flowLayoutPanel.Key == index;                
            }
        }

        /// <summary>
        /// 打开相应的page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPage_Click(object sender, EventArgs e)
        {
            var name = ((Button) sender).Tag.ToString();
            var form = GetModule(name) as Form;
            form.ShowDialog(this);
        }

        #region
        /// <summary>
        /// 利用反射去加载相应的页面
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mainNamespace"></param>
        /// <returns></returns>
        private object GetModule(string path, string mainNamespace = "ReflectionPermissionDemo")
        {
            var curNamespace = "";

            var index = path.IndexOf('.');
            if (index > -1)
            {
                curNamespace = "." + path.Substring(0, index);
            }
            else
            {
                curNamespace = "";
            }

            var assemblyPath = mainNamespace + curNamespace;
            var classPath = mainNamespace + "." + path;
            object module = null;
            if (_formTypes.ContainsKey(classPath))
            {
                module = Activator.CreateInstance(_formTypes[classPath]);
            }
            else
            {
                try
                {
                    module = Assembly.Load(assemblyPath).CreateInstance(classPath);
                    if (module != null)
                        _formTypes.Add(classPath, module.GetType());
                }
                catch
                {
                    // 查找当前已加载的dll。
                    Type type = null;
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (!assembly.FullName.Contains(mainNamespace))
                            continue;

                        type = assembly.GetType(classPath, false);
                        if (type != null)
                        {
                            break;
                        }
                    }
                    if (type == null)
                    {
                        throw;
                    }
                    else
                    {
                        module = Activator.CreateInstance(type);
                        if (module != null)
                        {
                            _formTypes.Add(classPath, type);
                        }
                        ;
                    }                  
                }
            }
            return module;
        }
        #endregion
    }
}
