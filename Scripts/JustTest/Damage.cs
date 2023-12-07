using System;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private const float MaxHp = 100f;
    public float hp = 100;
    public Image bloodBar;

    private void Start()
    {
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        bloodBar.fillAmount = hp / MaxHp;
        if (hp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // transform.rotation = Quaternion.Euler(90, 0, 0);
        // Destroy(gameObject, 1f);
    }
}