using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;Å@//á@Å©Ç±ÇÍÇë´ÇµÇƒÇ®Ç©Ç»Ç¢Ç∆ìÆÇ©Ç»Ç¢ÇÃÇ≈ïKê{Ç≈Ç∑ÅB

public class Navigation : MonoBehaviour
{

    public Transform[] goals;
    private int destNum = 0;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goals[destNum].position;
    }

    void nextGoal()
    {

        destNum = Random.Range(0, 3);

        agent.destination = goals[destNum].position;

        Debug.Log(destNum);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(agent.remainingDistance);
        if (agent.remainingDistance < 0.5f)
        {
            nextGoal();
        }

    }
}