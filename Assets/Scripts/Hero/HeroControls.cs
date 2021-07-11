using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroControls : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
  [SerializeField]
    private AgentScript _agent;
    [SerializeField]
    float _health;
    public Text healthText;

    [SerializeField]
    int damage = 3;

    [SerializeField]
    float attackFrequency;
    float attackF = 0;

    [SerializeField]
    private float _navigationUpdate;
    private float _navigationTime = 0;

    [SerializeField]
    float walkSpeed = 1;

    [SerializeField]
    private List<Tower> _towersInRange = new List<Tower>();

    [SerializeField]
    private List<Enemy> _enemiesInRange = new List<Enemy>();

    [SerializeField]
    Tower nearestTower;

    [SerializeField]
    Enemy nearestEnemy;

    [SerializeField]
    Castle enemyCastle = null;

    [SerializeField]
    string nearst = "";


    RaycastHit2D hit;
    private Vector3 mouseClickPos;
    GameObject target;
    bool walk = false;
        [SerializeField]

    private bool _isDead = false;
    public Transform follow;
    public float zOffset;
    void Start()
    {
        UpdateHealth();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        mouseClickPos = transform.position;
      


    }
    // Update is called once per frame
    void Update()
    {

        
        if(!IsDead && GameManager.Instance.currentState==GameStatus.duringWave)
        {
 //mouseclick
        reachDestination();
           //Debug.Log("player" +(transform.position.x - _agent.Agent.destination.x));
        
           // if (transform.position == _agent.Agent.destination)
               // Debug.Log("hey you");
            //go to nearest target after reaching destination clicked with mouse

            // Debug.Log("1x"+Mathf.Abs(transform.position.x - mouseClickPos.x));
            //Debug.Log("2x"+Mathf.Abs(transform.position.z - mouseClickPos.z));
            // if (Vector3.Distance(transform.position, mouseClickPos) > 0.6f)
            //if (mouseClickPos.x == transform.position.x )
            //Debug.Log("pos 1"+ transform.position);
            //Debug.Log("post 2" + mouseClickPos);

            if (walk && transform.position.x != _agent.Agent.destination.x && Mathf.Abs(transform.position.z - _agent.Agent.destination.z)> 0.01f)
            {
                Debug.Log("is Walking ");
                walk = true;
                // Debug.Log
                ///    ("hey baby!!");
                // Debug.Log(Vector3.Distance(transform.position, mouseClickPos));
                // if (target != null)
                //transform.position = Vector2.MoveTowards(transform.position, target.transform.position, walkSpeed * Time.deltaTime);
               
            }

        else 
        {
                
            //Debug.Log("not walking");
            walk = false;

            nearestTower = GetNearestTowerInRange();
            nearestEnemy = GetNearestEnemyInRange();
                
            //choosing nearest target
            if (nearestEnemy != null && nearestTower == null && enemyCastle == null )
            {
                 
                goToEnemy(nearestEnemy);
            }
            else if (nearestEnemy == null && nearestTower != null && enemyCastle == null)
            {

                    goToTower(nearestTower);
            }
            else if (nearestEnemy == null && nearestTower == null && enemyCastle != null)
            {
                goToCastle();
            }
            else if (nearestEnemy != null && nearestTower != null && enemyCastle == null)
            {
                if (Vector2.Distance(transform.position, nearestEnemy.transform.position) < Vector2.Distance(transform.position, nearestTower.transform.position))
                { goToEnemy(nearestEnemy); }
                else
                {
                    goToTower(nearestTower);
                }
            }
            else if (nearestEnemy != null && nearestTower == null && enemyCastle != null)
            {
                if (Vector2.Distance(transform.position, nearestEnemy.transform.position) < Vector2.Distance(transform.position, enemyCastle.transform.position))
                { goToEnemy(nearestEnemy); }
                else
                {
                    goToCastle();
                }
            }
            else if (nearestEnemy == null && nearestTower != null && enemyCastle != null)
            {
                if (Vector2.Distance(transform.position, nearestTower.transform.position) < Vector2.Distance(transform.position, enemyCastle.transform.position))
                { goToTower(nearestTower); }
                else
                {
                    goToCastle();
                }
            }

            else if (nearestEnemy != null && nearestTower != null && enemyCastle != null)
            {
                nearst = minDistance3(nearestTower, nearestEnemy, enemyCastle);
                switch (nearst)
                {
                    case "tower":
                        goToTower(nearestTower);
                        break;
                    case "enemy":
                        goToEnemy(nearestEnemy);
                        break;
                    case "castle":
                        goToCastle();
                        break;
                }
            }
        }
        }
       
    }

    private Tower GetNearestTowerInRange()
    {
        Tower nearestTower = null;
        float smallestDistance = float.PositiveInfinity;
        foreach (Tower tower in _towersInRange)
        {
          
            if (tower.IsDestroyed == false)
            {
                if (Vector2.Distance(transform.localPosition, tower.gameObject.transform.localPosition) < smallestDistance)
                {
                    smallestDistance = Vector2.Distance(transform.localPosition, tower.gameObject.transform.localPosition);
                    nearestTower = tower;
                }
            }
       

        }
        return nearestTower;
    }

 private Enemy GetNearestEnemyInRange()
    {
        Enemy nearestEnemy = null;
        float smallestDistance = float.PositiveInfinity;
        foreach (Enemy enemy in _enemiesInRange)
        {
            if (enemy.IsDead == false)
            {
                if (Vector2.Distance(transform.localPosition, enemy.transform.localPosition) < smallestDistance && !enemy.IsDead)
                {
                    smallestDistance = Vector2.Distance(transform.localPosition, enemy.transform.localPosition);
                    nearestEnemy = enemy;
                }
            }

        }
        return nearestEnemy;
    }

    void OnTriggerEnter(Collider col)
    {


        if (col.gameObject.tag == "Enemy" && col.gameObject.GetComponent<Enemy>().IsDead==false)
        {
            _enemiesInRange.Add(col.gameObject.GetComponent<Enemy>());
        }
        else if (col.gameObject.tag == "EnemyTower" && col.gameObject.GetComponent<Tower>().IsDestroyed==false)
        {
            _towersInRange.Add(col.gameObject.GetComponent<Tower>());
        }
        else if (col.gameObject.tag == "EnemyCastle")
        {
            enemyCastle = col.gameObject.GetComponent<Castle>();
        }else if(col.tag == "Projectile")
        {
            Projectile newP = col.gameObject.GetComponent<Projectile>();
            if (newP != null)
               HeroHit(newP.AttackStrength);
            Destroy(col.gameObject);
        }
    
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            _enemiesInRange.Remove(col.gameObject.GetComponent<Enemy>());
        }
        else if (col.gameObject.tag == "EnemyTower")
        {
            _towersInRange.Remove(col.gameObject.GetComponent<Tower>());
        }
        else if (col.gameObject.tag == "EnemyCastle")
        {
            enemyCastle = null;
        }
     
    }

    void attackEnemy(Enemy enemy)
    {
        if (enemy != null &&  !_isDead && enemy.gameObject.GetComponent<Enemy>().IsDead==false)
        {
            if (attackF > 0)
            {
                attackF = attackF - Time.deltaTime;
            }
            else
            {
                enemy.EnemyHit(damage);
                attackF = attackFrequency;
                if (enemy.GetHealth == 0)
                {
                    _enemiesInRange.Remove(enemy);

                    if (this.gameObject.name == "enemy(Clone)")
                        GameManager.Instance.UnregisterEnemy(enemy);
                    else if (this.gameObject.name == "ally(Clone)")
                    {
                        GameManager.Instance.UnregisterAlly(enemy);
                    }
                }
            }
        }
    }

    void attackTower(Tower tower)
    {
       // Debug.Log("attacking");
        if (tower != null &&  !_isDead && tower.gameObject.GetComponent<Tower>().IsDestroyed==false)
        {
            if (attackF > 0)
            {
                attackF = attackF - Time.deltaTime;
            }
            else
            {
                tower.TowerHit(damage);
                
                attackF = attackFrequency;
                if (tower.GetHealth == 0)
                {
                   tower.IsDestroyed=true;
                    _towersInRange.Remove(tower);
                    TowersManager.Instance.UnregisterTower(tower);
                }
            }
        }
    }

    void attackCastle(Castle castle)
    {
        if (castle != null &&  !_isDead)
        {
            if (attackF > 0)
            {
                attackF = attackF - Time.deltaTime;
            }
            else
            {
                castle.CastleHit(damage);
                attackF = attackFrequency;
            }
        }

    }

    void goToEnemy(Enemy nearestEnemy)
    {
     
        if (nearestEnemy != null && !_isDead)
        {
            _navigationTime += Time.deltaTime;
            if (_navigationTime > _navigationUpdate)

                if (Vector3.Distance(transform.position, nearestEnemy.transform.position) > 1f)
                {
                    Debug.Log(Vector3.Distance(transform.position, nearestEnemy.transform.position));
                    transform.position = Vector3.MoveTowards(transform.position, nearestEnemy.transform.position, _navigationTime);
                }
                else
                {
                    attackEnemy(nearestEnemy);
                }

            _navigationTime = 0;
        }
    }

    void goToTower(Tower nearestTower)
    {


        if (nearestTower != null && !_isDead)
        {

            _navigationTime += Time.deltaTime;
            //Debug.Log(Vector2.Distance(transform.position, nearestTower.transform.position));
            if (_navigationTime > _navigationUpdate)
                if (Vector3.Distance(transform.position, nearestTower.transform.position) > 0.5f)
                {

                    
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(nearestTower.transform.position.x,transform.position.y,nearestTower.transform.position.z), _navigationTime);
                 
                }
              else
                {
                    

                    attackTower(nearestTower);
                }

            _navigationTime = 0;
        }
    }

    void goToCastle()
    {
        if (enemyCastle != null && !_isDead) 
        {
            _navigationTime += Time.deltaTime;
            if (_navigationTime > _navigationUpdate)
                if (Vector2.Distance(transform.position, enemyCastle.transform.position) > 0.5f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, enemyCastle.transform.position, _navigationTime);
                }
                else
                {
                    attackCastle(enemyCastle);
                }

            _navigationTime = 0;
        }
    }

    public void HeroHit(int hitpoints)
    {

        if (hitpoints < _health)
        {
            _health -= hitpoints;
            UpdateHealth();
        }
        else
        {
            healthText.gameObject.SetActive(false);
            _health = 0;

            Die();
        }
    }

    public void Die()
    {

        _spriteRenderer.color = Color.red;
        IsDead = true;
        Destroy(gameObject);
    }
    string minDistance3(Tower tower, Enemy enemy, Castle castle)
    {
        float distTower = Vector2.Distance(transform.position, tower.transform.position);
        float distEnemy = Vector2.Distance(transform.position, enemy.transform.position);
        float distCastle = Vector2.Distance(transform.position, castle.transform.position);

        if ((distTower < distEnemy) && (distTower < distCastle))
        {
            return "tower";
        }
        else if ((distEnemy < distTower) && (distEnemy < distCastle))
        {
            return "enemy";
        }
        else if ((distCastle < distEnemy) && (distCastle < distTower))
        {
            return "castle";
        }

        return "";
    }

    void reachDestination()
    {
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject() && TowersManager._towerBtnPressed == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
            

               
                //mouseClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // hit = Physics2D.Raycast(mouseClickPos, Vector2.zero);
                target = hit.transform.gameObject;
                walk = true;
            }
           
        }
    }

    public void UpdateHealth()
    {
        healthText.text = _health.ToString();
    }
    
    public float GetHealth
    {
        get
        {
            return _health;
        }
    
    }
    public bool IsDead
    {
        get
        {
            return _isDead;
        }
        set
        {
            _isDead = value;
        }
    }
    void LateUpdate()
    {if(nearestTower==null && !IsDead)
        transform.position = Vector3.Lerp(transform.position,new Vector3(follow.position.x,transform.position.y,follow.position.z + zOffset),0.1f);
    }
    public Tower NearestTower
    {
        set
        {
            nearestTower = value;
        }
    }
    
    //navugatio time and walkspeed?
}