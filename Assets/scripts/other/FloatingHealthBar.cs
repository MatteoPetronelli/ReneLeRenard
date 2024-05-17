using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private float _timeToDrain = 0.25f;
    [SerializeField] private Gradient _healthBarGradient;
    private Image _image;
    private Color _newHealthBarColor;
    private float _target = 1f;
    private Coroutine _drainHealthBarCoroutine;

    private void Start()
    {
        _image = GetComponent<Image>();

        _image.color = _healthBarGradient.Evaluate(_target);

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
        Color currentColor = _image.color;

        float elapsedTime = 0f;
        while (elapsedTime < _timeToDrain)
        {
            elapsedTime += Time.deltaTime;

            _image.fillAmount = Mathf.Lerp(fillAmount, _target, (elapsedTime / _timeToDrain));

            _image.color = Color.Lerp(currentColor, _newHealthBarColor, (elapsedTime / _timeToDrain));

            yield return null;
        }
    }

    private void CheckHealthGradientAmount()
    {
        _newHealthBarColor = _healthBarGradient.Evaluate(_target);
    }
}
