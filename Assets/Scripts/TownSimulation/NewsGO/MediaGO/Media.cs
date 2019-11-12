using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TownSimulation.NewsGO.MediaGO
{
    /// <summary>
    /// Class containing information about a media.
    /// </summary>
    public class Media
    {
        private readonly uint id;
        private readonly string url;   //web link of the media
        private readonly byte type; // Whether it's an image (0) , a video (1) or a sound (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="Media"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="url">The URL.</param>
        /// <param name="type">The type.</param>
        public Media(uint id, string url, byte type)
        {
            this.id = id;
            this.type = type;
            this.url = url;
        }

        public uint GetId() { return id; }
        public string GetUrl() { return url; }
        public byte GetMediaType() { return type; }

        /// <summary>
        /// Gets the media type to string.
        /// </summary>
        /// <returns></returns>
        public string GetMediaTypeToString()
        {
            switch (type)
            {
                case 0:
                    return "Image";
                case 1:
                    return "Video";
                case 2:
                    return "Audio";
                default:
                    return "Unknown";
            }
        }
    }
}