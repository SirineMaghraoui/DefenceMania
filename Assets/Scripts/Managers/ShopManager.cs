using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : Singleton<ShopManager> {
    [SerializeField]
    private GameObject _minionPanel;
    [SerializeField]
    private GameObject _buyPanel;
    [SerializeField]
    private int _smallMinionPrice;
    [SerializeField]
    private int _bigMinionPrice;
    [SerializeField]
    private Text _howManyToBuyText;
    [SerializeField]
    private Text _costText;
    [SerializeField]
    private Slider _howManyToBuySlider;
    private int _howManyToBuy;
    [SerializeField]
    AllySpawner _spawnAllies;
    [SerializeField]
    Image _buyMinionImage;
    const float spawnDelay = 0.5f;
    private int _whichEnemyToSpawn;
    private int x;
    private int _enemyUnitPrice = 3;
    // Use this for initialization
    void Start() {
        _minionPanel.SetActive(true);
        _howManyToBuySlider.maxValue = GameManager.Instance.TotalMoney / _enemyUnitPrice;
    }

    // Update is called once per frame
    void Update() {
        if (BuyPanel.activeSelf)
        {
            _howManyToBuy = (int)_howManyToBuySlider.value;
            _howManyToBuyText.text = _howManyToBuy.ToString();
            _costText.text = "Cost: " + _enemyUnitPrice * _howManyToBuy;
            _howManyToBuySlider.maxValue = GameManager.Instance.TotalMoney / _enemyUnitPrice;
        }
            
    }
    public GameObject BuyPanel
    {
        get
        {
            return _buyPanel;
        }
        set
        {
            _buyPanel = value;
        }
    }
    public GameObject MinionPanel
    {

        get
        {
            return _minionPanel; 
        }
    }

    public void BuyMinion(GameObject obj)
    {
        if(obj.name == "SmallMinionParam")
        {
            _enemyUnitPrice = _smallMinionPrice;
            _howManyToBuySlider.maxValue = GameManager.Instance.TotalMoney / _enemyUnitPrice;
        }
        else
        {
          
            _enemyUnitPrice = _bigMinionPrice;
            _howManyToBuySlider.maxValue = GameManager.Instance.TotalMoney / _enemyUnitPrice;
        }
        //GameManager.Instance.SubtractMoney(SmallMinionPrice);
        _buyMinionImage.sprite = obj.GetComponent<Extra>().sprite;
        _whichEnemyToSpawn = obj.GetComponent<Extra>().whichEnemyToSpawn;
        _buyPanel.SetActive(true);
        //_whichEnemyToSpawn = whichenemy;
        //Debug.Log("buy a small minion" + SmallMinionPrice);
    }

    public void BuySubmit()
    {
      
        if (_whichEnemyToSpawn == 0)
           x = SmallMinionPrice * _howManyToBuy;
        else if (_whichEnemyToSpawn == 1)
            x = _bigMinionPrice * _howManyToBuy;
        if (GameManager.Instance.TotalMoney >= x)
        {
            GameManager.Instance.SubtractMoney(x);
            _spawnAllies.TotalAllies = _howManyToBuy;
            _spawnAllies.WhichEnemyToSpawn = _whichEnemyToSpawn;
            _buyPanel.SetActive(false);
            _minionPanel.SetActive(false);
        }
    }
    public int SmallMinionPrice{
        get
        {
            return _smallMinionPrice;
        }
    }

    public IEnumerator Spawn()
    {
      
        _spawnAllies.SpawnAlly();
        yield return new WaitForSeconds(spawnDelay);
        if (_spawnAllies.CountAllies >= _spawnAllies.TotalAllies) 
            StopCoroutine(Spawn());
        else
            StartCoroutine(Spawn());

    }


}

