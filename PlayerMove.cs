using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMove : MonoBehaviour
{

    public int FloorMask;
    //public PlayerMove player;
    private NavMeshAgent player;
    public Vector3 mousemap;
    public float offset;
    public float speed=20f;
    private Animator run;
    public Transform target;
    //private bool isRun=false;
    //private bool isMove = false;
    //public Transform target;
    void Start ()
	{
	    FloorMask = LayerMask.GetMask("floor");
	    player = GetComponent<NavMeshAgent>();
	    run = GetComponent<Animator>();
	    mousemap = player.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
        Debug.Log(mousemap);
	    if (Input.GetMouseButtonDown(0))
	    { 
	        SetPos();
            //Debug.Log(mousemap);
	        Transform new_target=Instantiate(target, mousemap, Quaternion.identity);
           Destroy(new_target.gameObject,1);
	    }
	    offset = (transform.position - mousemap).magnitude;
       
	    run.SetBool("Run", offset > 0.3 ? true : false);
	    player.SetDestination(mousemap);

	}
    public void SetPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, FloorMask))
        {
             mousemap = hit.point;
        }
    }
}
