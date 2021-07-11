
using UnityEngine;

public class AllySpawner : MonoBehaviour
{

 
    
    [SerializeField]
    private GameObject[] _allies;
    private  int _countAllies;
    [SerializeField]    
    private  int _totalAllies;
    private int _whichEnemyToSpawn;
    [SerializeField]
    private int _alliePerSpawn;
    [SerializeField]
    private GameObject _spawnPoint;

    public int CountAllies
    {
        get
        {
            return _countAllies;
        }
        set
        {
            _countAllies = value;
        }
    }

    public int TotalAllies
    {
        get
        {
            return _totalAllies;
        }
        set
        {
            _totalAllies = value;
        }
    }

    public int WhichEnemyToSpawn
    {
        get
        {
            return _whichEnemyToSpawn;
        }
        set
        {
            _whichEnemyToSpawn = value;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      
    }
    public  void SpawnAlly()
    {
        //Debug.Log("SpawnAlly");
        int _managerAllyCount = GameManager.Instance.allyList.Count;

        //Debug.Log(_managerAllyCount);
        if (_alliePerSpawn > 0 && _managerAllyCount < _totalAllies)
        {

            for (int i = 0; i < _alliePerSpawn; i++)
            {

                if (_managerAllyCount < _totalAllies)
                {
                    Instantiate(_allies[_whichEnemyToSpawn], _spawnPoint.transform.position, Quaternion.identity);
                    _countAllies++;
                }
            }
        }
    }



}
