using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controlador de los enemigos.
/// </summary>
public class Tuto_MonsterController : MonoBehaviour {
	public int health = 5;					// Vida del enemigo (se resta 1 por cada disparo)
	public GameObject damageParticles;		// Partículas a instanciar al ser dañado
	public GameObject deathParticles;		// Partículas a instanciar al ser derrotado
	public int strength = 10;				// Vida a restar cuando el monstruo ataca al jugador
	public int score = 100;					// Puntuación que otorgará al ser derrotado

	private NavMeshAgent agent;				// Referencia al componente NavMeshAgent
	private Tuto_GameController gameController;	// Referencia al GameController
	private GameObject player;				// Referencia al objeto jugador

	// Llamado cuando el objeto es creado
	void Awake () {
		// Obtiene referencias
		agent = GetComponent<NavMeshAgent> ();
		gameController = (GameObject.FindGameObjectWithTag ("GameController")).GetComponent <Tuto_GameController> ();
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Llamado en cada frame
	void Update () {
		if (player) // Si player contiene una referencia (al jugador)
			agent.destination = player.transform.position; // Actualiza la posición objetivo a la posición actual del jugador
		else
			agent.enabled = false; // Si player no contiene ninguna referencia, desactiva el componente agente
	}

	/// <summary>
	/// Devuelve el poder del monstruo (la vida a restar cuando este toca al jugador).
	/// </summary>
	/// <returns>El poder.</returns>
	public int getStrength () {
		return strength;
	}

	/// <summary>
	/// Daña al monstruo.
	/// </summary>
	/// <param name="hitPosition">La posición donde el monstruo es golpeado</param>
	public void damage (Vector3 hitPosition) {
		// Instancia el efecto de daño en la posición donde el monstruo ha sido golpeado con este objeto como padre
		GameObject particles = GameObject.Instantiate (damageParticles, hitPosition, Quaternion.identity, transform.parent);
		// Escala el efecto según el tamaño de este objeto
		particles.transform.localScale = transform.localScale;

		// Resta un punto de vida al monstruo
		health--;

		// Si la vida ha bajado a 0, se llama al método "die"
		if (health <= 0) {
			die ();
		}
	}

	/// <summary>
	/// Destruye el monstruo.
	/// </summary>
	public void die () {
		// Instancia las partículas de muerte con la escena como padre (no serán destruidas junto al monstruo)
		ParticleSystem particles = GameObject.Instantiate (deathParticles, 
			transform.position, Quaternion.Euler (-90, 0, 0), transform.parent).GetComponent<ParticleSystem> ();
		// Cambia el tamaño de las partículas según el tamaño de este objeto (al modificar "startSizeMultiplier" en lugar
		// de "localScale" se consigue que las partículas alcancen la misma altura pero sea más grandes)
		ParticleSystem.MainModule pSMain = particles.main;
		pSMain.startSizeMultiplier = transform.localScale.x*1.5f;

		// Añade puntuación al jugador
		gameController.addScore (score);

		// Destruye este objeto
		Destroy (gameObject);
	}
}
