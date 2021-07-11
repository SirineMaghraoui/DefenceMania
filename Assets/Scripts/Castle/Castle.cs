using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Castle : MonoBehaviour
{
    public Text healthText;
    [SerializeField]
    float _health;
    // Use this for initialization
    void Start()
    {
        healthText.text = _health.ToString();

    }

    // Update is called once per frame
    void Update()
    {
    }

    public float GetHealth
    {
        get
        {
            return _health;
        }
    }

    public void UpdateHealth(){
        healthText.text=_health.ToString();
    }

  
     public void CastleHit(int hitpoints)
    {

        if (hitpoints<_health)
        {
            _health -= hitpoints;
            UpdateHealth();
        }
        else
        {
            _health=0;
            UpdateHealth();
            Die();
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
}
