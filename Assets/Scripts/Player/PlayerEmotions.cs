using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEmotions : MonoBehaviour
{
    [SerializeField]
    private Sprite _happySprite;

    [SerializeField]
    private Sprite _ouchSpriteMinor;

    [SerializeField]
    private Sprite _ouchSpriteMajor;

    [SerializeField, Min(0.1f)]
    private float hurtThreshold = 0.1f;

    [SerializeField, Min(0)]
    private float _hurtCycleImageSpeed = 0.2f;

    [SerializeField]
    private float _painEnduranceMultiplier = 2f;

    private SpriteRenderer _spriteRenderer;

    private Coroutine _hurtCoroutine = null;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _spriteRenderer.sprite = _happySprite;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.GetContact(0).normalImpulse >= hurtThreshold)
        {
            if (_hurtCoroutine != null)
                StopCoroutine(_hurtCoroutine);

            _hurtCoroutine = StartCoroutine(ManifestPhysicalPain(collision));
        }
    }

    private IEnumerator ManifestPhysicalPain(Collision2D collision)
    {
        //if (collision.GetContact(0).normalImpulse)
        _spriteRenderer.sprite = _ouchSpriteMajor;

        yield return new WaitForSeconds(collision.GetContact(0).normalImpulse * _painEnduranceMultiplier);

        if (_hurtCycleImageSpeed == 0)
        {
            yield break;
        }

        _spriteRenderer.sprite = _ouchSpriteMinor;

        yield return new WaitForSeconds(_hurtCycleImageSpeed);

        _spriteRenderer.sprite = _happySprite;
    }
}
