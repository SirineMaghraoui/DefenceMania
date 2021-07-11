using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentScript : MonoBehaviour {
    [SerializeField]
    HeroControls _hero;
    private NavMeshAgent _agent;
    string nameOfLayer = "battle1";
    LayerMask layer;
    public Vector3 agentDestination;
    // Use this for initialization
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    void Start () {
        _agent.updateRotation = false;
        layer =  ~(1 << LayerMask.NameToLayer(nameOfLayer));
       
    }
	
	// Update is called once per frame
	void Update () {
        //transform.Rotate(0, 0, 0);
        transform.Rotate(90, 0, 0);
        if(GameManager.Instance.currentState == GameStatus.duringWave)
        if (Input.GetMouseButtonDown(1))
        {
            if(!_hero.IsDead)
            {

         
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit, Mathf.Infinity, layer))
            {
               
                if (hit.collider.tag == "Ground1" ||hit.collider.tag == "EnemyTower")
                {
                    //Debug.Log(hit.point);
                    //transform.position = new Vector3(0, 0, 0);
                    _hero.NearestTower = null;
                        //Debug.Log(hit.collider.tag);
                        
                    
                    _agent.SetDestination(hit.point);
                        agentDestination = hit.point;
                       
                    
                    //_agent.destination = 
                }
            }

            }
        }
        //transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    public NavMeshAgent Agent
    {
        get
        {
            return _agent;
        }
    }
}
