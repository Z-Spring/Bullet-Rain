using System.Collections.Generic;
using ScriptObject;
using UnityEngine;

namespace UI.GameSceneUI
{
    public class GameSceneUI : MonoBehaviour
    {
        internal readonly Dictionary<WeaponSO, int[]> bulletDict = new();
        [SerializeField] private WeaponListSO weaponListSo;

        private void SetInitWeaponBullets()
        {
            foreach (var weapon in weaponListSo.weaponList)
            {
                bulletDict.Add(weapon, new[] { weapon.currentBulletCount, weapon.remainBulletCount });
            }
        }
    }
}