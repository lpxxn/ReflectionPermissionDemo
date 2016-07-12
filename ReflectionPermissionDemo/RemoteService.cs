using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionPermissionDemo
{
    /// <summary>
    /// 模拟远程服务器
    /// 返回拥有的权限
    /// </summary>
    public class RemoteService
    {
        public static readonly RemoteService PermissionService = new RemoteService();
        public DataTable PermissionTable { get; private set; }

        #region   字段名称
        public readonly string ModuleId = @"ModuleID";
        public readonly string ModuleName = @"ModuleName";
        public readonly string PermissioniNameSpace = @"PermissioniNameSpace";
        public readonly string ParentId = @"ParentID";
        #endregion

        private RemoteService()
        {
            AllPermision();
        }

        /// <summary>
        /// 所有的权限
        /// </summary>
        /// <returns></returns>
        private DataTable AllPermision()
        {
            PermissionTable = new DataTable();
            #region Permission Page
            PermissionTable.Columns.AddRange(new[]
            {                
                new DataColumn(ModuleId, typeof (Int32)),               // 模块id
                new DataColumn(ModuleName, typeof (string)),            // 模块名称                                 
                new DataColumn(PermissioniNameSpace, typeof (string)),  // 命名空间
                new DataColumn(ParentId, typeof (Int32))                // 父id          
            });
            #endregion

            #region A

            CreateNewRow(1001, @"A模块", @"A", -1);
            CreateNewRow(1002, @"A 页面1", @"A.AForm1", 1001);
            // 测试权限先注掉
            //CreateNewRow(1003, @"A 页面2", @"A.AForm2", 1001);
            #endregion


            #region B 由于我们的例子只不需要B的权限，这里先注掉
            //CreateNewRow(2001, @"B模块", @"B", -1);
            //CreateNewRow(2002, @"B 页面1", @"B.BForm1", 2001);

            #endregion

            #region C
            CreateNewRow(3001, @"C模块", @"C", -1);
            CreateNewRow(3002, @"C Page1", @"C.CForm1", 3001);
            CreateNewRow(3003, @"C Page2", @"C.CForm2", 3001);
            #endregion

            return null;
        }

        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="moduleName"></param>
        /// <param name="perNameSpace"></param>
        /// <param name="parentId"></param>
        private void CreateNewRow(int moduleId, string moduleName, string perNameSpace, int parentId)
        {
            var newRow = PermissionTable.NewRow();            
            newRow[ModuleId] = moduleId;
            newRow[ModuleName] = moduleName;
            newRow[PermissioniNameSpace] = perNameSpace;
            newRow[ParentId] = parentId;
            PermissionTable.Rows.Add(newRow);
        }

       
    }
}
