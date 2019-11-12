using UnityEngine;

namespace Assets.Scripts.TownSimulation.NewsGO
{
    /// <summary>
    /// This handles the title of the news and the fact that it turns toward you.
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/News/News/NewsSphere/NewsTitle</remarks>
    public class NewsTitle : MonoBehaviour
    {
        private GameObject WhereTolookAt;
        public GameObject ParentSphere;

        void Start()
        {
            WhereTolookAt = GameObject.Find("HeadCollider");
            transform.Rotate(180, 0, 0);
        }

        // The title turn toward the player
        void Update()
        {
            Vector3 v = WhereTolookAt.transform.position - transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt(WhereTolookAt.transform.position - v);
            transform.Rotate(0, 180, 0);
        }
    }
}