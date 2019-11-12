using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.TownSimulation.NewsGO;
using Assets.Scripts.TownSimulation.NewsGO.MediaGO;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Class containing information about a news and methods related to news gameobject.
    /// </summary>
    public class News
    {
        private readonly uint id;
        private readonly string title;
        private readonly string content;
        private readonly float posX;
        private readonly float posZ;
        private readonly uint euclideanDistanceFromSpawn;
        private readonly List<string> tags;
        public uint nbOfView;  // AKA Popularity
        public uint nbComment;
        private readonly DateTime date;
        private readonly List<Media> media;

        private readonly GameObject NewsPreFab = (GameObject)Resources.Load("Prefabs/News/News", typeof(GameObject));

        /// <summary>
        /// Reference to the news gameobject related to this <see cref="News"/> instance.
        /// </summary>
        public GameObject NewsGameObject { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="News"/> class.
        /// </summary>
        /// <param name="id">News ID on database.</param>
        /// <param name="title">News title.</param>
        /// <param name="content">News content.</param>
        /// <param name="posX">The position x in the game world of the news.</param>
        /// <param name="posZ">The position z in the game world of the news.</param>
        /// <param name="nbOfView">The number of views.</param>
        /// <param name="nbComment">The number of comments.</param>
        /// <param name="date">The creation date of the news.</param>
        /// <param name="tags">The tags associate to the news.</param>
        /// <param name="media">The <see cref="Media"/> associate to the news.</param>
        public News(uint id, string title, string content, float posX, float posZ, uint nbOfView, uint nbComment, DateTime date, List<string> tags, List<Media> media)
        {
            this.id = id;
            this.title = title;
            this.content = content;
            this.posX = posX;
            this.posZ = posZ;
            this.euclideanDistanceFromSpawn = StaticClass.DistanceFromSpawn(posX, posZ);
            this.tags = tags;
            this.nbOfView = nbOfView;
            this.nbComment = nbComment;
            this.date = date;
            this.media = media;
        }


        // GETTERS
        public uint GetId() { return id; }
        public uint GetDist() { return euclideanDistanceFromSpawn; }
        public string GetTitle() { return title; }
        public string GetContent() { return content; }
        public Vector3 GetPos() { return new Vector3(posX, 0f, posZ); }
        public List<string> GetTags() { return tags; }
        public DateTime GetDate() { return date; }
        public List<Media> GetMedia() { return media; }

        public string GetTagsToString()
        {
            string buff = "/ ";
            foreach (string s in this.tags)
            {
                buff += s + " / ";
            }
            return buff;
        }

        //use for debug
        public override string ToString()
        {
            return "N: " + this.id + "-" + this.title + " (" + this.euclideanDistanceFromSpawn + ")";
        }


        public void GenerateNewsGameObject(Transform parent)
        {
            GameObject news = UnityEngine.Object.Instantiate(NewsPreFab, parent);
            NewsGameObject newsScript = news.GetComponent<NewsGameObject>();
            newsScript.CreateNews(this);
            this.NewsGameObject = news;
        }

        public Color GetNewsColor()
        {
            if (tags.Count > 0 && StaticClass.tagPrefColorList.ContainsKey(tags[0]))
            {
                return StaticClass.tagPrefColorList[tags[0]];
            }
            else
            {
                return StaticClass.tagDefaultColor;
            }
        }
    }
}
