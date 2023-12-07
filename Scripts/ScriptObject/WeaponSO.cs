using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScriptObject
{
    [CreateAssetMenu()]
    public class WeaponSO : ScriptableObject
    {
        public int weaponId;
        public string weaponName;
        public Transform weaponPrefab;
        public Sprite weaponIcon;
        public int bulletCountMax;
        public int currentBulletCount;
        public int remainBulletCount;
        public float reloadTime;
        public float shootInterval;
    }
}