using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class TowersManager : Singleton<TowersManager>
{

    public static TowerBtn _towerBtnPressed { get; set; }
    private SpriteRenderer _spriteRenderer;
    public List<Tower> _towerList = new List<Tower>();
    public List<Tower> _towerListDestroyed = new List<Tower>();
    [SerializeField]
    private List<Collider> _buildList = new List<Collider>();
    private Collider _buildTile;
    private bool _movingTower = false;
    private Tower _previouslyClickedTower = null;
    private Tower _movingTowerObject;

    string nameOfLayer = "battle1";
    LayerMask layer;


    // Use this for initialization

    [SerializeField]
    private bool _rockTower;
    [SerializeField]
    private bool _flameTower;
    [SerializeField]
    private bool _archerTower;

    [SerializeField]
    private GameObject _rockTowerBtn;
    [SerializeField]
    private GameObject _flameTowerBtn;
    [SerializeField]
    private GameObject _archerTowerBtn;
    private Tower _selectedTower;
    Tower _clickedTower;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _buildTile = GetComponent<Collider>();
    }
    void Start()
    {
        _spriteRenderer.enabled = false;
        _rockTowerBtn.SetActive(_rockTower);
        _flameTowerBtn.SetActive(_flameTower);
        _archerTowerBtn.SetActive(_archerTower);
        layer = ~(1 << LayerMask.NameToLayer(nameOfLayer));
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.tag);
            }
        }
        */

        if (Input.GetMouseButtonDown(0))
        {
            /*Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            */
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
            {

                //Debug.Log(hit.collider.tag);

                if (hit.collider.tag == "BuildSite")
                {
                    if (_previouslyClickedTower != null)
                    {
                        _previouslyClickedTower.IsClicked = false;
                        foreach (Transform child in _previouslyClickedTower.transform)
                        {
                            if (child.tag != "Health")
                                child.gameObject.SetActive(false);
                        }
                        _previouslyClickedTower = null;
                    }
                    if (_towerBtnPressed != null || _movingTower)
                    {

                        _buildTile = hit.collider;
                        RegisterBuildSite(_buildTile);
                        if (_movingTower)
                        {
                          
                            PlaceTower(hit, _movingTowerObject);
                           
                            //Debug.Log(_towerBtnPressed.TowerObject);
                        }
                        else
                        {
                            PlaceTower(hit, _towerBtnPressed.TowerObject);

                        }

                    }
                }
                else if (hit.collider.tag == "UpgradeTower")
                {
                    Debug.Log("Upgrade tower" + _previouslyClickedTower.name);
                    _movingTower = false;
                    DisableDragSprite();
                    _previouslyClickedTower.UpgradeTower();

                }
                else if (hit.collider.tag == "MoveTower")
                {
                    //move the Tower
                    //start by saving the tower position
                    _movingTowerObject = _previouslyClickedTower;
                    EnableDragSprite(_previouslyClickedTower.DragSprite);
                    _movingTower = true;


                }
                else if (hit.collider.tag == "SellTower")
                {
                    if (_previouslyClickedTower.Projectile != null)
                        Destroy(_previouslyClickedTower.Projectile.gameObject);
                    GameManager.Instance.AddMoney(_previouslyClickedTower.SellPrice);
                    Destroy(_previouslyClickedTower.gameObject);
                    _movingTower = false;
                    DisableDragSprite();



                }

                else if (hit.collider.tag == "Tower" && !_towerBtnPressed)
                {
                  
                    if (GameManager.Instance.currentState != GameStatus.duringWave)
                    {

                        _movingTower = false;
                        DisableDragSprite();
                        //Debug.Log(hit.collider.gameObject.tag);
                        //Tower _clickedTower = hit.collider.gameObject.GetComponent<Tower>() as Tower;
                         _clickedTower = hit.collider.gameObject.GetComponentInParent<Tower>() as Tower;
                        //Debug.Log(_clickedTower);
                        if (_clickedTower != null)
                        {
                           
                            if (_clickedTower.IsClicked == false && _previouslyClickedTower == null)
                            {
                                _clickedTower.IsClicked = true;
                                foreach (Transform child in _clickedTower.gameObject.transform.Find("Body"))
                                {
                                    //Debug.Log("hey you");
                                    if (child.tag != "Health")
                                        child.gameObject.SetActive(true);
                                }
                                _previouslyClickedTower = _clickedTower;
                            }
                            else
                            if (_clickedTower.IsClicked == false && _previouslyClickedTower != null)
                            {
                                _previouslyClickedTower.IsClicked = false;
                                _clickedTower.IsClicked = true;
                                foreach (Transform child in _clickedTower.gameObject.transform.Find("Body"))
                                {
                                    if (child.tag != "Health")
                                        child.gameObject.SetActive(true);
                                }
                                foreach (Transform child in _previouslyClickedTower.gameObject.transform.Find("Body"))
                                {
                                    if (child.tag != "Health")
                                        child.gameObject.SetActive(false);
                                }
                                _previouslyClickedTower = _clickedTower;

                            }

                        }
                    }
                }
                else
                {
                    _movingTower = false;
                    DisableDragSprite();
                    _towerBtnPressed = null;
                    if (_previouslyClickedTower != null)
                    {
                        _previouslyClickedTower.IsClicked = false;
                        foreach (Transform child in _previouslyClickedTower.gameObject.transform.Find("Body"))
                        {
                            if (child.tag != "Health")
                                child.gameObject.SetActive(false);
                        }
                        _previouslyClickedTower = null;
                       
                    }


                }


            }

            /*
         Debug.Log(hit.collider);


         if (hit.collider.tag == "UpgradeTower")
         {
             Debug.Log("Upgrade tower" + _previouslyClickedTower.name);
             _movingTower = false;
             DisableDragSprite();
             _previouslyClickedTower.UpgradeTower();

         }
         else if (hit.collider.tag == "MoveTower")
         {
             //move the Tower
             //start by saving the tower position
             _movingTowerObject = _previouslyClickedTower;
             EnableDragSprite(_previouslyClickedTower.DragSprite);
             _movingTower = true;


         }
         else if (hit.collider.tag == "SellTower")
         {
             if (_previouslyClickedTower.Projectile != null)
                 Destroy(_previouslyClickedTower.Projectile.gameObject);
             GameManager.Instance.AddMoney(_previouslyClickedTower.SellPrice);
             Destroy(_previouslyClickedTower.gameObject);
             _movingTower = false;
             DisableDragSprite();



         }
         else if (hit.collider.tag == "Tower" && !_towerBtnPressed)
         {
             if (GameManager.Instance.currentState != GameStatus.duringWave)
             {
                 _movingTower = false;
                 DisableDragSprite();
                 //Debug.Log(hit.collider.gameObject.tag);
                 Tower _clickedTower = hit.collider.gameObject.GetComponent<Tower>() as Tower;
                 Debug.Log(_clickedTower);
                 if (_clickedTower != null)
                 {
                     if (_clickedTower.IsClicked == false && _previouslyClickedTower == null)
                     {                           
                         _clickedTower.IsClicked = true;
                         foreach (Transform child in _clickedTower.transform)
                         {
                             if (child.tag != "Health")
                                 child.gameObject.SetActive(true);
                         }
                         _previouslyClickedTower = _clickedTower;
                     }
                     else
                     if (_clickedTower.IsClicked == false && _previouslyClickedTower != null)
                     {
                         _previouslyClickedTower.IsClicked = false;
                         _clickedTower.IsClicked = true;
                         foreach (Transform child in _clickedTower.transform)
                         {
                             if (child.tag != "Health")
                                 child.gameObject.SetActive(true);
                         }
                         foreach (Transform child in _previouslyClickedTower.transform)
                         {
                             if (child.tag != "Health")
                                 child.gameObject.SetActive(false);
                         }
                         _previouslyClickedTower = _clickedTower;

                     }

                 }
             }
         }

         else if (hit.collider.tag == "BuildSite")
         {
             if (_previouslyClickedTower != null)
             {
                 _previouslyClickedTower.IsClicked = false;
                 foreach (Transform child in _previouslyClickedTower.transform)
                 {
                     if (child.tag != "Health")
                         child.gameObject.SetActive(false);
                 }
                 _previouslyClickedTower = null;
             }
             if (_towerBtnPressed != null || _movingTower)
             {

                 _buildTile = hit.collider;
                 RegisterBuildSite(_buildTile);
                 if (_movingTower)
                     PlaceTower(hit, _movingTowerObject);

                 else
                     PlaceTower(hit, _towerBtnPressed.TowerObject);
             }
         }

         else
         {
             _movingTower = false;
             DisableDragSprite();
             if (_previouslyClickedTower != null)
             {
                 _previouslyClickedTower.IsClicked = false;
                 foreach (Transform child in _previouslyClickedTower.transform)
                 {if(child.tag != "Health")
                     child.gameObject.SetActive(false);
                 }
                 _previouslyClickedTower = null;
             }


         }


     }*/

        }
        if (_spriteRenderer.enabled)
        {
            FollowMouse();
        }
    }
    public void RegisterBuildSite(Collider buildTag)

    {
        _buildList.Add(buildTag);
    }

    public void RegisterTower(Tower tower)
    {
        _towerList.Add(tower);
    }

    public void RenameTagsBuildSites()
    {
        foreach (Collider buildTag in _buildList)
        {
            buildTag.tag = "BuildSite";
        }
        _buildList.Clear();
    }

    public void DestroyAllTower()
    {
        foreach (Tower tower in _towerList)
        {if(tower != null)
            Destroy(tower.gameObject);
        }
        foreach (Tower tower in _towerListDestroyed)
        {
            Destroy(tower.gameObject);
        }
        _towerList.Clear();
        _towerListDestroyed.Clear();
    }
    public void UnregisterTower(Tower tower)
    {
        _towerList.Remove(tower);
        RegisterTowerDestroyed(tower);
        

    }
    public void RegisterTowerDestroyed(Tower tower)
    {
        _towerListDestroyed.Add(tower);
    }
    public void PlaceTower(RaycastHit hit, Tower tower)
    {

        if (!EventSystem.current.IsPointerOverGameObject() && (_towerBtnPressed != null || _movingTower) && GameManager.Instance.currentState != GameStatus.duringWave)
        {


            Tower newTower = Instantiate(tower);
            /*
            // Tower newTower = Instantiate(_towerBtnPressed.TowerObject);
           */
            newTower.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, hit.transform.position.z);
            newTower.transform.rotation = Quaternion.Euler(90, 0, 0);
            //
            if (_towerBtnPressed != null && _movingTower == false)
            {
                BuyTower(_towerBtnPressed.TowerPrice);
                RegisterTower(newTower);
                _towerBtnPressed = null;
            }
            else if (_towerBtnPressed == null && _movingTower == true)
            {
                newTower.transform.Find("Body").gameObject.SetActive(true);

                foreach (Transform child in newTower.transform.Find("Body"))
                {
                    if (child.tag != "Health")
                        child.gameObject.SetActive(false);
                }
                if (_movingTowerObject.Projectile)
                    Destroy(_movingTowerObject.Projectile.gameObject);
                Destroy(_movingTowerObject.gameObject);
            }
            DisableDragSprite();
            _movingTower = false;
        }
        else if (GameManager.Instance.currentState == GameStatus.duringWave)
            DisableDragSprite();
        
           
    }
    public void BuyTower(int price)
    {
        GameManager.Instance.SubtractMoney(price);
    }

    public void SelectedTower(TowerBtn towerSelected)
    {
        if (towerSelected.TowerPrice <= GameManager.Instance.TotalMoney && GameManager.Instance.currentState != GameStatus.duringWave && GameManager.Instance.currentState != GameStatus.beforeStart)
        {
            _towerBtnPressed = towerSelected;
            EnableDragSprite(_towerBtnPressed.DragSprite);
          
            
        }
    }
    public void FollowMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //this will make sure its on the top of everthing
        transform.position = new Vector3(transform.position.x,  -2.8f,transform.position.z);
        //transform.position = new Vector3(-0.1056679f, -2.76f, -9.056301f);
    }

    public void EnableDragSprite(Sprite sprite)
    {
        
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = sprite;
    }

    public void DisableDragSprite()
    {
        _spriteRenderer.enabled = false;

    }
    public void Hide() {
   
        if(_clickedTower != null)
        foreach (Transform child in _clickedTower.gameObject.transform.Find("Body"))
        {
               
            if (child.tag != "Health")
                child.gameObject.SetActive(false);
        }
      
    }
}
