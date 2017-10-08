using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador del mago.
/// </summary>
public class MageController : MonoBehaviour {
	public float speed;				// Velocidad a la que se moverá el mago
	public GameObject shotPrefab;	// Prefab del proyectil a instanciar
	public Transform shotGenerator;	// Referencia al objeto desde cuya posición se generarán los proyectiles
	public float shotFrequency;		// Tiempo en segundos que ha de transcurrir entre cada disparo
	public float bulletSpeed;		// Velocidad a la que se moverán los proyectiles
	public float immuneTime;		// Tiempo durante el cual el mago es inmune a todo daño tras ser atacado
	public LayerMask floorMask;		// Layer en la que se encuentra el suelo, utilizada para detectar a dónde está apuntando el usuario con el ratón

	private GameController gameController;		// Referencia al GameController

	private float camRayLength = 100f;          // Longitud del rayo generado desde el ratón al suelo

	private Rigidbody rb;								// Referencia al RigidBody
	private Animator animator;							// Referencia al Animator
	private Vector3 movement = new Vector3 ();			// Vector que determina el movimiento del personaje
	private Vector3 mousePosition = new Vector3 ();		// Posición del ratón
	private Vector3 mageDirection = new Vector3 ();		// Dirección a la que encara el personaje

	private float shotTimer = 0;						// Temporizador para controlar la generación de disparos según la frecuencia dada
	private float immuneTimer = 0;						// Temporizador para controlar el tiempo de inmunidad del mago tras ser atacado

	// Llamado cuando el objeto es creado (al comenzar la partida, en este caso)
	void Awake () {
		// Obtiene referencias
		rb = GetComponent<Rigidbody> ();
		animator = GetComponent<Animator> ();
		gameController = (GameObject.FindGameObjectWithTag ("GameController")).GetComponent <GameController> ();
	}

	// Llamado en cada frame
	void Update () {
		getInput ();			// Obtiene inputs del juegador
		getMouse ();			// Obtiene el punto al que está apuntando el jugador con el ratón
		move ();				// Mueve el mago según los inputs obtenidos
		rotate ();				// Rota el mago según la posición del ratón
		shot ();				// Intenta disparar
		updateImmuneTimer ();	// Actualiza el temporizador de inmunidad
	}

	/// <summary>
	/// Obtiene los inputs del jugador.
	/// </summary>
	private void getInput () {
		// El eje Horizontal está asignado a los botones "A" (negativo) y "D" (positivo), el Vertical a "W" (positivo) y "S" (negativo)
		// Da valor al vector "movement" según estos ejes (x, y, z)
		movement.Set (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
	}

	/// <summary>
	/// Obtiene el punto al que el jugador está apuntando con el ratón.
	/// </summary>
	private void getMouse () {
		// Crea un rayo desde el cursor del ratón en la dirección de la cámara
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		// Esta variable RaycastHit guardará la información acerca de la colisión del rayo
		RaycastHit floorHit;

		// Hace el raycast según el rayo definido, obteniendo el resultado en "flootHit", con la longitud
		// dada por "camRayLength" y colisionando únicamente con la layer determinada por "floorMask"
		if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)) {
			mousePosition = floorHit.point;	// Obtiene el punto de colisión (con el suelo)

			// Crea un vector que comienza en la posición del mago y termina en la posición donde se está apuntando con el ratón
			// (la dirección a la que se ha de rotar el mago para que encare la posición del ratón)
			mageDirection = mousePosition - transform.position;

			// Se asegura que este vector de dirección no está inclinado (sobre el eje Y)
			mageDirection.y = 0;
		}
	}

	/// <summary>
	/// Mueve el mago según los inputs obtenidos.
	/// </summary>
	private void move () {
		// Si el usuario ha pulsado algún botón de dirección, se reproducirá la animación de "caminando"
		animator.SetBool ("Walking", (movement != Vector3.zero));
			
		rb.velocity = movement.normalized * speed; // Da valor a la velocidad del RigidBody
	}

	/// <summary>
	/// Rota el mago según la posición donde apunta el ratón
	/// </summary>
	private void rotate () {
		// Crea un Quaternion (tipo propio de los ángulos) a partir de la dirección calculada en "getMouse"
		Quaternion newRotation = Quaternion.LookRotation (mageDirection);

		rb.MoveRotation (newRotation); // Rota el mago
	}

	/// <summary>
	/// Genera un disparo si el usuario lo desea y la frecuencia de disparo lo permite.
	/// </summary>
	private void shot () {
		if (shotTimer > 0) { // Si se ha disparado recientemente, actualiza el temportizador "shotTimer"
			shotTimer -= Time.deltaTime; // Nota: deltaTime guarda el tiempo transcurrido (en segundos) entre el frame anterior y el actual
		} else if (Input.GetAxis ("Fire") > 0) { // Si es posible disparar y el usuario está pulsando el botón de disparo...
			// Obtiene el vector de movimiento del disparo al multiplicar la dirección del mago normalizada
			// por la velocidad a la que se desea mover el proyectil
			Vector3 bulletVelocity = mageDirection.normalized * bulletSpeed;
			// Instancia el proyectil desde el generador de disparos
			GameObject bullet = GameObject.Instantiate (shotPrefab, shotGenerator.position, Quaternion.identity);
			// Da velocidad al proyectil
			(bullet.GetComponent<Bullet> ()).setVelocity (bulletVelocity);
			// Da valor al temporizador de disparos para que no se pueda volver a disparar en un tiempo determinado por "shotFrequency"
			shotTimer = shotFrequency;
		}
	}

	/// <summary>
	/// Actualiza el temporzador de inmunidad.
	/// </summary>
	private void updateImmuneTimer () {
		if (immuneTimer > 0)
			immuneTimer -= Time.deltaTime;
	}

	// Llamado cuando sucede una colisión (en cualquier momento, no como en OnCollisionEnter donde solo se llama justo al comenzar la colisión)
	private void OnCollisionStay (Collision collision) {
		if (immuneTimer <= 0) { // Si el mago no es inmune actualmente
			if (collision.gameObject.CompareTag ("Enemy")) { // Si se ha colisionado con un enemigo
				// Se llama al método "damagePlayer" del GameController para restar el poder del monstruo ("getStrength") a la vida del jugador,
				// y en el caso de que este método devuelva "true" (la vida ha llegado a 0), se llama al método "die"
				if (gameController.damagePlayer ((collision.gameObject.GetComponent<MonsterController> ()).getStrength ()))
					die ();
				else {
					// Si el jugador no ha sido derrotado aún, se da valor al temporizador de inmunidad para evitar recibir daño durante
					// un tiempo determinado por "immuneTime"
					immuneTimer = immuneTime; 
				}
			}
		}
	}

	/// <summary>
	/// Método llamado cuando el jugador es derrotado.
	/// </summary>
	private void die () {
		// Obtiene el efecto de explosión (su primer hijo)
		Transform deathParticles = transform.GetChild (0);
		// Activa el efecto de explosión
		deathParticles.gameObject.SetActive (true);
		// Hace que el padre del efecto sea la propia escena, de manera que la explosión no sea destruida junto al mago
		deathParticles.transform.SetParent (transform.parent);

		// Destruye este objeto
		Destroy (gameObject);
	}
}