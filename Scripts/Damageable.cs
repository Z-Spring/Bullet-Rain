using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Damageable : MonoBehaviour
{
    [SerializeField] private Image bloodBar;
    [SerializeField] private float maxHp = 100;
    
    public float hp;
    [HideInInspector]
    public UnityEvent OnDie;

    private void Start()
    {
        hp = maxHp;
        bloodBar.fillAmount = 1f;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        bloodBar.fillAmount = hp / maxHp;
        if (hp <= 0)
        {
            OnDie?.Invoke();
        }
    }
}