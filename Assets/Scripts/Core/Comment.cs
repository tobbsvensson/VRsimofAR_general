using Assets.Scripts.TownSimulation.NewsGO.CommentGO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Class containing information about a comment and methods related to comment gameobject.
    /// Contains a static list of the current in-game loaded comments. Only comments from the open news are loaded.
    /// </summary>
    public class Comment
    {
        public uint IdComment { get; private set; }
        public DateTime Date { get; private set; }
        public string Content { get; private set; }
        public string Author { get; private set; }

        private readonly GameObject commentPreFab = (GameObject)Resources.Load("Prefabs/News/Comment", typeof(GameObject));

        /// <summary>
        /// Static list containing comments loaded ingame.
        /// </summary>
        public static List<Comment> commentsList = new List<Comment>();

        public Comment(){ this.Author = null; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Media"/> class. Only call by <see cref="Database"/> static methods to keep coherence with database contents.
        /// </summary>
        /// <param name="idComment">Comment ID.</param>
        /// <param name="date">Data of creation.</param>
        /// <param name="content">Comment content.</param>
        /// <param name="author">The author of the comment.</param>
        public Comment(uint idComment, DateTime date, string content, string author)
        {
            this.IdComment = idComment;
            this.Date = date;
            this.Content = content;
            this.Author = author;
        }

        /// <summary>
        /// Generate a gameobject with a <see cref="CommentGameObject"/> component attach to it from this <see cref="Comment"/> instance.
        /// </summary>
        /// <param name="commentParent">Parent of the new <see cref="CommentGameObject"/> gameobject.</param>
        public void GenerateGameObject(Transform commentParent)
        {
            GameObject comment = UnityEngine.Object.Instantiate(commentPreFab, commentParent);
            CommentGameObject cmtGO = comment.GetComponent<CommentGameObject>();
            cmtGO.FillText(Content);
            cmtGO.FillAuthor(Author);
            cmtGO.idComment = IdComment;
            cmtGO.DestroyButtons();

            // Add the delete comments option if the current player is the one who made the comments before.
            if (Author == StaticClass.CurrentPlayerName)
            {
                comment.GetComponent<CommentGameObject>().DeleteButton.SetActive(true);
            }

            cmtGO.PlaceComment();
        }

        /// <summary>
        /// Call to delete this <see cref="Comment"/> object and the associate comment on the database.
        /// </summary>
        public void Delete()
        {
            Database.DeleteComment(IdComment);
            commentsList.Remove(this);
        }
    }
}
