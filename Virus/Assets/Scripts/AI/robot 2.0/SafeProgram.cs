using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
//safe program means that this gameObject represent a normal software in a pc (ex : photoshop, chrome, notepad...)
public class SafeProgram : MonoBehaviour
{
    #region variables

    #region private variables

    private Animator _animator;
    private static readonly int AnimationNumber = Animator.StringToHash("animation number");

    #endregion

    #region public variables

    [HideInInspector] public float currentHealth;
    public Image healthSlider;
    public int maxHealth = 100;

    
    #endregion

    #endregion

    #region buildin methods

    private void Start()
    {
        _animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        StartCoroutine(SelectAnAnimation(0));
    }

    private void Update()
    {
        healthSlider.fillAmount = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    #endregion
    
    #region custom methods

    private IEnumerator SelectAnAnimation(int periodOfTime)
    {
        yield return new WaitForSeconds(periodOfTime);
        int randomAnimation = Random.Range(1, 9);
        _animator.SetInteger(AnimationNumber, randomAnimation);
        int randomPeriodOfTime = Random.Range(5, 11);
        StartCoroutine(SelectAnAnimation(randomPeriodOfTime));
    }

    #endregion
    
}
