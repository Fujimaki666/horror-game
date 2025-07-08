using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public class SpawnInFront : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Transform target;
    public float distanceInFront = 2f;
    private NavMeshAgent agent;
    public AudioClip lion; // ï°êîÇÃâπê∫ÉNÉäÉbÉv
    private AudioSource audioSource;
    public Charahealth charaHealth;
    void Start()
    {
        agent = target.GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    /*void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SpawnObject();
            
        }
    }*/

    public void SpawnObject()
    {
        agent = target.GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        Vector3 spawnPosition = target.position + target.forward * distanceInFront;
        Vector3 directionToTarget = target.position - spawnPosition;
        Quaternion spawnRotation = Quaternion.LookRotation(directionToTarget);

        GameObject spawned = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
        audioSource.PlayOneShot(lion);
        ChaseTarget chase = target.GetComponent<ChaseTarget>();
        if (chase != null)
        {
            
            StartCoroutine(chase.PauseForDuration(2f));
        }
        Destroy(spawned, 2f);

        
    }

   
}
