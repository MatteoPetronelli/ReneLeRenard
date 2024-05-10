using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private float _timeToDrain = 0.25f;
    [SerializeField] private Gradient _healthBarGradient;
    private Image _image;
    private float _target = 1f;
    private Coroutine _drainHealthBarCoroutine;

    private void Start()
    {
        _image = GetComponent<Image>();
        CheckHealthGradientAmount();
    }
    
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    { 
        _target = currentHealth / maxHealth;

        _drainHealthBarCoroutine = StartCoroutine(drainHealthBar());

        CheckHealthGradientAmount();
    }

    private IEnumerator drainHealthBar()
    {
        float fillAmount = _image.fillAmount;

        float elapsedTime = 0f;
        while (elapsedTime < _timeToDrain)
        {
            elapsedTime += Time.deltaTime;
            _image.fillAmount = Mathf.Lerp(fillAmount, _target, (elapsedTime / _timeToDrain));
            yield return null;
        }
    }

    private void CheckHealthGradientAmount()
    {
        _image.color = _healthBarGradient.Evaluate(_target);
    }
}
