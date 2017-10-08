using UnityEngine;

/// <summary>
/// Controlador para los proyectiles.
/// </summary>
public class Bullet : MonoBehaviour {
	public float lifeTime = 4.0f; 	// Tiempo en segundos tras el cual el proyectil será destruido

	private bool destroyed = false; // True si el proyectil ha sido destruido

	// Llamada cuando el proyectil es creado
	void Awake () {
		Destroy (gameObject, lifeTime); // Destruye la bala cuando pasan "lifeTime" segundos
	}

	/// <summary>
	/// Da valor a la velocidad del proyectil.
	/// </summary>
	/// <param name="velocity">La velocidad.</param>
	public void setVelocity (Vector3 velocity) {
		GetComponent<Rigidbody> ().velocity = velocity;
	}

	// Cuando sucede una colisión
	void OnCollisionEnter(Collision collision) {
		// Si no ha sido ya destruido
		if (!destroyed) {
			destroyed = true; // El proyectil ha sido destruido

			Transform sparks = transform.GetChild (0); // Obtiene una referencia al efecto de explosión (su primer hijo)
			sparks.gameObject.SetActive (true);        // Activa la explosión
			sparks.parent = transform.parent;		   // Desliga la explosión del proyectil, haciendo que su padre sea la propia escena (la explosión no será destruida junto al proyectil)
			//Transform trail = transform.GetChild (0);  // Hace lo mismo con la estela del proyectil, de manera que esta no desaparezca de manera brusca
			//trail.parent = transform.parent;			

			if (collision.gameObject.CompareTag ("Enemy")) {	// Si se ha colisionado con un enemigo
				// Llama al método "damage" de su componente "MonsterController" para aplicar un punto de daño, pasando
				// el punto de colisión como argumento para generar el efecto de daño en ese punto
				(collision.gameObject.GetComponent<MonsterController> ()).damage (collision.contacts [0].point); 
			}

			Destroy (this.gameObject); // Destruye el proyectil
		}
	}
}