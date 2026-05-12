using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isShaking = false;

    [Header("카메라 흔들림 설정")]
    public float shakeDuration = 0.3f;  // 흔들림 지속 시간
    public float shakeMagnitude = 0.1f; // 흔들림 강도

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void Shake()
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeCoroutine());
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // 랜덤한 방향으로 카메라 위치 변경
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 원래 위치로 복귀
        transform.localPosition = originalPosition;
        isShaking = false;
    }
}