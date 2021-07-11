using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    private SphereCollider _collider2D;
    [SerializeField]

    private bool _isDestroyed = false;
    [SerializeField]
    GameObject Hero = null;

    [SerializeField]
    private List<Enemy> _enemiesInRange = new List<Enemy>();

    float _attackRadius;
    SpriteRenderer _spriteRenderer;

    [SerializeField]
    float _health;
    public Text healthText;
    [SerializeField]
    private float _timeBetweenAttacks;

    [SerializeField]
    private Projectile _projectile;
    [SerializeField]

    private GameObject _targetEnemy = null;
    private float _attackCounter;
    private bool _isAttacking = false;

    [SerializeField]
    private Sprite _dragSprite;
    [SerializeField]
    private int _sellPrice;
    private Projectile _newProjectile;

    private bool _isClicked;

    [SerializeField]
    private Sprite _upgradeSprite;

    [SerializeField]
    private int _upgradeCost;
    [SerializeField]
    private int _upgradeHealthAmount;
    [SerializeField]
    private int _upgradeDammageAmount;

    [SerializeField]
    private GameObject _upgradeBtn;
    private int _dammage;

    // Use this for initialization
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
    }
    void Start()
    {
        _isClicked = false;
        UpdateHealth();
        _collider2D = GetComponent<SphereCollider>();
        _dammage = _projectile.AttackStrength;
      
    }
    //its better local position when you move a game object to another object
    // Update is called once per frame
    void Update()
    {

        if(GameManager.Instance.currentState == GameStatus.duringWave)
        if (!this.IsDestroyed)
        {
            _attackCounter -= Time.deltaTime;

            if (_targetEnemy == null || (_targetEnemy.tag == "Enemy" && _targetEnemy.GetComponent<Enemy>().IsDead) || (_targetEnemy.tag == "Hero" && _targetEnemy.GetComponent<HeroControls>().IsDead))
            {
                if (_newProjectile != null)
                    Destroy(_newProjectile.gameObject);
                Enemy nearestEnemy = GetNearestEnemyInRange();

                if (nearestEnemy == null && Hero != null)
                {
                    _targetEnemy = Hero;
                }
                else if (nearestEnemy != null && Hero == null)
                {

                    _targetEnemy = nearestEnemy.gameObject;

                }
                else if (nearestEnemy != null && Hero != null)
                {
                    if (Vector3.Distance(this.transform.position, nearestEnemy.transform.position) < Vector3.Distance(this.transform.position, Hero.transform.position))
                    {
                        _targetEnemy = nearestEnemy.gameObject;
                    }
                    else
                    {
                        _targetEnemy = Hero;
                    }
                }
            }
            else
            {
                if (_attackCounter <= 0)
                {
                    _isAttacking = true;
                    //reset Attack Counter
                    _attackCounter = _timeBetweenAttacks;
                }
                else
                {
                    _isAttacking = false;
                }
            }

        }
    }




    //for moving gameobjects around
    void FixedUpdate()
    {
        
        if (_targetEnemy != null &&((_targetEnemy.gameObject.tag == "Enemy" && !_targetEnemy.GetComponent<Enemy>().IsDead) || (_targetEnemy.gameObject.tag == "Hero" && !_targetEnemy.GetComponent<HeroControls>().IsDead)  || (_targetEnemy.gameObject.tag == "Ally" && !_targetEnemy.GetComponent<Enemy>().IsDead)))
        {
            if (_isAttacking)
                Attack();
        }
        else
        {
            
            _targetEnemy = null;
        }
    }
    public void Attack()
    {

        Debug.Log("attack");
        if ((_targetEnemy.gameObject.tag == "Hero" && _targetEnemy.gameObject.GetComponent<HeroControls>().IsDead == false) || (_targetEnemy.gameObject.tag == "Enemy" && _targetEnemy.gameObject.GetComponent<Enemy>().IsDead == false) || (_targetEnemy.tag == "Ally" && _targetEnemy.GetComponent<Enemy>().IsDead == false))
        {
          ;

            _isAttacking = false;
            _newProjectile = Instantiate(_projectile) as Projectile;
         
            _newProjectile.transform.localPosition = transform.position;
            _newProjectile.AttackStrength = _dammage;
            
            if (_targetEnemy == null)
            {

                Destroy(_newProjectile.gameObject);
            }
            else
            {
                //move projectile to enemy
               StartCoroutine(MoveProjectile(_newProjectile));
            }
        }


    }
    IEnumerator MoveProjectile(Projectile projectile)
    {

        if (_targetEnemy.tag == "Hero")
        {
            
            while (getTargetDistance(_targetEnemy) > 0.01f && projectile != null && _targetEnemy != null)
            {
                //projectile.transform.Rotate(90, 0, 0);
                var dir = _targetEnemy.transform.position - transform.position;

                //get the angle in radiant and transform to degrees 
                var angleDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                
                //projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.up);
                //projectile.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
                projectile.transform.rotation = Quaternion.Euler(new Vector3(90, 0, angleDirection));
                projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, _targetEnemy.transform.position, 5f * Time.deltaTime);
                //projectile.transform.Rotate(90, angleDirection, 0);
                
            
                yield return null;
            }
        }
        else
        {
            while (getTargetDistance(_targetEnemy) > 0.01f && projectile != null && _targetEnemy != null && !_targetEnemy.GetComponent<Enemy>().IsDead)
            {
                var dir = _targetEnemy.transform.localPosition - transform.localPosition;

                //get the angle in radiant and transform to degrees 
                var angleDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                //projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
                projectile.transform.rotation = Quaternion.Euler(new Vector3(90, 0, angleDirection));
                projectile.transform.localPosition = Vector3.MoveTowards(projectile.transform.localPosition, _targetEnemy.transform.localPosition, 5f * Time.deltaTime);
                yield return null;
            }
        }


        if (projectile != null || _targetEnemy == null)
        {
            if (projectile != null)
                Destroy(projectile.gameObject);
        }
    }
    private float getTargetDistance(GameObject thisEnemy)
    {
        if (thisEnemy == null)
        {
            Enemy nearestEnemy = GetNearestEnemyInRange();
            if (nearestEnemy != null)
            {
                thisEnemy = nearestEnemy.gameObject;
                if (thisEnemy == null)
                {
                    return 0f;
                }
            }
        }
        if (thisEnemy != null)
            return Mathf.Abs(Vector3.Distance(transform.localPosition, thisEnemy.transform.localPosition));
        else
            return 0f;
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
                
                if (Vector3.Distance(transform.localPosition, enemy.transform.localPosition) < smallestDistance )
                {
                    smallestDistance = Vector3.Distance(transform.localPosition, enemy.transform.localPosition);
                    nearestEnemy = enemy;
                 
                }
            }

        }
        return nearestEnemy;
    }


    public bool IsClicked
    {
        get
        {
            return _isClicked;
        }
        set
        {
            _isClicked = value;
        }
    }
    public Sprite DragSprite
    {
        get
        {
            return _dragSprite;
        }
    }
    public Projectile Projectile
    {
        get
        {
            return _newProjectile;
        }
    }

    public int SellPrice
    {
        get
        {
            return _sellPrice;
        }
        set
        {
            _sellPrice = value;
        }
    }

    public void UpdateHealth()
    {
        healthText.text = _health.ToString();
    }

    public void TowerHit(int hitpoints)
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
        _collider2D.enabled = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (this.tag == "Tower")
        {
            if (col.gameObject.tag == "Enemy")
            {
                _enemiesInRange.Add(col.gameObject.GetComponent<Enemy>());

            }
        }
        else if (this.tag == "EnemyTower")
        {
            if (col.gameObject.tag == "Ally")
            {
                _enemiesInRange.Add(col.gameObject.GetComponent<Enemy>());
            }
            else if (col.tag == "Hero")
            {
                Hero = col.gameObject;
            }
        }

    }

    void OnTriggerExit(Collider col)
    {
        if (this.tag == "Tower")
        {
            if (col.gameObject.tag == "Enemy")
            {
                _enemiesInRange.Remove(col.gameObject.GetComponent<Enemy>());
            }
        }
        else if (this.tag == "EnemyTower")
        {
            if (col.gameObject.tag == "Ally")
            {
                _enemiesInRange.Remove(col.gameObject.GetComponent<Enemy>());
            }
            else if (col.tag == "Hero")
            {
                Hero = null;
            }
        }
        _targetEnemy = null;
        if (_newProjectile != null)
            Destroy(_newProjectile.gameObject);
    }

    public float GetHealth
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
        }
    }
    public bool IsDestroyed
    {
        get
        {
            return _isDestroyed;
        }
        set
        {
            _isDestroyed = value;
        }
    }
    public Sprite UpgradeSprite{
        get{
            return _upgradeSprite;
        }
    
    }
    public int UpgradeHealth
    {
        get
        {
            return _upgradeHealthAmount;
        }
    }
    public int UpgradeDammage
    {
        get
        {
            return _upgradeDammageAmount;
        }
    }
    public int UpgradeCost
    {
        get
        {
            return _upgradeCost;
        }
    }

    public GameObject UpgradeBtn
    {
        get
        {
            return _upgradeBtn;
        }
    }
    public int Dammage
    {
        set
        {
           _dammage = value;
       
        }
    }

    
    public void UpgradeTower()
    {
        if (GameManager.Instance.TotalMoney > UpgradeCost)
        {


            GameManager.Instance.SubtractMoney(UpgradeCost);
            //Destroy(_previouslyClickedTower.gameObject);
            GetComponent<SpriteRenderer>().sprite = UpgradeSprite;
            UpgradeBtn.SetActive(false);
            Dammage = UpgradeDammage;
            GetHealth = UpgradeHealth;
            SellPrice += UpgradeCost / 2;
            UpdateHealth();
        }
    }
}

