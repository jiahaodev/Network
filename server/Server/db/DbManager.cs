/****************************************************
	文件：DbManager.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2019/11/15 23:02   	
	功能：数据库管理者
*****************************************************/
using MySql.Data.MySqlClient;
using System.Web.Script.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Server
{
    class DbManager
    {
        public static MySqlConnection mysql;
        static JavaScriptSerializer Js = new JavaScriptSerializer();

        //连接mysql数据库
        public static bool Connect(string db, string ip, int port, string user, string pw)
        {
            //创建MySqlConnection对象
            mysql = new MySqlConnection();
            //连接参数
            string sqlStr = string.Format("Database={0};Data Source={1}; port={2};User Id={3}; Password={4}",
                db, ip, port, user, pw);
            mysql.ConnectionString = sqlStr;
            //连接
            try
            {
                mysql.Open();
                Console.WriteLine("[数据库]connect succ");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库]connect fail," + e.Message);
                return false;
            }
        }


        //测试并重连
        private static void CheckAndReconnect()
        {
            try
            {
                if (mysql.Ping())
                {
                    return;
                }
                mysql.Close();
                mysql.Open();
                Console.WriteLine("[数据库] Reconnect succ!");
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库] Reconnect fail," + e.Message);
            }

        }

        //判断安全字符串(防止sql注入)
        private static bool IsSafeString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }


        //是否(不)存在该用户
        public static bool IsAccountNoExist(string id)
        {
            CheckAndReconnect();
            if (!DbManager.IsSafeString(id))
            {
                return false;
            }
            //sql语句
            string sqlStr = string.Format("select * from account where id = '{0};'", id);
            //查询
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr, mysql);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                dataReader.Close();
                return !hasRows; // ?? todo
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库] IsSafeString err, " + e.Message);
                return false;
            }
        }


        //注册
        public static bool Register(string id, string pw)
        {
            CheckAndReconnect();
            //防止sql注入
            if (!DbManager.IsSafeString(id))
            {
                Console.WriteLine("[数据库] Register fail, id not safe");
                return false;
            }
            if (!DbManager.IsSafeString(pw))
            {
                Console.WriteLine("[数据库] Register fail, pw not safe");
                return false;
            }

            //玩家是否存在
            if (!IsAccountNoExist(id))
            {
                Console.WriteLine("[数据库] Register fail, id exist");
            }
            //写入数据库User表
            string sqlStr = string.Format("insert into account set id='{0}',pw='{1}';", id, pw);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr, mysql);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库] Register fail " + e.Message);
                return false;
            }
        }

        //检测用户名密码
        public static bool CheckPassword(string id, string pw)
        {
            CheckAndReconnect();
            //防止sql注入
            if (!DbManager.IsSafeString(id))
            {
                Console.WriteLine("[数据库] CheckPassword fail, id not safe");
                return false;
            }
            if (!DbManager.IsSafeString(pw))
            {
                Console.WriteLine("[数据库] CheckPassword fail, pw not safe");
                return false;
            }
            //查询
            string sqlStr = string.Format("select * from account where id='{0}' and pw='{1};'", id, pw);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr, mysql);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                dataReader.Close();
                return hasRows;
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库] CheckPassword err, " + e.Message);
                return false;
            }
        }


        //创建角色
        public static bool CreatePlayer(string id)
        {
            CheckAndReconnect();
            //防止sql注入
            if (!DbManager.IsSafeString(id))
            {
                Console.WriteLine("[数据库] CreatePlayer fail, id not safe");
                return false;
            }
            //序列化
            PlayerData playerData = new PlayerData();
            string data = Js.Serialize(playerData);
            //写入数据库
            string sqlStr = string.Format("insert into player set id ='{0}, data ='{1}';", id, data);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr, mysql);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库] CreatePlayer fail " + e.Message);
                return false;
            }
        }


        //获取玩家数据
        public static PlayerData GetPlayerData(string id)
        {
            CheckAndReconnect();
            //防止sql注入
            if (!DbManager.IsSafeString(id))
            {
                Console.WriteLine("[数据库] GetPlayerData fail, id not safe");
                return null;
            }

            string sqlStr = string.Format("select * from player where id ='{0}';", id);
            try
            {
                //查询
                MySqlCommand cmd = new MySqlCommand(sqlStr, mysql);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    return null;
                }
                //读取
                dataReader.Read();
                string data = dataReader.GetString("data");
                //反序列化
                PlayerData playerData = Js.Deserialize<PlayerData>(data);
                dataReader.Close();
                return playerData;
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库] GetPlayerData fail, " + e.Message);
                return null;
            }
        }


        //保存角色
        public static bool UpdatePlayerData(string id, PlayerData playerData)
        {
            CheckAndReconnect();
            //序列化
            string data = Js.Serialize(playerData);
            //sql
            string sqlStr = string.Format("update player set data='{0}' where id ='{1}'", data, id);
            //更新
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr, mysql);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库] UpdatePlayerData err, " + e.Message);
                return false;
            }
        }
    }


}
