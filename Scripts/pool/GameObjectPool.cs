using System.Collections.Generic;
using UnityEngine;

namespace pool
{
    public class BloodEffect : MonoBehaviour
    {
    }

    public class GameObjectPool : MonoBehaviour
    {
        public static GameObjectPool Instance { get; private set; }
        private readonly Dictionary<string, Queue<GameObject>> gameObjectsPool = new();
        private readonly Dictionary<string, GameObject> gameObjects = new();
        private readonly Dictionary<string, Transform> typeToParentMap = new();
        private static GameObject bulletRes;
        private static GameObject bloodEffectRes;

        private const string BulletName = "Bullet";
        private const string BloodEffectName = "BloodEffect";

        [SerializeField] private Transform bulletPool;
        [SerializeField] private Transform bloodEffectPool;

        private void Awake()
        {
            Instance = this;
            // bulletRes = ResManager.LoadPrefab(BulletName);
            // bloodEffectRes = ResManager.LoadPrefab(BloodEffectName);
            bulletRes = ResManager.Instance.loadedBundles["bullet"].LoadAsset<GameObject>(BulletName);
            bloodEffectRes = ResManager.Instance.loadedBundles["effect"].LoadAsset<GameObject>(BloodEffectName);

            gameObjects.Add(BulletName, bulletRes);
            gameObjects.Add(BloodEffectName, bloodEffectRes);

            typeToParentMap.Add(BulletName, bulletPool);
            typeToParentMap.Add(BloodEffectName, bloodEffectPool);
        }

        private void Start()
        {
            InitGameObjectPool<Bullet>(100);
            InitGameObjectPool<BloodEffect>(100);
        }


        private void InitGameObjectPool<T>(int count)
        {
            string gameObjectName = typeof(T).Name;
            if (gameObjects.TryGetValue(gameObjectName, out GameObject prefab))
            {
                Queue<GameObject> queue = new Queue<GameObject>();
                for (int i = 0; i < count; i++)
                {
                    Debug.Log(gameObjectName);

                    GameObject spawnGameObject = Instantiate(prefab, typeToParentMap[gameObjectName]);
                    spawnGameObject.SetActive(false);
                    queue.Enqueue(spawnGameObject);
                }

                gameObjectsPool[gameObjectName] = queue;
            }
        }

        public GameObject GetGameObject<T>()
        {
            string gameObjectName = typeof(T).Name;
            if (gameObjectsPool.TryGetValue(gameObjectName, out Queue<GameObject> queue))
            {
                GameObject obj;
                if (queue.Count <= 0)
                {
                    obj = Instantiate(gameObjects[gameObjectName], typeToParentMap[gameObjectName]);
                    return obj;
                }

                obj = queue.Dequeue();
                obj.SetActive(true);
                return obj;
            }

            return null;
        }


        public void ReturnGameObject<T>(GameObject obj)
        {
            obj.SetActive(false);
            string gameObjectName = typeof(T).ToString();
            if (gameObjectsPool.TryGetValue(gameObjectName, out Queue<GameObject> queue))
            {
                queue.Enqueue(obj);
            }
        }
    }
}