using System.Collections.Generic;
using UnityEngine;

namespace ScriptObject
{
    [CreateAssetMenu()]
    public class WeaponListSO : ScriptableObject
    {
        public List<WeaponSO> weaponList;
        // public WeaponSO m4;
        // public WeaponSO l96;
        // public WeaponSO rifle;
        // public WeaponSO scifi;
    }
}