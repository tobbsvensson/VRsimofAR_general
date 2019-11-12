using Assets.Scripts.TownSimulation.NewsGO.CommentGO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// A static class containing player information and settings.
    /// Also contains spawn coordinates and a list of all loaded <see cref="News"/>.
    /// </summary>
    public static class StaticClass
    {

        // Player infos
        public static string CurrentPlayerName = "";
        public static uint CurrentPlayerId;
        public static uint CurrentNewsId;


        // Comments settings
        public static int nbrCommentDisplayed;
        public static CommentGameObject.Positions CommentPosition
        {
            set
            {
                CommentGameObject.SetCommentsPosition(value);
            }
        }

        // News loaded
        public static List<News> newsList = new List<News>();

        // Spawn coordinates.
        public const float SPAWN_X = -95.7f;
        public const float SPAWN_Z = 87.3f;

        // Tags settings
        public static List<uint> newsBeaconedList = new List<uint>();
        public static Dictionary<string, Color> tagPrefColorList = new Dictionary<string, Color>();
        public static Color tagDefaultColor = new Color(1, 1, 175f / 255f);


        //return the euclidian distance between a new's location and the spawn
        public static uint DistanceFromSpawn(float x, float y) => Convert.ToUInt32(Math.Sqrt(((x - SPAWN_X) * (x - SPAWN_X) + (y - SPAWN_Z) * (y - SPAWN_Z))));


        public static void GoBackToMenu()
        {
            SceneManager.LoadScene(0);
        }

    }
}