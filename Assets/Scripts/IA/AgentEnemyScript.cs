
using UnityEngine;
using UnityEngine.AI;
public class AgentEnemyScript : MonoBehaviour {
    private NavMeshAgent _agent;
    [SerializeField]
    private Transform _finishPoint;
    // Use this for initialization
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    void Start () {
        
        _agent.SetDestination(_finishPoint.position);

        _agent.updateRotation = false;
        
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(90, 0, 0);
    }
}
