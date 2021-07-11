using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {
  

    GameObject _parent;
    [SerializeField]
    private Transform _exitPoint;
    // Use this for initialization
    void Start () {
        _parent = gameObject.transform.parent.gameObject;


    }

                                               
    void OnTriggerEnter(Collider col)   
        
    {

        if (col.tag == "Projectile")
        {
            Debug.Log("projectile");

            Projectile newP = col.gameObject.GetComponent<Projectile>();
            if (newP != null)
                _parent.GetComponent<Enemy>().EnemyHit(newP.AttackStrength);
            Destroy(col.gameObject);
        }else
    
        if (col.tag == "FinishAlly" && _parent.tag=="Ally")
        {    
                GameManager.Instance.RoundEscapedAllies += 1;
                GameManager.Instance.TotalEscapedAllies += 1;
                //GameManager.Instance.UnregisterAlly(_parent.GetComponentInParent<Enemy>());
                GameManager.Instance.IsWaveOver();
            Destroy(_parent.gameObject);
        }
        else
            if(col.tag == "Finish" && _parent.tag == "Enemy")
        {
            GameManager.Instance.RoundEscaped += 1;
            GameManager.Instance.TotalEscaped += 1;
            //GameManager.Instance.UnregisterEnemy(_parent.GetComponentInParent<Enemy>());
            GameManager.Instance.IsWaveOver();
            Destroy(_parent.gameObject);
        }

    }
}
