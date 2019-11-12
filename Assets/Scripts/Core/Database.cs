using Assets.Scripts.DevMode;
using Assets.Scripts.TownSimulation.NewsGO.MediaGO;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Scripts.Core
{
    

    /// <summary>
    /// This file contain all the SQL queries needed to run the game and the connection logs to the database.
    /// All the queries are sorted by SQL table.
    /// SELECT - get data from the db.
    /// INSERT - place new data in the db.
    /// UPDATE - change value in the db.
    /// </summary>
    static class Database 
    {

        /***********************************************************************************************************************************************************/
        /******************************************************************                         ****************************************************************/
        /******************************************************************  CONNECTION PARAMETERS  ****************************************************************/
        /******************************************************************                         ****************************************************************/
        /***********************************************************************************************************************************************************/

        const int VIEW_DELAY_MIN = 10;  //10 min
        public static MySqlConnection con;

        public static void ConnectDB()
        {
            string constr = "Server='mysql-levelup.alwaysdata.net';DATABASE='levelup_newsvr';User ID='levelup';Password='LevelUp20!)';Pooling=true;Charset=utf8;";
            try
            {
                con = new MySqlConnection(constr);
                con.Open();   
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        public static void DisconnectDB()
        {
            try
            {
                con.Dispose();
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        



        /*******************************************************************************************************
         *************************                                                    **************************
         *************************   ALL THE REQUEST WE NEED LISTED BY TABLE (line)   **************************
         *************************                                                    **************************
         *******************************************************************************************************
         *******************************************************************************************************
         *************                             ***************                            ******************
         **************     1 - NEWS (83)           ***************     2 - PLAYER (433)       *****************
         ***************     3 - COMMENTS (723)      ***************     4 - TAGS (852)         ****************
         ****************     5 - NOTIFICATION (946)  ***************     6 - VIEWS (1085)       ***************
         *****************     7 - MEDIA (1245)        ***************     8 - TOPICS (1284)      **************
         ******************                             ***************                            *************
         *******************************************************************************************************/




        /************************************************************************************************/
        /* 1 - NEWS  ************************************************************************************/
        /************************************************************************************************/
        /// <summary>
        /// Create all the c# objects News. Add them to the list "newsList" in StaticClass.cs.
        /// </summary>
        public static void GenerateNewsList()
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT DISTINCT NEWS.idNews, NEWS.title, NEWS.text, NEWS.positionX, NEWS.positionZ, NEWS.nbView, NEWS.nbComment, NEWS.creationDate FROM NEWS ORDER BY NEWS.creationDate DESC;", con);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            //clear
            StaticClass.newsList = new List<News>();

            List<string> tagsTemp;
            List<Media> media;

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tagsTemp = new List<string>();
                        media = new List<Media>();
                        StaticClass.newsList.Add(new News(reader.GetUInt32(0), reader.GetString(1), reader.GetString(2), reader.GetFloat(3), reader.GetFloat(4), reader.GetUInt32(5), reader.GetUInt32(6), reader.GetDateTime(7), tagsTemp, media));
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }
            reader.Dispose();
            cmdSQL.Dispose();

            //get tags from the db
            try
            {
                foreach (News n in StaticClass.newsList)
                {
                    MySqlCommand cmdSQLtags = new MySqlCommand("SELECT * FROM TOPICS WHERE idNews = @dbIdNews ;", con);
                    cmdSQLtags.Parameters.AddWithValue("@dbIdNews", n.GetId());
                    MySqlDataReader readerTags = cmdSQLtags.ExecuteReader();

                    if (readerTags.HasRows)
                    {
                        while (readerTags.Read())
                        {
                            n.GetTags().Add(readerTags.GetString(1));
                        }
                    }
                    Debug.Log(n.GetTags().Count);
                    readerTags.Dispose();
                    cmdSQLtags.Dispose();
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }

            //get media from the db
            try
            {
                foreach (News n in StaticClass.newsList)
                {
                    MySqlCommand cmdSQLtags = new MySqlCommand("SELECT * FROM MEDIA WHERE idNews = @dbIdNews ;", con);
                    cmdSQLtags.Parameters.AddWithValue("@dbIdNews", n.GetId());
                    MySqlDataReader readerTags = cmdSQLtags.ExecuteReader();

                    if (readerTags.HasRows)
                    {
                        while (readerTags.Read())
                        {
                            n.GetMedia().Add(new Media(readerTags.GetUInt32(0), readerTags.GetString(2), readerTags.GetByte(3)));
                        }
                    }
                    Debug.Log(n.GetTags().Count);
                    readerTags.Dispose();
                    cmdSQLtags.Dispose();
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }
            DisconnectDB();
        }

        /// <summary>
        /// Get the numb of comment for a news.
        /// </summary>
        /// <param name="idNews">id of the targeted news</param>
        /// <returns>Numb. of comment for this news</returns>
        public static string ReadComntNum(uint idNews)
        {
            string response;

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT nbComment FROM NEWS WHERE idNews = @dbNewsId", con);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", idNews);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    int res = reader.GetInt32(0);
                    response = "" + res;
                }
                else
                {
                    response = "/";
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
                response = "/";
            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return response;
        }

        /// <summary>
        /// Increment the number of view for a news. Call when a comment is entered.
        /// </summary>
        static void Add1ViewToNews()
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("UPDATE NEWS SET nbView = nbView + 1 WHERE idNews = @dbNewsId", con);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", StaticClass.CurrentNewsId);

            try
            {
                cmdSQL.ExecuteNonQuery();
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();
        }

        /// <summary>
        /// Get the numb. of reaction for a news
        /// </summary>
        /// <param name="rea">Type of reatcion (Sad, Happy, ...)</param>
        /// <param name="idNews">id of the targeted news</param>
        /// <returns>Numb. of reaction for this news</returns>
        public static string NumOfReatcionToNews(string rea, uint idNews)
        {
            string response = "/";

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT nb" + rea + " FROM NEWS WHERE idNews = @dbNewsId", con);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", idNews);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    response = Convert.ToString(reader.GetUInt32(0));
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return response;
        }

        /// <summary>
        /// Get the numb. of view for a news
        /// </summary>
        /// <param name="id">id of the targeted news</param>
        /// <returns>Numb. of view for this news</returns>
        internal static string ReadViewNum(uint id)
        {
            string response = "/";

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT nbView FROM NEWS WHERE idNews = @dbNewsId", con);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", id);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    int res = reader.GetInt32(0);
                    response = "" + res;
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return response;
        }

        /// <summary>
        /// Decrement the numb. of a reaction for a news. Call when the player change his reaction.
        /// </summary>
        /// <param name="reactionType">Old player reaction's choice</param>
        public static void RemoveOneReasctionCount(string reactionType)
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("UPDATE NEWS SET nb" + reactionType + " = nb" + reactionType + " - 1 WHERE idNews = @dbNewsId", con);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", StaticClass.CurrentNewsId);

            try
            {
                cmdSQL.ExecuteNonQuery();
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();
        }

        /// <summary>
        /// Increment the numb. of a reaction (player choice)
        /// </summary>
        /// <param name="reactionType">player reaction's choice</param>
        /// <param name="idNews">id of the targeted news</param>
        public static void AddReactionToDatabaseNews(string reactionType, uint idNews)
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("UPDATE NEWS SET nb" + reactionType + " = nb" + reactionType + " + 1 WHERE idNews = @dbNewsId", con);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", idNews);

            try
            {
                cmdSQL.ExecuteNonQuery();
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();
        }

        /// <summary>
        /// Call when a news is created.
        /// </summary>
        /// <param name="title">Title of the news</param>
        /// <param name="text">Text of the news</param>
        /// <param name="posX">x cartesian coordonate</param>
        /// <param name="posZ">z cartesian coordonate</param>
        public static void CreateANews(string title, string text, float posX, float posZ)
        {
            ConnectDB();
            MySqlCommand cmdCreateNews = new MySqlCommand("INSERT INTO NEWS(title, text, author, creationDate, nbView, nbComment, nbHappy, nbSad, nbAngry, nbSurprised, positionX, positionZ) VALUES(@dbNewsCreaTitle,@dbNewsCreaText,@dbNewsCreaAuthor, @dbNewsCreaDate,0,0,0,0,0,0,@dbNewsCreaPosiX, @dbNewsCreaPosiZ);", Database.con);
            cmdCreateNews.Parameters.AddWithValue("@dbNewsCreaTitle", title);
            cmdCreateNews.Parameters.AddWithValue("@dbNewsCreaText", text);
            cmdCreateNews.Parameters.AddWithValue("@dbNewsCreaAuthor", StaticClass.CurrentPlayerName);
            cmdCreateNews.Parameters.AddWithValue("@dbNewsCreaDate", DateTime.Now);
            cmdCreateNews.Parameters.AddWithValue("@dbNewsCreaPosiX", posX);
            cmdCreateNews.Parameters.AddWithValue("@dbNewsCreaPosiZ", posZ);

            try
            {
                cmdCreateNews.ExecuteReader();
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }

            cmdCreateNews.Dispose();
            DisconnectDB();
        }

        /// <summary>
        /// Get the id of the Last News Created.
        /// </summary>
        /// <returns>id of the last news</returns>
        public static int LastNewsCreated()
        {
            int res = -1;

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT MAX(idNews) FROM NEWS;", con);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    res = reader.GetInt32(0);
                }

            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return res;
        }

        
        /// <summary>
        /// Delete a news from its id.
        /// </summary>
        /// <param name="id">id of the targeted news</param>
        public static void DeleteNews(uint id)
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("DELETE FROM `NEWS` WHERE `idNews` = @dbIdNews;", con);
            cmdSQL.Parameters.AddWithValue("@dbIdNews", id);

            try
            {
                cmdSQL.ExecuteNonQuery();
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();
        }

        /************************************************************************************************/
        /* 2 - PLAYERS  *********************************************************************************/
        /************************************************************************************************/

        /// <summary>
        /// Generate a dictionnary id/name of each player of db
        /// </summary>
        /// <returns>dictionnary id/name of each player of db</returns>
        public static Dictionary<uint, string> GetPlayers()
        {
            Dictionary<uint, string> response = new Dictionary<uint, string>();

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT idPlayer, name FROM `PLAYERS` ORDER BY name", con);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        response.Add(reader.GetUInt32(0), reader.GetString(1));
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return response;
        }

        /// <summary>
        /// Get the numb. of comment of the connected player
        /// </summary>
        /// <returns>Numb. of comment of the connected player</returns>
        public static int ReadNbrCommentDisplayed()
        {
            ConnectDB();
            string sqlCmdName = "SELECT cmtNbShown FROM PLAYERS WHERE name = @dbUserName;";
            MySqlCommand cmdSQL = new MySqlCommand(sqlCmdName, con);
            cmdSQL.Parameters.AddWithValue("@dbUserName", StaticClass.CurrentPlayerName);
            MySqlDataReader reader = cmdSQL.ExecuteReader();
            int response;

            try
            {
                if (reader.Read())
                {
                    int res = reader.GetInt32(0);
                    response = res;
                }
                else
                {
                    response = 2; ///default number shown
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
                response = 2;  ///default number shown
            }
            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return response;
        }

        /// <summary>
        /// Get the perferred comment's position of a player
        /// </summary>
        /// <returns> 0 = left, 1 = Above, 2 = Right, 3 = Behind </returns>
        public static int ReadCommentPosition()
        {
            ConnectDB();
            string sqlCmdName = "SELECT cmtPositionPref FROM PLAYERS WHERE name = @dbUserName;";
            MySqlCommand cmdSQL = new MySqlCommand(sqlCmdName, con);
            cmdSQL.Parameters.AddWithValue("@dbUserName", StaticClass.CurrentPlayerName);
            MySqlDataReader reader = cmdSQL.ExecuteReader();
            int response;

            try
            {
                if (reader.Read())
                {
                    int res = reader.GetInt32(0);
                    response = res;
                }
                else
                {
                    response = 0; ///default position left
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
                response = 0;  ///default position left
            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return response;
        }

        /// <summary>
        /// Before a player register himself, check if his name is not already taken
        /// </summary>
        /// <param name="name">content of the player name input field</param>
        /// <returns>true if the name is available</returns>
        public static bool VerifNameAvailable(string name)
        {
            ConnectDB();

            bool response = false;

            string sqlCmdName = "SELECT name FROM PLAYERS WHERE name = @dbUserName;";
            MySqlCommand cmdVerifName = new MySqlCommand(sqlCmdName, con);
            cmdVerifName.Parameters.AddWithValue("@dbUserName", name);
            MySqlDataReader reader = cmdVerifName.ExecuteReader();
            
            try
            {                
                //if there is no result, this name is available
                if (!reader.Read())
                {
                    response = true;
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }

            reader.Dispose();
            cmdVerifName.Dispose();
            DisconnectDB();
            return response;
        }


        /// <summary>
        /// Create a new player in DB - call when registration
        /// </summary>
        /// <param name="name">name of the player</param>
        /// <param name="password">password of the player</param>
        /// <returns>true if the player is correctly created</returns>
        public static bool InsertNewPlayer(string name,string password)
        {
            ConnectDB();
            MySqlCommand cmdReg = new MySqlCommand("INSERT INTO PLAYERS VALUES (default,@dbUserName,@dbUserMDP,0,0,0,2,default);", con);
            cmdReg.Parameters.AddWithValue("@dbUserName", name);
            cmdReg.Parameters.AddWithValue("@dbUserMDP", password);
            bool response;

            try
            {
                cmdReg.ExecuteReader();
                response = true;
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
                response = false;
            }

            cmdReg.Dispose();
            DisconnectDB();
            return response;
        }


        /// <summary>
        /// Check if the pair name/password is related to an authentic player
        /// </summary>
        /// <param name="name">name of the player</param>
        /// <param name="password">password of the player</param>
        /// <returns>true if the player exist</returns>
        public static bool IsThisUserAnAuthenticPlayer(string name, string password)
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT idPlayer FROM PLAYERS WHERE name = @dbUserName AND password = @dbUserMDP;", con);
            cmdSQL.Parameters.AddWithValue("@dbUserName", name);
            cmdSQL.Parameters.AddWithValue("@dbUserMDP", password);
            MySqlDataReader reader = cmdSQL.ExecuteReader();
            bool response = false;

            try
            {
                if (reader.Read())
                {
                    StaticClass.CurrentPlayerId = reader.GetUInt32(0);
                    response = true;
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }
            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectName"></param>
        /// <returns></returns>
        public static string SqlCmd(string selectName)
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT " + selectName + " FROM PLAYERS WHERE name = '" + StaticClass.CurrentPlayerName + "'", con);
            MySqlDataReader reader = cmdSQL.ExecuteReader();
            string response;
            try
            {
                reader.Read();
                response = "" + reader.GetValue(0);
            }
            catch (IOException ex)
            {
                response = "error";
                Debug.Log(ex);
            }
            cmdSQL.Dispose();
            reader.Dispose();
            DisconnectDB();
            return response;
        }

        /// <summary>
        /// Save the player preferences
        /// </summary>
        /// <param name="cmtNumbers"></param>
        /// <param name="cmtPosition">player choice for comment position</param>
        /// <returns>True if the update goes well</returns>
        public static bool PrefSucessfullySaved(int cmtNumbers, int cmtPosition)
        {
            bool response = false;

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("UPDATE PLAYERS SET cmtNbShown = '" + cmtNumbers + "', cmtPositionPref = '" + cmtPosition + "' WHERE name = '" + StaticClass.CurrentPlayerName + "'; ", con);

            try
            {
                //if there is a line that have been updated, then the choice is saved
                if (cmdSQL.ExecuteNonQuery() > 0)
                {
                    response = true;
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();
            return response;
        }

        /// <summary>
        /// Increment the player numb. of view. Call when a player pick a news
        /// </summary>
        static void Add1ViewToPlayer()
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("UPDATE PLAYERS SET nbOfView = nbOfView + 1 WHERE idPlayer = @dbUserId", con);
            cmdSQL.Parameters.AddWithValue("@dbUserId", StaticClass.CurrentPlayerId);

            try
            {
                cmdSQL.ExecuteNonQuery();
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();
        }

        /************************************************************************************************/
        /* 3 - COMMENTS  ********************************************************************************/
        /************************************************************************************************/

        /// <summary>
        /// Add comment to database.
        /// </summary>
        /// <param name="idNews">id of the commented news</param>
        /// <param name="text">text of the comment</param>
        public static void AddComment(uint idNews, string text)
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("INSERT INTO COMMENTS (idNews, idPlayer, text, date) VALUES(@dbNewsId,@dbUserId,@dbComtText,@dbDate); ", con);
            cmdSQL.Parameters.AddWithValue("@dbComtText", text);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", idNews);
            cmdSQL.Parameters.AddWithValue("@dbUserId", StaticClass.CurrentPlayerId);
            cmdSQL.Parameters.AddWithValue("@dbDate", DateTime.Now); //set the date of creation to now
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                reader.Read();
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }
            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
        }



        /// <summary>
        /// Delete a comment from its id
        /// </summary>
        /// <param name="id">id of the comment to delete</param>
        static public void DeleteComment(uint id)
        {
            ConnectDB();
            MySqlCommand cmdDeleteAction = new MySqlCommand("DELETE FROM COMMENTS WHERE idComment = @dbIdComment;", Database.con);
            cmdDeleteAction.Parameters.AddWithValue("@dbIdComment", id);

            try
            {
                cmdDeleteAction.ExecuteReader();
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }

            cmdDeleteAction.Dispose();
            DisconnectDB();
        }


        /// <summary>
        /// Get all the comment of a news
        /// </summary>
        /// <param name="idNews">id of the news</param>
        /// <returns>List of <see cref="Comment"/> (c# objects)</returns>
        static public List<Comment> QueryComments(uint idNews)
        {
            List<Comment> cmntList = new List<Comment>();

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT idComment,date,text,PLAYERS.name FROM `COMMENTS` INNER JOIN PLAYERS ON COMMENTS.idPlayer = PLAYERS.idPlayer WHERE IdNews = " + idNews + " ORDER BY date DESC;", con);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", idNews);
            MySqlDataReader reader = cmdSQL.ExecuteReader();


            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        cmntList.Add(new Comment(reader.GetUInt32(0), reader.GetDateTime(1), reader.GetString(2), reader.GetString(3)));
                    }
                }

            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());

            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return cmntList;
        }

        /// <summary>
        /// Get the last created comment of the current news
        /// </summary>
        /// <returns>last comment (c# object)</returns>
        static public Comment GetLastComment()
        {
            Comment cmt = new Comment();

            ConnectDB();
            MySqlCommand cmdLastComment = new MySqlCommand("SELECT idComment,date,text FROM COMMENTS WHERE idPlayer = @dbIdPlayer AND idNews = @dbIdNews ORDER BY idComment DESC LIMIT 1;", Database.con);
            cmdLastComment.Parameters.AddWithValue("@dbIdPlayer", StaticClass.CurrentPlayerId);
            cmdLastComment.Parameters.AddWithValue("@dbIdNews", StaticClass.CurrentNewsId);
            MySqlDataReader reader = cmdLastComment.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    cmt = new Comment(reader.GetUInt32(0), reader.GetDateTime(1), reader.GetString(2), StaticClass.CurrentPlayerName);
                }

            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }

            reader.Dispose();
            cmdLastComment.Dispose();
            DisconnectDB();
            return cmt;
        }

        /************************************************************************************************/
        /* 4 - TAGS  ************************************************************************************/
        /************************************************************************************************/

        /// <summary>
        /// Get all the tags
        /// </summary>
        /// <returns>List of tags (String)</returns>
        public static List<string> GetTags()
        {
            List<string> tagsList = new List<string>();

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT `tagLabel` FROM `TAGS`;", con);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tagsList.Add(reader.GetString(0));
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return tagsList;
        }



        /// <summary>
        /// Insert a tag in the db. Call when a tag is created.
        /// </summary>
        /// <param name="tag">name of the tag</param>
        /// <returns>true if the tag is correctly created</returns>
        public static bool InsertTag(string tag)
        {
            bool res = false;

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("INSERT INTO `TAGS`(`tagLabel`) VALUES (@dbTagName);", con);
            cmdSQL.Parameters.AddWithValue("@dbTagName", tag);

            try
            {
                if (cmdSQL.ExecuteNonQuery() > 0)
                {
                    res = true;
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();

            return res;
        }

        /// <summary>
        /// Delete tag from its name.
        /// </summary>
        /// <param name="tag">name of the tag to delete</param>
        public static void RemoveTag(string tag)
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("DELETE FROM `TAGS` WHERE `tagLabel` = @dbTagName;", con);
            cmdSQL.Parameters.AddWithValue("@dbTagName", tag);

            try
            {
                cmdSQL.ExecuteNonQuery();
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();
        }


        /************************************************************************************************/
        /* 5 - NOTIFICATIONS  ***************************************************************************/
        /************************************************************************************************/

        /// <summary>
        /// Get each pair tag/color set by the player. Insert them in tagPrefColorList of StaticClass.cs
        /// </summary>
        public static void GetTagColors()
        {
            ConnectDB();
            StaticClass.tagPrefColorList.Clear();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT tagName, color FROM `NOTIFICATIONS` WHERE idPlayer= @dbUserId;", con);
            cmdSQL.Parameters.AddWithValue("@dbUserId", StaticClass.CurrentPlayerId);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (ColorUtility.TryParseHtmlString(reader.GetString(1), out Color color))
                        {
                            StaticClass.tagPrefColorList.Add(reader.GetString(0), color);
                        }
                        else
                        {
                            Debug.Log("Impossible to read hexadecimal color on database. Set to default (white).");
                        }
                    }
                }
                else  // if there is nothing from this id player in the db yet, initialize every tag to white
                {
                    
                    foreach (string s in Database.GetTags())
                    {
                        StaticClass.tagPrefColorList.Add(s, StaticClass.tagDefaultColor);
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
                
            }
            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
        }

        internal static void AddDefaultNotificationByTag(string tag)
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("INSERT INTO `NOTIFICATIONS`(`tagName`, `idPlayer`, `color`) SELECT @dbTextTag AS tagName, idPlayer, @dbHexColor AS color FROM PLAYERS;", con);
            cmdSQL.Parameters.AddWithValue("@dbTextTag", tag);
            cmdSQL.Parameters.AddWithValue("@dbHexColor", "#FFFFAF");
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                reader.Read();
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }
            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
        }


        /// <summary>
        /// Create default tag/color pair for a player
        /// </summary>
        /// <param name="tag">tag name</param>
        /// <param name="player">name of the player</param>
        /// <returns>true if everything goes well</returns>
        public static bool InsertTagColorChoice(string tag, string player)
        {
            // Add comment to database
            bool res;
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("INSERT INTO NOTIFICATIONS (tagName, idPlayer, color) VALUES(@dbTextTag,(SELECT idPlayer FROM PLAYERS WHERE name = @dbUserId), @dbHexColor);", con);
            cmdSQL.Parameters.AddWithValue("@dbTextTag", tag);
            cmdSQL.Parameters.AddWithValue("@dbHexColor", "#FFFFAF");
            cmdSQL.Parameters.AddWithValue("@dbUserId", player);        
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                reader.Read();
                res = true;
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
                res = false;
            }
            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return res;
        }

        /// <summary>
        /// Save a pair tag/color set by the player.
        /// </summary>
        /// <param name="tag">tag name</param>
        /// <param name="hexColor">color in hexa</param>
        /// <returns></returns>
        public static bool ChangeTagColorChoice(string tag, string hexColor)
        {
            bool res = false;
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("UPDATE `NOTIFICATIONS` SET `color`= @dbHexColor WHERE `tagName`= @dbTextTag AND `idPlayer`= @dbPlayerId", con);
            cmdSQL.Parameters.AddWithValue("@dbTextTag", tag);
            cmdSQL.Parameters.AddWithValue("@dbHexColor", hexColor);
            cmdSQL.Parameters.AddWithValue("@dbPlayerId", StaticClass.CurrentPlayerId);
           
            try
            {
                //if there is a line that have been updated, then the choice is saved
                if (cmdSQL.ExecuteNonQuery() > 0)
                {
                    res = true;
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();
            return res;
        }


        /************************************************************************************************/
        /* 6 - VIEWS  ***********************************************************************************/
        /************************************************************************************************/
       
        //just to make sure they are always called together
        public static void CountView()
        {
            Add1ViewToNews();
            Add1ViewToPlayer();
        }

        /// <summary>
        /// Get the selected reaction for current news, set by the current player.
        /// </summary>
        /// <returns> 0 = none, 1 = Happy, 2 = Sad, 3 = Angry, 4 = Surprised <returns>
        public static byte ReadReactionSelected()
        {
            byte res = 0;

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT reactionSelected FROM VIEWS WHERE idNews = @dbNewsId AND idPlayer = @dbPlayerId", con);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", StaticClass.CurrentNewsId);
            cmdSQL.Parameters.AddWithValue("@dbPlayerId", StaticClass.CurrentPlayerId);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    res = reader.GetByte(0);
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return res;
        }


        /// <summary>
        /// Save the selected reaction
        /// </summary>
        /// <param name="b">reatcion 0 = none, 1 = Happy, 2 = Sad, 3 = Angry, 4 = Surprised </param>
        public static void SaveReactionSelected(byte b)
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("UPDATE `VIEWS` SET `reactionSelected`= @dbReacToSave WHERE idNews = @dbNewsId AND idPlayer = @dbPlayerId", con);
            cmdSQL.Parameters.AddWithValue("@dbReacToSave", b);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", StaticClass.CurrentNewsId);
            cmdSQL.Parameters.AddWithValue("@dbPlayerId", StaticClass.CurrentPlayerId);

            try
            {
                cmdSQL.ExecuteNonQuery();
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();
        }

        /// <summary>
        /// First time the player enter a news, set the date of view to now
        /// </summary>
        static void InsertDateTimeView()
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("INSERT INTO VIEWS (idNews, idPlayer, `dateLatestView`,reactionSelected ) VALUES (@dbNewsId,@dbPlayerId,@dbDateTime,0);", con);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", StaticClass.CurrentNewsId);
            cmdSQL.Parameters.AddWithValue("@dbPlayerId", StaticClass.CurrentPlayerId);
            cmdSQL.Parameters.AddWithValue("@dbDateTime", DateTime.Now);

            try
            {
                cmdSQL.ExecuteNonQuery();
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();
        }

        /// <summary>
        /// When the player enter a news, set the date of view to now
        /// </summary>
        static void UpdateDateTimeView()
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("UPDATE VIEWS SET `dateLatestView` = @dbDateTime WHERE idNews = @dbNewsId AND idPlayer = @dbPlayerId;", con);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", StaticClass.CurrentNewsId);
            cmdSQL.Parameters.AddWithValue("@dbPlayerId", StaticClass.CurrentPlayerId);
            cmdSQL.Parameters.AddWithValue("@dbDateTime", DateTime.Now);

            try
            {
                cmdSQL.ExecuteNonQuery();
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();
        }

        /// <summary>
        /// Count the view if the last time the player saw this news is less than 10 min.
        /// </summary>
        public static void ViewCountApproval()
        {
            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT `dateLatestView` FROM VIEWS WHERE `idNews` =  @dbNewsId AND `idPlayer` = @dbPlayerId;", con);
            cmdSQL.Parameters.AddWithValue("@dbNewsId", StaticClass.CurrentNewsId);
            cmdSQL.Parameters.AddWithValue("@dbPlayerId", StaticClass.CurrentPlayerId);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    DateTime dt = reader.GetDateTime(0);

                    //true if the save datetime is at least passed by 10 minutes
                    if (DateTime.Compare(dt.AddMinutes(VIEW_DELAY_MIN), DateTime.Now) <= 0)
                    {
                        UpdateDateTimeView();
                        CountView();
                    }
                }
                else
                {
                    InsertDateTimeView();
                    CountView(); //if there is no result, the player never saw this news, so we approved the view count
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
        }



        /************************************************************************************************/
        /* 7 - MEDIA  ***********************************************************************************/
        /************************************************************************************************/

        /// <summary>
        /// Insert a media for a news
        /// </summary>
        /// <param name="idNews">id of the news</param>
        /// <param name="url">url of the media</param>
        /// <param name="type">type of the media : image (0) , a video (1) or a sound (2)</param>
        /// <returns></returns>
        public static bool InsertMedia(int idNews,string url,int type)
        {
            bool res = false;

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("INSERT INTO `MEDIA`(`idNews`, `link`, `type`) VALUES (@dbIdNews,@dbUrl,@dbType);", con);
            cmdSQL.Parameters.AddWithValue("@dbIdNews", idNews);
            cmdSQL.Parameters.AddWithValue("@dbUrl", url);
            cmdSQL.Parameters.AddWithValue("@dbType", type);

            try
            {
                if (cmdSQL.ExecuteNonQuery() > 0)
                {
                    res = true;
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();

            return res;
        }

        /************************************************************************************************/
        /* 6 - TOPICS  **********************************************************************************/
        /************************************************************************************************/

        /// <summary>
        /// Link a news to a tag
        /// </summary>
        /// <param name="idNews">id of the news</param>
        /// <param name="tag">name of the tag</param>
        /// <returns>true if everythings goes well</returns>
        public static bool InsertTopic(int idNews, string tag)
        {
            bool res = false;

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("INSERT INTO `TOPICS`(`tagName`, `idNews`) VALUES (@dbTagName, @dbIdNews);", con);
            cmdSQL.Parameters.AddWithValue("@dbTagName", tag);
            cmdSQL.Parameters.AddWithValue("@dbIdNews", idNews);

            try
            {
                if (cmdSQL.ExecuteNonQuery() > 0)
                {
                    res = true;
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex);
            }

            cmdSQL.Dispose();
            DisconnectDB();

            return res;
        }

        //SELECT NEWS.idNews AS ID, (SELECT name FROM PLAYERS WHERE idPlayer = 3) Player, NEWS.title, VIEWS.reactionSelected,( SELECT COUNT(`idComment`) FROM `COMMENTS` JOIN NEWS ON COMMENTS.idNews = NEWS.idNews WHERE idPlayer = 3 AND NEWS.idNews = ID ) NumOfCmt, VIEWS.dateLatestView FROM VIEWS JOIN NEWS ON NEWS.idNews = VIEWS.idNews WHERE idPlayer=3 ORDER BY VIEWS.dateLatestView DESC
        //SELECT VIEWS.idPlayer AS ID, PLAYERS.name,(SELECT title FROM NEWS WHERE idNews = 3) Title, VIEWS.reactionSelected, (SELECT COUNT(`idComment`) FROM `COMMENTS` JOIN NEWS ON COMMENTS.idNews = NEWS.idNews WHERE idPlayer = ID AND NEWS.idNews = 3) NumOfCmt, VIEWS.dateLatestView FROM VIEWS JOIN PLAYERS ON PLAYERS.idPlayer = VIEWS.idPlayer WHERE idNews = 3 ORDER BY VIEWS.dateLatestView DESC

        /********************/
        /********************/
        /****  DEVMODE  *****/
        /********************/
        /********************/


        /// <summary>
        /// Get the last views for admin page
        /// </summary>
        /// <param name="idNews">news id selected</param>
        /// <param name="idPlayer">player id selected</param>
        /// <param name="filterNews">activate news filter</param>
        /// <param name="filterPlayer">activate player filter</param>
        /// <returns>List of logs</returns>
        public static List<DevStatsData> GetDevStatsView(uint idNews, uint idPlayer, bool filterNews, bool filterPlayer)
        {
            List<DevStatsData> response = new List<DevStatsData>();
            string specialCondition = "";

            if (filterNews)
            {
                if (filterPlayer)
                    specialCondition = "WHERE NEWS.idNews = " + idNews.ToString() + " AND PLAYERS.idPlayer = " + idPlayer.ToString();
                else
                    specialCondition = "WHERE NEWS.idNews = " + idNews.ToString();
            }
            else
            {
                if (filterPlayer)
                    specialCondition = "WHERE PLAYERS.idPlayer = " + idPlayer.ToString();
            }

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT NEWS.idNews AS IDN, VIEWS.idPlayer AS IDP, (SELECT name FROM PLAYERS WHERE idPlayer = IDP) Player, (SELECT title FROM NEWS WHERE idNews = IDN) Title, VIEWS.reactionSelected,( SELECT COUNT(`idComment`) FROM `COMMENTS` JOIN NEWS ON COMMENTS.idNews = NEWS.idNews WHERE idPlayer = IDP AND NEWS.idNews = IDN ) NumOfCmt, VIEWS.dateLatestView FROM VIEWS JOIN NEWS ON NEWS.idNews = VIEWS.idNews JOIN PLAYERS ON PLAYERS.idPlayer = VIEWS.idPlayer "+ specialCondition + " ORDER BY VIEWS.dateLatestView DESC", con);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        response.Add(new DevStatsData(reader.GetString(2), reader.GetString(3), reader.GetUInt32(4), reader.GetUInt32(5), reader.GetDateTime(6)));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return response;
        }

        /// <summary>
        /// Get the comments for admin page
        /// </summary>
        /// <param name="idNews">news id selected</param>
        /// <param name="idPlayer">player id selected</param>
        /// <param name="filterNews">activate news filter</param>
        /// <param name="filterPlayer">activate player filter</param>
        /// <returns>List of logs</returns>
        public static Dictionary<Comment, string> GetDevStatsComment(uint idNews, uint idPlayer, bool filterNews, bool filterPlayer)
        {
            Dictionary<Comment, string> response = new Dictionary<Comment, string>();
            string specialCondition = "";

            if (filterNews)
            {
                if (filterPlayer)
                    specialCondition = "WHERE NEWS.idNews = " + idNews.ToString() + " AND PLAYERS.idPlayer = " + idPlayer.ToString();
                else
                    specialCondition = "WHERE NEWS.idNews = " + idNews.ToString();
            }
            else
            {
                if (filterPlayer)
                    specialCondition = "WHERE PLAYERS.idPlayer = " + idPlayer.ToString();
            }

            ConnectDB();
            MySqlCommand cmdSQL = new MySqlCommand("SELECT NEWS.idNews AS IDN, COMMENTS.idPlayer AS IDP, COMMENTS.idComment AS IDC, (SELECT name FROM PLAYERS WHERE idPlayer = IDP) Player, (SELECT title FROM NEWS WHERE idNews = IDN) Title, (SELECT text FROM COMMENTS WHERE idComment = IDC) Content, (SELECT date FROM COMMENTS WHERE idComment = IDC) CreationDate FROM COMMENTS JOIN NEWS ON NEWS.idNews = COMMENTS.idNews JOIN PLAYERS ON PLAYERS.idPlayer = COMMENTS.idPlayer " + specialCondition+" ORDER BY COMMENTS.date DESC", con);
            MySqlDataReader reader = cmdSQL.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        response.Add(new Comment(reader.GetUInt32(2), reader.GetDateTime(6), reader.GetString(5), reader.GetString(3)), reader.GetString(4));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

            reader.Dispose();
            cmdSQL.Dispose();
            DisconnectDB();
            return response;
        }
    }
}
