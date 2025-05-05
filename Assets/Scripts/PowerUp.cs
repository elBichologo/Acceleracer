using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int pointsValue = 50;
    public float rotationSpeed = 50f; // Para un efecto visual que lo haga girar
    
    void Start()
    {
        // Asegurarse que tiene collider trigger y rigidbody adecuados
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.Log("PowerUp: Collider establecido como Trigger");
        }
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            Debug.Log("PowerUp: Añadido Rigidbody sin gravedad y kinematic");
        }
    }
    
    void Update()
    {
        // Efecto visual: hacer girar el powerup
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
    
    // Opcionalmente: animación de flotación
    void FloatEffect()
    {
        // Implementar efecto de flotación usando sin/cos para movimiento vertical
        float yOffset = Mathf.Sin(Time.time) * 0.2f;
        transform.position = new Vector3(
            transform.position.x,
            Mathf.PingPong(Time.time * 0.5f, 0.5f) + transform.position.y - 0.25f,
            transform.position.z
        );
    }
    
    // Ya no necesitamos OnTriggerEnter aquí
}