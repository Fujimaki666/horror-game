using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHp = 5;
    private int currentHp;
    [SerializeField] Image DamageImg;
    public Slider hpSlider;
    public AudioClip damage1;
    AudioSource audioSource;
    public bool isInvincible = false;
    private Coroutine damageEffectCoroutine;
    void Start()
    {
        currentHp = maxHp;
        hpSlider.maxValue = maxHp;
        hpSlider.value = currentHp;
        audioSource = GetComponent<AudioSource>();
        DamageImg.color = Color.clear;
    }

    public void TakeDamage(int damage)
    {
        GameManager.Instance?.OnPlayerDamaged(currentHp);
        audioSource.PlayOneShot(damage1);

        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;
       
        hpSlider.value = currentHp;
        if (damageEffectCoroutine != null) StopCoroutine(damageEffectCoroutine);
        damageEffectCoroutine = StartCoroutine(DamageFlash());
        if (currentHp == 0)
        {
            Debug.Log("ゲームオーバー！");
            // ここにゲームオーバー演出やリトライ処理などを追加
        }

    }

    IEnumerator DamageFlash()
    {
        DamageImg.color = new Color(0.7f, 0, 0, 0.7f);
        float fadeSpeed = 2f;

        while (DamageImg.color.a > 0f)
        {
            DamageImg.color = Color.Lerp(DamageImg.color, Color.clear, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        DamageImg.color = Color.clear;
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp) currentHp = maxHp;

        hpSlider.value = currentHp;
    }
}
