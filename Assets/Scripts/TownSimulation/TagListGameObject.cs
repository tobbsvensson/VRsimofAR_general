using Assets.Scripts.Core;
using Assets.Scripts.TownSimulation.NewsGO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TownSimulation
{
    /// <summary>
    /// A horizontal (layout group) tag list. Fill in with the associate tag of the news (reference : <see cref="newsGameObject"/>).
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/Tags</remarks>
    public class TagListGameObject : MonoBehaviour
    {
        public NewsGameObject newsGameObject = null;

        private GameObject tagPreFab;

        private void Awake()
        {
            tagPreFab = (GameObject)Resources.Load("Prefabs/Tag", typeof(GameObject));
        }

        private void OnEnable()
        {
            if (newsGameObject != null)
            {
                foreach (string tag in newsGameObject.Tags)
                {
                    Color color;
                    if (StaticClass.tagPrefColorList.ContainsKey(tag))
                    {
                        color = StaticClass.tagPrefColorList[tag];
                    }
                    else
                    {
                        color = StaticClass.tagDefaultColor;
                    }
                    color.a = 100f / 255f;
                    GameObject tagGO = Instantiate(tagPreFab, transform);
                    tagGO.GetComponent<Image>().color = color;
                    tagGO.GetComponentInChildren<Text>().text = tag;
                    tagGO.GetComponent<ContentSizeFitter>().enabled = true;
                }
            }
        }

        private void OnDisable()
        {
            foreach (Transform tag in transform)
            {
                Destroy(tag.gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}