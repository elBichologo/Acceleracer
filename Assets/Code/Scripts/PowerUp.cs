using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int pointsValue = 50;
    public float rotationSpeed = 50f;
    public float lifetime = 7f; // Tiempo de vida en segundos
    
    [HideInInspector] public PowerUpSpawner spawner; // Referencia al spawner
    
    private float timeRemaining;
    
    void OnEnable()
    {
        // Reiniciar tiempo de vida cuando se activa
        ResetLifetime();
    }
    
    void Start()
    {
        // Asegurarse que tiene collider trigger y rigidbody adecuados
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }
    
    void Update()
    {
        // Efecto visual: hacer girar el powerup
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Temporizador de vida
        if (gameObject.activeInHierarchy)
        {
            timeRemaining -= Time.deltaTime;
            
            if (timeRemaining <= 0)
            {
                // Expiró tiempo de vida, volver al pool sin dar puntos
                Debug.Log("PowerUp expirado. Volviendo al pool.");
                ReturnToPool();
            }
        }
        
        // Opcionalmente añadir aquí FloatEffect() si queremos ese efecto
    }
    
    public void ResetLifetime()
    {
        timeRemaining = lifetime;
    }
    
    void ReturnToPool()
    {
        if (spawner != null)
        {
            spawner.ReturnToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    // Opcionalmente: animación de flotación
    void FloatEffect()
    {
        // Implementar efecto de flotación usando sin/cos para movimiento vertical
        transform.position = new Vector3(
            transform.position.x,
            Mathf.PingPong(Time.time * 0.5f, 0.5f) + transform.position.y - 0.25f,
            transform.position.z
        );
    }
}