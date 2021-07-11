using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    float _health;
    public Text healthText;
    

    [SerializeField]
    int damage = 3;

    [SerializeField]
    float attackFrequency;
    float attackF = 0;
    AgentEnemyScript _agentEnemyScript;
   

    [SerializeField]
    private int _rewardAmount;

    [SerializeField]
    private float _navigationUpdate;

    private int _target = 0;
    private Transform _enemy;
    //keep tracks of time
    private float _navigationTime = 0;
    private Collider _enemyCollider;
    [SerializeField]

    private bool _isDead = false;
    SpriteRenderer _spriteRenderer;

    [SerializeField]
    private List<Tower> _towersInRange = new List<Tower>();

    [SerializeField]
    private List<Enemy> _enemiesInRange = new List<Enemy>();

    [SerializeField]
    Tower nearestTower;

    [SerializeField]
    Enemy nearestEnemy;

    [SerializeField]
    Castle castle = null;
    [SerializeField]
    GameObject Hero = null;
    string nearst;
    private NavMeshAgent _agent;
    // Use this for initialization
    private void Awake()
    {
        _enemy = GetComponent<Transform>();
        _enemyCollider = GetComponent<Collider>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _agent = GetComponent<NavMeshAgent>();
        _agentEnemyScript = GetComponent<AgentEnemyScript>();
    }

    void Start()
    {
        UpdateHealth();
        if (this.gameObject.tag=="Enemy")
            GameManager.Instance.RegisterEnemy(this);
        else if (this.gameObject.tag == "Ally")
        {

            GameManager.Instance.RegisterAlly(this);
        }
    }

    // Update is called once per frame
    void Update()
    {

        nearestEnemy = GetNearestEnemyInRange();
       nearestTower = GetNearestTowerInRange();

        if (Hero == null )
        {
            if (nearestEnemy != null && nearestTower == null && castle == null)
            {
                goToEnemy(nearestEnemy);
            }
            else if (nearestEnemy == null && nearestTower != null && castle == null)
            {
                
                goToTower(nearestTower);
            }
            else if (nearestEnemy == null && nearestTower == null  && !IsDead && _agent != null && _agent.isStopped)
            {
                _agent.isStopped = false;
                //goToCastle();
            }
            else if (nearestEnemy != null && nearestTower != null && castle == null)
            {
                if (Vector2.Distance(transform.position, nearestEnemy.transform.position) < Vector2.Distance(transform.position, nearestTower.transform.position))
                {
                    goToEnemy(nearestEnemy);
                }
                else
                {
                    goToTower(nearestTower);
                }
            }
            else if (nearestEnemy != null && nearestTower == null && castle != null)
            {
                if (Vector2.Distance(transform.position, nearestEnemy.transform.position) < Vector2.Distance(transform.position, castle.transform.position))
                {
                    goToEnemy(nearestEnemy);
                }
                else
                {
                    goToCastle();
                }
            }
            else if (nearestEnemy == null && nearestTower != null && castle != null)
            {
                if (Vector2.Distance(transform.position, nearestTower.transform.position) < Vector2.Distance(transform.position, castle.transform.position))
                {
                    goToTower(nearestTower);
                }
                else
                {
                    goToCastle();
                }
            }
            else if (nearestEnemy != null && nearestTower != null && castle != null)
            {
                nearst = minDistance3(nearestTower, nearestEnemy, castle);
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

           /* else if (nearestEnemy == null && nearestTower == null && castle == null)
            {
                Debug.Log(_target);
                if (_wayPoints != null && !_isDead)
                {
                    _navigationTime += Time.deltaTime;
                    if (_navigationTime > _navigationUpdate)

                        if (_target < _wayPoints.Length)
                        {//the third parameter navigationTime is the time it will take to go the new waypoint
                            _enemy.position = Vector2.MoveTowards(_enemy.position, _wayPoints[_target].position, _navigationTime);
                        }
                        else
                        {
                            _enemy.position = Vector2.MoveTowards(_enemy.position, _exitPoint.position, _navigationTime);
                        }
                    _navigationTime = 0;
                }
            }*/
        }
        else

        {

            if (!_isDead && Hero.gameObject.GetComponent<HeroControls>().IsDead == false)
                if (_agent.isStopped)
                {
                    goToHero();
                    _agent.isStopped = true;
                }
                else 
                {

                    //_agent.isStopped = false;
                    Hero = null;
                }
          /*  if (!IsDead && Hero.gameObject.GetComponent<HeroControls>().IsDead)
                _agent.isStopped = false;*/

        }
    }
    void OnTriggerEnter(Collider other)
    {

  

        if (this.tag == "Ally")
        {
            if (other.gameObject.tag == "Enemy" && other.gameObject.GetComponent<Enemy>().IsDead == false)
            {
                _agent.isStopped = true;
                _enemiesInRange.Add(other.gameObject.GetComponent<Enemy>());
            }
            else if (other.gameObject.tag == "EnemyTower" && other.gameObject.GetComponent<Tower>().IsDestroyed == false)
            {
                _agent.isStopped = true;
                _towersInRange.Add(other.gameObject.GetComponent<Tower>());
            }
            else if (other.gameObject.tag == "EnemyCastle")
            {
               
                castle = other.gameObject.GetComponent<Castle>();
            }
        }
        else if (this.tag == "Enemy")
        {
            
            if (other.gameObject.tag == "Ally" && other.gameObject.GetComponent<Enemy>().IsDead == false)
            {
                _agent.isStopped = true;
                _enemiesInRange.Add(other.gameObject.GetComponent<Enemy>());
            }
            else if (other.gameObject.GetComponent<Tower>() != null && other.gameObject.tag == "Tower" && other.gameObject.GetComponent<Tower>().IsDestroyed == false)
            {
                _agent.isStopped = true;
                _towersInRange.Add(other.gameObject.GetComponent<Tower>());
            }
            else if (other.gameObject.tag == "AllyCastle")
            {
                castle = other.gameObject.GetComponent<Castle>();
            }
            else if (other.tag == "Hero")
            {
               
                _agent.isStopped = true;
                Hero = other.gameObject;
            }
        }
    }


    void OnTriggerExit(Collider other)
    {

        if (this.tag == "Ally")
        {
            if (other.gameObject.tag == "Enemy")
            {
                _enemiesInRange.Remove(other.gameObject.GetComponent<Enemy>());
            }
            else if (other.gameObject.tag == "EnemyTower")
            {
                _towersInRange.Remove(other.gameObject.GetComponent<Tower>());
            }
            else if (other.gameObject.tag == "EnemyCastle")
            {
                castle = null;
            }
        }
        else if (this.tag == "Enemy")
        {

            if (other.gameObject.tag == "Ally")
            {
                _enemiesInRange.Remove(other.gameObject.GetComponent<Enemy>());
            }
            else if (other.gameObject.tag == "Tower")
            {
                _towersInRange.Remove(other.gameObject.GetComponent<Tower>());
            }
            else if (other.gameObject.tag == "AllyCastle")
            {
                castle = null;
            }
            else if (other.tag == "Hero")
            {
                Hero = null;
            }
        }
    }


    public void EnemyHit(int hitpoints)
    {

        if (hitpoints < _health)
        {
            //play hurt animation here 
            _health -= hitpoints;
            UpdateHealth();
        }
        else
        {
            if (!_isDead)
            {
                _health = 0;
                healthText.gameObject.SetActive(false);
                Die();
            }

        }
    }

    public void Die()
    {

        //play die animation here
        _agent.enabled = false;
        _agentEnemyScript.enabled = false;
        transform.rotation = Quaternion.Euler(90, 0, 0);
        _isDead = true;
        _spriteRenderer.color = Color.red;
        _enemyCollider.enabled = false;
        transform.Find("Body").GetComponent<Collider>().enabled = false;
        if (this.gameObject.tag == "Enemy")
        {
            GameManager.Instance.TotalKilled += 1;
            GameManager.Instance.AddMoney(_rewardAmount);
        }
   
        else if(this.gameObject.tag=="Ally")
        GameManager.Instance.TotalKilledAllies += 1;
     
        GameManager.Instance.IsWaveOver();

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
    }

    public void UpdateHealth()
    {
        healthText.text = _health.ToString();
    }

    private Tower GetNearestTowerInRange()
    {
        Tower nearestTower = null;
        float smallestDistance = float.PositiveInfinity;
        foreach (Tower tower in _towersInRange)
        {if(tower != null)
            if (tower.IsDestroyed == false)
            {
                if (Vector2.Distance(transform.localPosition, tower.gameObject.transform.localPosition) < smallestDistance)
                {
                    smallestDistance = Vector2.Distance(transform.localPosition, tower.gameObject.transform.localPosition);
                    nearestTower = tower;
                }
            }
            else if (tower.IsDestroyed == true)
            {
             
                _towersInRange.Remove(tower);
                break;
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
            if(enemy != null)
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

    void goToEnemy(Enemy nearestEnemy)
    {
        if (nearestEnemy != null && !_isDead)
        {
            _navigationTime += Time.deltaTime;
            if (_navigationTime > _navigationUpdate)

                if (Vector2.Distance(transform.position, nearestEnemy.transform.position) > 0.8f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, nearestEnemy.transform.localPosition, _navigationTime);
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
            if (_navigationTime > _navigationUpdate)
                if (Vector3.Distance(transform.position, nearestTower.transform.position) > 0.8f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, nearestTower.transform.position, _navigationTime);
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
        if (castle != null && !_isDead)
        {
            _navigationTime += Time.deltaTime;
            if (_navigationTime > _navigationUpdate)
                if (Vector2.Distance(transform.position, castle.transform.position) > 0.8f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, castle.transform.position, _navigationTime);
                }
                else
                {
                    attackCastle(castle);
                }

            _navigationTime = 0;
        }
    }

    void goToHero()
    {
       
        
            _navigationTime += Time.deltaTime;
            if (_navigationTime > _navigationUpdate)
                if (Vector3.Distance(transform.position, Hero.transform.position) > 0.8f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, Hero.transform.position, _navigationTime);
                }
                else
                {
                    attackHero(Hero);
                }

            _navigationTime = 0;
        
    }
    void attackEnemy(Enemy enemy)
    {
        if (enemy != null && !_isDead && enemy.gameObject.GetComponent<Enemy>().IsDead==false)
        {
            if (attackF > 0)
            {
                attackF = attackF - Time.deltaTime;
            }
            else
            {
                enemy.EnemyHit(damage);
                attackF = attackFrequency;
                if (enemy.GetHealth <= 0)
                {
                    _enemiesInRange.Remove(enemy);
                    if (this.gameObject.name == "Enemy")
                        GameManager.Instance.UnregisterEnemy(enemy);
                    else if (this.gameObject.tag=="Ally")
                    {
                        GameManager.Instance.UnregisterAlly(enemy);
                    }
                }
            }
        }
    }

    void attackTower(Tower tower)
    {
        if (tower != null && !_isDead && tower.gameObject.GetComponent<Tower>().IsDestroyed==false)
        {
            if (attackF > 0)
            {
                attackF = attackF - Time.deltaTime;
            }
            else
            {
                tower.TowerHit(damage);
                attackF = attackFrequency;
                if (tower.GetHealth <= 0)
                {
                    tower.IsDestroyed = true;
                    _towersInRange.Remove(tower);
                
                    TowersManager.Instance.UnregisterTower(tower);
                }
            }
        }
    }

    void attackCastle(Castle castle)
    {
        if (castle != null && !_isDead)
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
    void attackHero(GameObject Hero)
    {
   
        if (Hero != null && !_isDead && Hero.gameObject.GetComponent<HeroControls>().IsDead==false)
        {
            if (attackF > 0)
            {
                attackF = attackF - Time.deltaTime;
            }
            else
            {
                Hero.GetComponent<HeroControls>().HeroHit(damage);
                attackF = attackFrequency;
            }
        }

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

    public Tower NearestTower
    {
        get
        {
            return nearestTower;
        }
    }

    public Enemy NearestEnemy
    {
        get
        {
            return nearestEnemy;
        }
    }

    public Castle Castle
    {
        get
        {
            return castle;
        }
    }
}
