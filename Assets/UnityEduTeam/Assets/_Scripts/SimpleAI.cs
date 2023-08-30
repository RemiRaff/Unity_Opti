using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SimpleAI : MonoBehaviour {
    [Header("Agent Field of View Properties")]
    public float viewRadius;
    public float viewAngle;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Space(5)]
    [Header("Agent Properties")]
    public float runSpeed;
    public float walkSpeed;
    public float patrolRadius;


    private Transform playerTarget;

    private Vector3 currentDestination;

    private bool playerSeen;
    private int maxNumberOfNewDestinationBeforeDeath;
    private enum State {Wandering, Chasing};
    private State currentState;
    private GameObject _player = null;
    private Animator _animator = null;
    private NavMeshAgent _navMeshAgent = null;

    // Use this for initialization
    void Start () {
        currentDestination = RandomNavSphere(transform.position, patrolRadius, -1);
        maxNumberOfNewDestinationBeforeDeath = Random.Range(5, 50);

        // init
        _animator = GetComponentInChildren<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void CheckState()
    {
        FindVisibleTargets();

        switch(currentState)
        {
            case State.Chasing:
                ChaseBehavior();
                break;

            default:
                WanderBehavior();
                break;

        }
    }

    void WanderBehavior()
    {
        _animator.SetTrigger("walk");
        _navMeshAgent.speed = walkSpeed;

        float dist = _navMeshAgent.remainingDistance;

        if (dist != Mathf.Infinity && _navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            currentDestination = RandomNavSphere(transform.position, patrolRadius, -1);
            _navMeshAgent.SetDestination(currentDestination);
            maxNumberOfNewDestinationBeforeDeath--;
            if (maxNumberOfNewDestinationBeforeDeath <= 0)
            {
                Destroy(gameObject);
                // REMI TODO: évènement pour EnemySpawner
            }
        }

    }

    void ChaseBehavior()
    {
        if (playerTarget != null)
        {
            _animator.SetTrigger("run");
            _navMeshAgent.speed = runSpeed;
            currentDestination = playerTarget.transform.position;
            _navMeshAgent.SetDestination(currentDestination);
        }
        else
        {
            playerSeen = false;
            currentState = State.Wandering;
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.tag == "Player")
    //     {
    //     }
    // }

    #region Vision
    void FindVisibleTargets()
    {

        playerTarget = null;
        playerSeen = false;
        
        if (InitPlayer())
        {
            // foreach plus nécessaire
            Vector3 dirToTarget = (_player.transform.position - transform.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dirToTarget, out hit))
            {
                float dstToTarget = Vector3.Distance(transform.position, _player.transform.position);
                if (dstToTarget <= viewRadius)
                {
                    if (Vector3.Angle(transform.forward, dirToTarget) <= viewAngle / 2)
                    {
                        if (hit.collider.tag == "Player")
                        {
                            playerSeen = true;
                            playerTarget = hit.transform;
                        }
                    }
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    #endregion

    private bool HasFindPlayer()
    {
        if (InitPlayer())
        {
            // foreach plus nécessaire
            if (Vector3.Distance(_player.transform.position, transform.position) <= _navMeshAgent.radius*2)
                return true;
        }
        return false;
    }

    private bool InitPlayer() {
        if (!_player) { // player pas encore initialisé
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 0) // player pas présent
                return false;
            _player = players[0]; // 1 player pas de multi donc assignation
        }

        return true;
    }
    
    // Update is called once per frame
    void Update () {
        CheckState();

        if (playerSeen)
        {
            currentState = State.Chasing;
        } else
        {
            currentState = State.Wandering;
        }

        // si on tape le player, chargement de la scène
        if (HasFindPlayer())
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
	}
}
