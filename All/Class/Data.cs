using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Xml;
using System.Data.SqlServerCe;
namespace All.Class
{
    /// <summary>
    /// 数据库读写
    /// </summary>
    [Serializable]
    public abstract class DataReadAndWrite
    {
        public abstract System.Data.Common.DbConnection Conn
        { get; }
        /// <summary>
        /// 将表批量更新到数据库
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public abstract int BlockCommand(DataTable dt);
        /// <summary>
        /// 登陆数据数据库
        /// </summary>
        /// <returns></returns>
        public abstract bool Login(string Address, string Data, string UserName, string Password);
        /// <summary>
        /// 将数据写入到数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public abstract int Write(string sql);
        /// <summary>
        /// 将符合条件的指定数据更新到指定列
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="value"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public abstract int Update(string tableName, string[] columns, object[] value, string[] conditions);
        /// <summary>
        /// 将指定数据更新到指定列
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Update(string tableName, string[] columns, object[] value)
        {
            return Update(tableName, columns, value, null);
        }
        /// <summary>
        /// 指定列插入指定数据
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract int Insert(string tableName,string[] columns, object[] value);
        /// <summary>
        /// 所有列插入指定数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Insert(string tableName,object[] value)
        {
            return Insert(null, value);
        }
        /// <summary>
        /// 从数据库读取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public abstract DataTable Read(string sql);
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            if (Conn != null)
            {
                Conn.Close();
                Conn.Dispose();
            }
        }
        /// <summary>
        /// 登陆 
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            if (Conn == null)
            {
                return false;
            }
            bool result = false;
            try
            {
                Conn.Open();
                result = (Conn.State == ConnectionState.Open);
            }
            catch (Exception e)
            {
                Error.Add(new string[] { "连接字符" }, new string[] { Conn.ConnectionString });
                Error.Add(e);
            }
            return result;
        }
        /// <summary>
        /// 检查连接状态
        /// </summary>
        protected void CheckConn()
        {
            if (Conn != null)
            {
                if (Conn.State != ConnectionState.Open)
                {
                    try
                    {
                        Conn.Open();
                    }
                    catch (Exception e)
                    {
                        All.Class.Error.Add(e);
                    }
                }
            }
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="columns">要读取的列名</param>
        /// <param name="tableName">表名</param>
        /// <param name="conditions">条件</param>
        /// <param name="orders">排序</param>
        /// <param name="Desc">是否须要逆排序</param>
        /// <returns></returns>
        public abstract DataTable Read(string[] columns, string tableName, string[] conditions, string[] orders,bool Desc);
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="columns">要读取的列名</param>
        /// <param name="tableName">表名</param>
        /// <param name="conditions">条件</param>
        /// <param name="orders">排序</param>
        /// <param name="Desc">是否须要逆排序</param>
        /// <returns></returns>
        public DataTable Read(string[] columns, string tableName, string[] conditions, string[] orders)
        {
            return Read(columns, tableName, conditions, orders, false);
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="columns">要读取的列名</param>
        /// <param name="tableName">表名</param>
        /// <param name="conditions">条件</param>
        public DataTable Read(string[] columns, string tableName, string[] conditions)
        {
            return Read(columns, tableName, conditions, null);
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="columns">要读取的列名</param>
        /// <param name="tableName">表名</param>
        public DataTable Read(string[] columns, string tableName)
        {
            return Read(columns, tableName, null);
        }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Text
        { get; set; }
        /// <summary>
        /// 从文本文件中反射数据库连接 
        /// </summary>
        /// <param name="fileName">文本文件位置</param>
        /// <param name="text">数据描述文本</param>
        /// <returns></returns>
        public static DataReadAndWrite GetData(string fileName, string text)
        {
            All.Class.DataReadAndWrite result = null;
            XmlNode tmpNode = All.Class.XmlHelp.GetXmlNode(fileName);
            foreach (XmlNode tmpConn in tmpNode.ChildNodes)
            {
                if (tmpConn.NodeType != XmlNodeType.Element)
                {
                    continue;
                }
                Dictionary<string, string> connAttribute = All.Class.XmlHelp.GetAttribute(tmpConn);
                if (connAttribute.ContainsKey("Name") && connAttribute["Name"] == text)
                {
                    try
                    {
                        Dictionary<string, string> connStr = All.Class.XmlHelp.GetInner(tmpConn);

                        All.Class.Reflex<All.Class.DataReadAndWrite> r = new All.Class.Reflex<All.Class.DataReadAndWrite>("All", connAttribute["Class"]);
                        result = (All.Class.DataReadAndWrite)r.Get();
                        if (result != null && connAttribute.ContainsKey("Text"))
                        {
                            result.Text = connAttribute["Text"];
                        }
                        if (!result.Login(connStr["Address"], connStr["DataBase"], connStr["UserName"], connStr["Password"]))
                        {
                            All.Class.Error.Add(string.Format("{0}:数据库登陆失败,请检查数据库连接", result.Text), Environment.StackTrace);
                        }
                    }
                    catch (Exception e)
                    {
                        All.Class.Error.Add(e);
                    }
                    break;
                }
            }
            return result;
        }
    }
    #region//Excel数据库
    /// <summary>
    /// Excel数据库
    /// </summary>
    public class Excel : DataReadAndWrite
    {
        object lockObject = new object();
        public override System.Data.Common.DbConnection Conn
        {
            get { return conn; }
        }
        OleDbConnection conn;
        /// <summary>
        /// 登陆数据数据库
        /// </summary>
        public override bool Login(string Address, string Data, string UserName, string Password)
        {
            return Login(Address, Data, Password, false);
        }
        public override int BlockCommand(DataTable dt)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 登陆EXCEL数据库
        /// </summary>
        /// <param name="Address">EXCEL文件夹路径</param>
        /// <param name="Data">EXCEL文件名</param>
        /// <param name="Password">密码</param>
        /// <param name="Write">只读，或读写</param>
        /// <returns></returns>
        public bool Login(string Address, string Data, string Password, bool Write)
        {
            bool result = false;
            try
            {
                //连接字符串
                //Provider:程序版本,Microsoft.ACE.OLEDB.12.0代表office2007版本,Microsoft.Jet.OLEDB.4.0代表office2003版本,貌似2007是兼容2003的

                //Data Source:数据源,指定Excel文件路径.
                //HDR:第一行是否有数据,为YES时,表示第一行为表头,没有数据.为NO时,表示第一行有数据.系统默认值为Yes
                //Imex:操作模式,为0时代表导出模式,为1时代表导入模式,为2时代表完全模式,
                //简单点,为0时,驱动根据数据表前8行来推算数据类型,
                //       为1时,所有单元格都按文本数据类型来计算,其实也不是...Bug么?
                //       为2时,完全写入模式
                //Extended Properties:驱动版本Excel 8.0代表office2003驱动,Excel 12.0代表office2007驱动

                //报ISAM错误,解决办法,用引号将Extended Properties的内容引用起来
                //唯一的解决办法,将所有列设为文本数据类型,方法一,将注册表的前8行改为所有行,方法二,第一行设为标题文本行.HDR=No来判断
                conn = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\\{1};Extended Properties=\"Excel 8.0;HDR=No;IMEX={2};\";Jet OLEDB:Database Password={3}",
                    Address, Data, Write ? 2 : 1, Password));
                conn.Open();
                result = (conn.State == ConnectionState.Open);
            }
            catch (Exception e)
            {
                Error.Add(e);
            }
            return result;
        }
        /// <summary>
        /// 将数据导出到excel,其实只是按Excel格式保存的文本文件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool OutToExcel(DataTable data, string fileName)
        {
            bool result = true;
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    sb.Append(string.Format("{0}\t", data.Columns[i].Caption));
                }
                sb.Append("\r\n");
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        sb.Append(string.Format("{0}\t", data.Rows[i][j]));
                    }
                    sb.Append("\r\n");
                }
                FileIO.Write(fileName, sb.ToString());
            }
            catch (Exception e)
            {
                result = false;
                Error.Add(e);
            }
            return result;
        }
        /// <summary>
        /// 按表格的起始单元格来读取数据
        /// </summary>
        /// <param name="tableName">工作本名称</param>
        /// <param name="StartCell"></param>
        /// <param name="EndCell"></param>
        /// <returns></returns>
        public DataTable Read(string tableName, string StartCell, string EndCell)
        {
            string sql = string.Format("select * from [{0}${1}:{2}]", tableName.Replace("$", ""), StartCell, EndCell);
            return Read(sql);
        }
        public override DataTable Read(string[] columns, string tableName, string[] conditions, string[] orders, bool Desc)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 从数据库读取数据 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override DataTable Read(string sql)
        {
            lock (lockObject)
            {
                DataTable dt = new DataTable();
                try
                {
                    CheckConn();
                    using (OleDbDataAdapter od = new OleDbDataAdapter(new OleDbCommand(sql, conn)))
                    {
                        od.Fill(dt);
                    }
                }
                catch (Exception e)
                {
                    Error.Add(e);
                    Error.Add(sql);
                }
                return dt.Copy();
            }
        }
        /// <summary>
        /// 将数据写入到数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override int Write(string sql)
        {
            lock (lockObject)
            {
                int result = 0;
                try
                {
                    CheckConn();
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Error.Add(e);
                    Error.Add(sql);
                }
                return result;
            }
        }
        public override int Insert(string tableName, string[] columns, object[] value)
        {
            throw new NotImplementedException();
        }
        public override int Update(string tableName, string[] columns, object[] value, string[] conditions)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
    #region//SQLCE数据库
    /// <summary>
    /// SQLCE数据库
    /// </summary>
    public class Sqlce : DataReadAndWrite
    {
        object lockObject = new object();
        public override System.Data.Common.DbConnection Conn
        {
            get { return conn; }
        }
        SqlCeConnection conn;
        public override int BlockCommand(DataTable dt)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 登陆数据数据库
        /// </summary>
        public override bool Login(string Address, string DataBase, string UserName, string Password)
        {
            bool result = false;
            try
            {
                conn = new SqlCeConnection(string.Format("Data Source={0}\\{1};Max Database Size=4091;Persist Security Info=False;", Address, DataBase));
                conn.Open();
                result = (conn.State == ConnectionState.Open);
            }
            catch (Exception e)
            {
                Error.Add(e);
            }
            return result;
        }
        /// <summary>
        /// 从数据库读取数据 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override DataTable Read(string sql)
        {
            lock (lockObject)
            {
                DataTable dt = new DataTable();
                try
                {
                    CheckConn();
                    using (SqlCeDataAdapter od = new SqlCeDataAdapter(new SqlCeCommand(sql, conn)))
                    {
                        od.Fill(dt);
                    }
                }
                catch (Exception e)
                {
                    Error.Add(e);
                    Error.Add(sql);
                }
                return dt.Copy();
            }
        }
        public override DataTable Read(string[] columns, string tableName, string[] conditions, string[] orders, bool Desc)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 将数据写入到数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override int Write(string sql)
        {
            lock (lockObject)
            {
                int result = 0;
                try
                {
                    CheckConn();
                    using (SqlCeCommand cmd = new SqlCeCommand(sql, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Error.Add(e);
                    Error.Add(sql);
                }
                return result;
            }
        }
        public override int Insert(string tableName, string[] columns, object[] value)
        {
            throw new NotImplementedException();
        }
        public override int Update(string tableName, string[] columns, object[] value, string[] conditions)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
    #region//Access数据库
    /// <summary>
    /// Access数据库
    /// </summary>
    public class Access : DataReadAndWrite
    {
        object lockObject = new object();
        public override System.Data.Common.DbConnection Conn
        {
            get { return conn; }
        }
        OleDbConnection conn;
        public override int BlockCommand(DataTable dt)
        {
            lock (lockObject)
            {
                int result = 0;
                CheckConn();

                if (dt.TableName == "")
                {
                    All.Class.Error.Add("无法完成批量数据更新,数据表名不能为空");
                    All.Class.Error.Add(Environment.StackTrace);
                    return 0;
                }
                try
                {
                    using (OleDbCommand cmd = new OleDbCommand(string.Format("select * from {0}", dt.TableName), conn))
                    {
                        using (OleDbDataAdapter ada = new OleDbDataAdapter(cmd))
                        {
                            using (OleDbCommandBuilder scb = new OleDbCommandBuilder(ada))
                            {
                                ada.InsertCommand = scb.GetInsertCommand();
                                ada.DeleteCommand = scb.GetDeleteCommand();
                                ada.UpdateCommand = scb.GetUpdateCommand();
                                result = ada.Update(dt);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    All.Class.Error.Add(string.Format("出错Table为：{0}", dt.TableName));
                    All.Class.Error.Add(e);//数据库中一定要有主键，不然当前方法会出错。即没有办法生成删除命令
                }
                return result;
            }
        }
        /// <summary>
        /// 登陆数据数据库
        /// </summary>
        public override bool Login(string Address, string DataBase, string UserName, string Password)
        {
            bool result = false;
            try
            {
                conn = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\\{1};Jet OLEDB:Database Password={2}",
                        Address, DataBase, Password));
                conn.Open();
                result = (conn.State == ConnectionState.Open);
            }
            catch (Exception e)
            {
                Error.Add(e);
            }
            return result;
        }
        /// <summary>
        /// 从数据库读取数据 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override DataTable Read(string sql)
        {
            lock (lockObject)
            {
                DataTable dt = new DataTable();
                try
                {
                    CheckConn();
                    using (OleDbDataAdapter od = new OleDbDataAdapter(new OleDbCommand(sql, conn)))
                    {
                        od.Fill(dt);
                    }
                }
                catch (Exception e)
                {
                    Error.Add(e);
                    Error.Add(sql);
                }
                return dt.Copy();
            }
        }
        public override DataTable Read(string[] columns, string tableName, string[] conditions, string[] orders, bool Desc)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 将数据写入到数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override int Write(string sql)
        {
            lock (lockObject)
            {
                int result = 0;
                try
                {
                    CheckConn();
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Error.Add(e);
                    Error.Add(sql);
                }
                return result;
            }
        }
        public override int Insert(string tableName, string[] columns, object[] value)
        {
            throw new NotImplementedException();
        }
        public override int Update(string tableName, string[] columns, object[] value, string[] conditions)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
    #region//SQL数据库
    /// <summary>
    /// SQL数据库
    /// </summary>
    public class SQLServer : DataReadAndWrite
    {
        object lockObject = new object();
        public override System.Data.Common.DbConnection Conn
        {
            get { return conn; }
        }
        SqlConnection conn;
        public override int  BlockCommand(DataTable dt)
        {
            int result = 0 ;
            lock (lockObject)
            {
                CheckConn();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(string.Format("select * from {0}", dt.TableName), conn))
                    {
                        using (SqlDataAdapter ada = new SqlDataAdapter(cmd))
                        {
                            using (SqlCommandBuilder scb = new SqlCommandBuilder(ada))
                            {
                                ada.InsertCommand = scb.GetInsertCommand();
                                ada.DeleteCommand = scb.GetDeleteCommand();
                                ada.UpdateCommand = scb.GetUpdateCommand();
                                result = ada.Update(dt);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    All.Class.Error.Add(string.Format("出错Table为：{0}", dt.TableName));
                    All.Class.Error.Add(e);//数据库中一定要有主键，不然当前方法会出错。即没有办法生成删除命令
                }
            }
            return result;
        }
        /// <summary>
        /// 登陆数据数据库
        /// </summary>
        public override bool Login(string Ipaddress, string DataBase, string UserName, string Password)
        {
            bool result = false;
            try
            {
                conn = new SqlConnection(string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Pooling=true;Max Pool Size=10000;Min Pool Size=0",
                        Ipaddress, DataBase, UserName, Password));
                if (Ipaddress == "127.0.0.1")
                {
                    conn = new SqlConnection(string.Format("Data Source=127.0.0.1;Initial Catalog={0};Integrated Security=True;Pooling=true;Max Pool Size=1000;Min Pool Size=0", DataBase));
                }
                conn.Open();
                result = (conn.State == ConnectionState.Open);
            }
            catch (Exception e)
            {
                Error.Add(new string[] { "连接字符" }, new string[] { conn.ConnectionString });
                Error.Add(e);
            }
            return result;
        }

        /// <summary>
        /// 从数据库读取数据 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override DataTable Read(string sql)
        {
            lock (lockObject)
            {
                DataTable dt = new DataTable();
                try
                {
                    CheckConn();
                    using (SqlDataAdapter od = new SqlDataAdapter(new SqlCommand(sql, conn)))
                    {
                        od.Fill(dt);
                    }
                }
                catch (Exception e)
                {
                    Error.Add(e);
                    Error.Add(sql);
                }

                return dt.Copy();
            }
        }
        public override DataTable Read(string[] columns, string tableName, string[] conditions, string[] orders, bool Desc)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 将数据写入到数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override int Write(string sql)
        {
            lock (lockObject)
            {
                int result = 0;
                try
                {
                    CheckConn();
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Error.Add(e);
                    Error.Add(sql);
                    result = -1;
                }
                return result;
            }
        }
        public override int Insert(string tableName, string[] columns, object[] value)
        {
            throw new NotImplementedException();
        }
        public override int Update(string tableName, string[] columns, object[] value, string[] conditions)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool SaveDataBase(string DataName,string fileName)
        {
            bool result = true;

            try
            {
                SqlConnection sqlConn = new SqlConnection();
                sqlConn.ConnectionString = string.Format("Data Source=127.0.0.1;Initial Catalog={0};Integrated Security=True",
                     DataName);
                sqlConn.Open();
                string sqlSave = string.Format("BackUp database {0} to Disk='{1}'", DataName,fileName);
                SqlCommand cmd = new SqlCommand(sqlSave, sqlConn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                sqlConn.Close();
                sqlConn.Dispose();
            }
            catch(Exception e)
            {
                Error.Add(e);
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 测试本地数据库连接
        /// </summary>
        /// <param name="DataName"></param>
        /// <returns></returns>
        public static bool TestLocal(string DataName)
        {
            bool result = true;
            using (SqlConnection sqlConn = new SqlConnection())
            {
                try
                {
                    sqlConn.ConnectionString = string.Format("Data Source=127.0.0.1;Integrated Security=True");
                    sqlConn.Open();

                }
                catch (Exception e)
                {
                    Error.Add(e);
                }
                finally
                {
                    result = (sqlConn.State == ConnectionState.Open);
                }
            }
            return result;
        }
        /// <summary>
        /// 删除本地数据库
        /// </summary>
        /// <param name="DataName"></param>
        /// <returns></returns>
        public static bool DropDataBase(string DataName)
        {
            bool result = true;

            try
            {
                SqlConnection sqlConn = new SqlConnection();
                sqlConn.ConnectionString = string.Format("Data Source=127.0.0.1;Integrated Security=True");
                sqlConn.Open();


                string strSQL = string.Format("select spid from master..sysprocesses where dbid=db_id('{0}')", DataName);

                SqlDataAdapter Da = new SqlDataAdapter(strSQL, sqlConn);
                DataTable spidTable = new DataTable();
                Da.Fill(spidTable);

                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandType = CommandType.Text;
                Cmd.Connection = sqlConn;

                if (spidTable.Rows.Count > 1)
                {
                    //强行关闭非本程序使用的所有用户进程 
                    for (int iRow = 0; iRow < spidTable.Rows.Count - 1; iRow++)
                    {
                        Cmd.CommandText = "kill " + spidTable.Rows[iRow][0].ToString();   //强行关闭用户进程 
                        int n = Cmd.ExecuteNonQuery();
                    }
                }
                Cmd.Dispose();


                string str = string.Format("Drop DataBase {0}", DataName);

                Cmd = new SqlCommand(str, sqlConn);
                Cmd.ExecuteNonQuery();

                Cmd.Dispose();
                sqlConn.Close();
                sqlConn.Dispose();

            }
            catch (Exception e)
            {
                Error.Add(e);
                result = false;
            }


            return result;
        }
        /// <summary>
        /// 还原数据库
        /// </summary>
        /// <param name="IpAddress"></param>
        /// <param name="DataName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool BackDataBase(string DataName,  string fileName)
        {
            bool result = true;

            try
            {
                SqlConnection sqlConn = new SqlConnection();
                sqlConn.ConnectionString = string.Format("Data Source=127.0.0.1;Integrated Security=True");
                sqlConn.Open();


                string strSQL = string.Format("select spid from master..sysprocesses where dbid=db_id('{0}')", DataName);

                SqlDataAdapter Da = new SqlDataAdapter(strSQL, sqlConn);
                DataTable spidTable = new DataTable();
                Da.Fill(spidTable);

                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandType = CommandType.Text;
                Cmd.Connection = sqlConn;

                if (spidTable.Rows.Count > 1)
                {
                    //强行关闭非本程序使用的所有用户进程 
                    for (int iRow = 0; iRow < spidTable.Rows.Count - 1; iRow++)
                    {
                        Cmd.CommandText = "kill " + spidTable.Rows[iRow][0].ToString();   //强行关闭用户进程 
                        int n = Cmd.ExecuteNonQuery();
                    }
                }
                Cmd.Dispose();


                string str = string.Format("use master;restore database {0} From disk='{1}' With Replace;", DataName, fileName);

                Cmd = new SqlCommand(str, sqlConn);
                Cmd.ExecuteNonQuery();
                
                Cmd.Dispose();
                sqlConn.Close();
                sqlConn.Dispose();

            }
            catch(Exception e)
            {
                Error.Add(e);
                result = false;  
            }
            return result;
        }
    }
    #endregion
}
