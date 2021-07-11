using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum GameStatus
{
    next, play, gameover, win, duringWave, beforeStart
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private Text _timeRemainingText;
    [SerializeField]
    private int _timeBetweenWaves;
    [SerializeField]
    private int _totalwaves;
    [SerializeField]
    private Text _totalMoneyLbl;
    [SerializeField]
    private Text _currentWaveLbl;
    [SerializeField]
    private Text _totalEscapedLbl;
    [SerializeField]
    private Text _totalEscapedAlliesLbl;
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private GameObject[] _enemies;
    [SerializeField]
    private int _initialEnemyNumber;
    [SerializeField]
    private int _totalEnemies;
    [SerializeField]
    private int _enemiesPerSpawn;
    [SerializeField]
    private Button _playBtn;
    [SerializeField]
    private Text _playBtnLbl;
    [SerializeField]
    AllySpawner _spawnAllies;

    public List<Enemy> enemyList = new List<Enemy>();
    public List<Enemy> allyList = new List<Enemy>();

    private int _countEnemies = 0;
    const float spawnDelay = 0.5f;

    private int _waveNumber = 0;
    [SerializeField]
    private int _totalMoney;
    private int _totalEscaped = 0;
    [SerializeField]
    private int _totalEscapedAllies;
    private int _roundEscaped = 0;
    private int _roundEscapedAllies= 0;
    private int _totalKilled = 0;
    private int _totalKilledAllies = 0;
    private int _whichEnemiesToSpawn = 0;
  

    public GameStatus currentState = GameStatus.beforeStart;

    public int TotalEscapedAllies
    {
        get
        {
            return _totalEscapedAllies;
        }

        set
        {
            _totalEscapedAllies = value;
        }
    }
    public int RoundEscapedAllies
    {
        get
        {
            return _roundEscapedAllies;
        }
        set
        {
            _roundEscapedAllies = value;
        }
    }
    public int TotalKilledAllies
    {
        get
        {
            return _totalKilledAllies;
        }
        set
        {
            _totalKilledAllies = value;
        }
    }
    public int TotalEscaped
    {
        get
        {
            return _totalEscaped;
        }
        set
        {
            _totalEscaped = value;
        }
    }

    public int RoundEscaped
    {
        get
        {
            return _roundEscaped;
        }
        set
        {
            _roundEscaped = value;
        }
    }
    public int TotalKilled
    {
        get
        {
            return _totalKilled;
        }
        set
        {
            _totalKilled = value;
        }
    }


    public int TotalMoney
    {
        get
        {
            return _totalMoney;
        }
        set
        {
            _totalMoney = value;
            _totalMoneyLbl.text = _totalMoney.ToString();
        }
    }
    public int TotalEnemies
    {
        get
        {
            return _totalEnemies;
        }
        set
        {
            _totalEnemies = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        _totalMoneyLbl.text = _totalMoney.ToString();
        _playBtn.gameObject.SetActive(false);
        ShowMenu();

    }

    void Update()
    {

        HandleEscape();

    }


    void SpawnEnemy()
    {
        if (_enemiesPerSpawn > 0 && enemyList.Count < _totalEnemies)
        {

            for (int i = 0; i < _enemiesPerSpawn; i++)
            {
                
                if (enemyList.Count < _totalEnemies)
                {
                
                    GameObject enemy = Instantiate(_enemies[0],   spawnPoint.transform.position, Quaternion.Euler(90,0,0));
                    _countEnemies++;
                }
            }
        }
    }
    public void RegisterEnemy(Enemy enemy)
    {
        enemyList.Add(enemy);
    }
    public void RegisterAlly(Enemy ally)
    {
        allyList.Add(ally);
    }
    public void UnregisterEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
        //Destroy(enemy.gameObject);

    }

    public void UnregisterAlly(Enemy ally)
    {
        allyList.Remove(ally);
        //Destroy(ally.gameObject);

    }


    public void DestroyAllEnemies()
    {
        foreach (Enemy enemy in enemyList)
        {
            if(enemy != null)
            Destroy(enemy.gameObject);
        }
        enemyList.Clear();
    }

    public void DestroyAllAllies()
    {

        foreach (Enemy ally in allyList)
        {
          if(ally != null)
            Destroy(ally.gameObject);
        }
        allyList.Clear();
    }


    IEnumerator Spawn()
    {
        SpawnEnemy();
        yield return new WaitForSeconds(spawnDelay);
        if (_countEnemies >= _totalEnemies)
            StopCoroutine(Spawn());
        else
            StartCoroutine(Spawn());

    }
    public void AddMoney(int amount)
    {
        TotalMoney += amount;
    }
    public void SubtractMoney(int amount)
    {
        TotalMoney -= amount;
    }

    public void IsWaveOver()
    {
        _totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
        _totalEscapedAlliesLbl.text = "Escaped " + TotalEscapedAllies + "/10";



        if ((RoundEscaped + TotalKilled) == TotalEnemies && (RoundEscapedAllies+TotalKilledAllies)==_spawnAllies.TotalAllies)
        {
        
            SetCurrentGameState();
            ShowMenu();
            RoundEscapedAllies = 0;
            RoundEscaped = 0;
            TotalKilled = 0;
            TotalKilledAllies = 0;
            _countEnemies = 0;
            //TotalEnemies = 0;
            _spawnAllies.TotalAllies = 0;
            _spawnAllies.CountAllies = 0;
            
        }
    }

    public void SetCurrentGameStateToPlaying()
    {
        currentState = GameStatus.duringWave;
    }

    public void SetCurrentGameState()
    {
      
        if (TotalEscaped >= 10)
        {
            currentState = GameStatus.gameover;
        }
        else if(TotalEscapedAllies >= 10)
        {

            currentState = GameStatus.win;
        }
        /*
        else if (_waveNumber == 0 && (TotalKilled + RoundEscaped) == 0)
        {
            currentState = GameStatus.play;
        }*/
        else if (_waveNumber >= _totalwaves - 1)
        {
            currentState = GameStatus.win;
        }
        else
        {
            currentState = GameStatus.next;
            StartCoroutine(StartTime());
        }
    }

    public void ShowMenu()
    {
        switch (currentState)
        {
            case GameStatus.beforeStart:
                _playBtnLbl.text = "Start the Game";
               
                break;
            case GameStatus.gameover:
                _playBtnLbl.text = "you lost";
                
                break;
            case GameStatus.next:
                _playBtnLbl.text = "Next Wave";
                ShopManager.Instance.MinionPanel.SetActive(true);
                break;
            case GameStatus.play:
                _playBtnLbl.text = "Play";
                break;
            case GameStatus.win:
                _playBtnLbl.text = "You won";
                break;
        }
        _playBtn.gameObject.SetActive(true);

    }

    public void PlayButtonPressed()
    {

        switch (currentState)
        {
            case GameStatus.next:
                ShopManager.Instance.MinionPanel.SetActive(false);
                ShopManager.Instance.BuyPanel.SetActive(false);

                _waveNumber++;
                
                _totalEnemies +=   _waveNumber * 2;
                DestroyAllEnemies();
                DestroyAllAllies();
                StartCoroutine(Spawn());

                StartCoroutine(ShopManager.Instance.Spawn());
                currentState = GameStatus.duringWave;
                _playBtn.gameObject.SetActive(false);
                _timeRemainingText.gameObject.SetActive(false);
                break;
            case GameStatus.beforeStart:
                //ShopManager.Instance.MinionPanel.SetActive(false);
                //ShopManager.Instance.BuyPanel.SetActive(false);
                _waveNumber = 0;
                TotalEscaped = 0;
                TotalMoney = _totalMoney;

                _totalMoneyLbl.text = TotalMoney.ToString();
                _totalEscapedLbl.text = "Escaped" + TotalEscaped + "/10";

                DestroyAllEnemies();
                DestroyAllAllies();
                //we only care about these variables per a wave
                TotalKilled = 0;
                RoundEscaped = 0;
                _countEnemies = 0;
                _totalEnemies = _initialEnemyNumber;
               // StartCoroutine(Spawn());
               // StartCoroutine(ShopManager.Instance.Spawn());
                currentState = GameStatus.next;
                StartCoroutine(StartTime());
                _playBtnLbl.text = "Start the wave";
                _waveNumber = -1;
                //_playBtn.gameObject.SetActive(false);

                break;
            case GameStatus.gameover:

                _waveNumber = 0;
                TotalEscaped = 0;
                TotalMoney = _totalMoney;
                TowersManager.Instance.DestroyAllTower();
                TowersManager.Instance.RenameTagsBuildSites();
                _totalMoneyLbl.text = TotalMoney.ToString();
                _totalEscapedLbl.text = "Escaped" + TotalEscaped + "/10";
                currentState = GameStatus.beforeStart;
                DestroyAllEnemies();
                DestroyAllAllies();
                _playBtnLbl.text = "Start The Game";
                _playBtn.gameObject.SetActive(true);
                ShopManager.Instance.MinionPanel.SetActive(true);
                //////
                TotalKilled = 0;
                RoundEscaped = 0;
                _countEnemies = 0;
                break;
            case GameStatus.win:
                _waveNumber = 0;
                TotalEscaped = 0;
                TotalMoney = _totalMoney;
                TowersManager.Instance.DestroyAllTower();
                TowersManager.Instance.RenameTagsBuildSites();
                _totalMoneyLbl.text = TotalMoney.ToString();
                _totalEscapedLbl.text = "Escaped" + TotalEscaped + "/10";
                currentState = GameStatus.beforeStart;
                DestroyAllEnemies();
                DestroyAllAllies();
                _playBtnLbl.text = "Start The Game";
                _playBtn.gameObject.SetActive(true);
                ShopManager.Instance.MinionPanel.SetActive(true);
                break;
            default:

                _waveNumber = 0;
                TotalEscaped = 0;
                TotalMoney = _totalMoney;
                TowersManager.Instance.DestroyAllTower();
                TowersManager.Instance.RenameTagsBuildSites();
                _totalMoneyLbl.text = TotalMoney.ToString();
                _totalEscapedLbl.text = "Escaped" + TotalEscaped + "/10";
                currentState = GameStatus.duringWave;
                break;

        }

        _currentWaveLbl.text = "Wave " + (_waveNumber + 1);
        //start spawning enemies
        //StartCoroutine(Spawn());
        //hide the button once the game started
        //

    }

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TowersManager.Instance.DisableDragSprite();
            TowersManager._towerBtnPressed = null;
        }
    }

       IEnumerator  StartTime()
    {
        
        _playBtn.gameObject.SetActive(true);

        int counter = _timeBetweenWaves;
        _timeRemainingText.gameObject.SetActive(true);
        while (counter > 0)
        {
            _timeRemainingText.text = "Time remaining before the wave starts: " + counter;
            
            yield return new WaitForSeconds(1);
            counter--;
                _timeRemainingText.text = "Time remaining before the wave starts: " + counter ;
            }
        if (_playBtn.gameObject.activeSelf)
        {

            _playBtn.onClick.Invoke();
            _playBtn.gameObject.SetActive(false);
            _timeRemainingText.gameObject.SetActive(false);
            TowersManager.Instance.Hide();
        }
        
        //_timeRemaining -= Time.deltaTime;

    }
      
}
