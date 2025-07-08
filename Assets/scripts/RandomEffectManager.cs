using UnityEngine;
using System.Collections;

public class RandomEffectManager : MonoBehaviour
{
    public DropObjectController dropObjectController;
    public PlayRandomSound soundPlayer;
    public Charahealth charaHealth;
    public ChaseTarget enemy;
    public ScatterShot scatterShot;
    public SpawnInFront pawnInFront;
    public FlipTarget flipTarget;
    public PlayerHealth playerHealth;



    public void TriggerRandomEffect()
    {
        //GameManager.Instance.SetPhase(GamePhase.Scare);

        int randomChoice = Random.Range(0, 5);
        if (randomChoice == 0)
        {
            dropObjectController.DropObject();
            StartCoroutine(DelayedDamage(1.0f));
        }
        else if (randomChoice == 1)
        {
            soundPlayer.PlayRandomClip();
            StartCoroutine(DelayedDamage(1.0f));
        }
        else if (randomChoice == 2)
        {
            scatterShot.Fire();
            StartCoroutine(DelayedDamage(1.0f));
        }
        else if (randomChoice == 3)
        {
            pawnInFront.SpawnObject();
        }
        else
        {
            flipTarget.Turn();
        }
    }

    private IEnumerator DelayedDamage(float delay)
    {
        if (enemy != null)
        {
            enemy.isReacting = true; // ★リアクション開始
        }

        yield return new WaitForSeconds(delay);
        charaHealth.TakeDamage(1);

        if (enemy != null)
        {
            enemy.TurnAround();
        }

        if (enemy != null)
        {
            enemy.isReacting = false; // ★リアクション終了
        }
    }
}
