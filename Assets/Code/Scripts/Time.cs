using UnityEngine;

public class TunnelEffectController : MonoBehaviour
{
    public Material material;
    public float timeToMax = 10f;
    public float maxFactor = 5f;

    private float elapsed = 0f;

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / timeToMax);
        material.SetFloat("_TimeFactor", t * maxFactor);
    }
}
