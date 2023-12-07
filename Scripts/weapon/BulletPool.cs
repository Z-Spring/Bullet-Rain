using System.Collections.Generic;
using UnityEngine;

namespace weapon
{
    /// <summary>
    /// 1. 提前生成一些子弹，然后放到队列中
    /// 2. 取子弹：有的话直接返回子弹，没有的话，直接生成
    /// 3. 放回子弹：将子弹放回队列中即可
    /// </summary>
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool Instance { get; private set; }
        [SerializeField] private int initBulletCount = 100;

        private readonly Queue<GameObject> bulletsPool = new();
        private readonly Queue<GameObject> gameObjectsPool = new();
        private readonly Dictionary<string, GameObject> gameObjects = new();
        private static GameObject bulletRes;
        private static GameObject bloodEffectRes;

        private void Awake()
        {
            Instance = this;

            // bulletRes = ResManager.LoadPrefab("Bullet");
            // bloodEffectRes = ResManager.LoadPrefab("BloodEffect");
            gameObjects.Add("Bullet", bulletRes);

            // InitBulletPool();
        }

        // private void InitBulletPool()
        // {
        //     for (int i = 0; i < initBulletCount; i++)
        //     {
        //         GameObject bullet = BulletSpawn();
        //         bullet.SetActive(false);
        //     }
        // }
        
        private void InitGameObjectPool<T>(int count) 
        {
            string gameObjectName = typeof(T).ToString();
            if (gameObjects.TryGetValue(gameObjectName, out GameObject prefab))
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject spawnGameObject = Instantiate(prefab, transform);
                    spawnGameObject.SetActive(false);
                    gameObjectsPool.Enqueue(spawnGameObject);
                }
            }
        }

        // public GameObject GetBullet()
        // {
        //     GameObject bullet;
        //     if (bulletsPool.Count <= 0)
        //     {
        //         bullet = BulletSpawn();
        //         return bullet;
        //     }
        //
        //     bullet = bulletsPool.Dequeue();
        //     bullet.SetActive(true);
        //     return bullet;
        // }

        public GameObject GetGameObject<T>()
        {
            string gameObjectName = typeof(T).ToString();
            GameObject pp;
            if (gameObjectsPool.Count <= 0)
            {
                pp = GameObjectSpawn<T>();
                return pp;
            }

            pp = gameObjectsPool.Dequeue();
            pp.SetActive(true);
            return pp;
        }

        public void ReturnBullet(GameObject bullet)
        {
            bulletsPool.Enqueue(bullet);
            bullet.SetActive(false);
        }
        
        public void ReturnGameObject(GameObject pp)
        {
            gameObjectsPool.Enqueue(pp);
            pp.SetActive(false);
        }

        private GameObject BulletSpawn()
        {
            GameObject bullet = Instantiate(bulletRes, transform);
            bulletsPool.Enqueue(bullet);
            return bullet;
        }

        private GameObject GameObjectSpawn<T>()
        {
            string gameObjectName = typeof(T).ToString();
            if (gameObjects.TryGetValue(gameObjectName, out GameObject prefab))
            {
                GameObject spawnGameObject = Instantiate(prefab, transform);

                return spawnGameObject;
            }

            return null;
        }
    }
}