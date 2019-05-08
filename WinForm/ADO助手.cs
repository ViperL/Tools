using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using System.Configuration;

namespace DAL
{/// <summary>
/// 通用数据访问类
/// </summary>
    public class SQLHelper
    {
        // private static string connString = @"Server=VIPERLI\SQLEXPRESS;DataBase = StudentManageDB;Uid=sa;Pwd=19961231ABClkr";//连接字符串
        //public static readonly string connString = ConfigurationManager.ConnectionStrings["connString"].ToString();//只读方式!未加密

        public static readonly string connString = Common.StringSecurity.DESDecrypt(ConfigurationManager.ConnectionStrings["connString"].ToString());//包含解密方法

        /// <summary>
        /// 执行增删改方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int UpData(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                Exception ex = new Exception("访问错误，请检查权限");
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 执行单一查询方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static object GetSingleResult(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception)
            {
                Exception ex = new Exception("访问错误，请检查权限");
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 执行多元数据查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static SqlDataReader GetReader(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);//CommandBehavior.CloseConnection自动关闭
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }
    }
}
