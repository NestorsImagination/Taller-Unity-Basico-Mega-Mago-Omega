using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador principal del juego.
/// </summary>
public class Tuto_GameController : MonoBehaviour {
	public Slider healthBar;					// Referencia a la barra de vida
	public Text healthText;						// Refencia al texto que muestra la vida restante
	public Image damageScreen;					// Referencia a la imagen en la GUI que avisa al jugador de que ha recibido daño
	public float damageScreenMaxAlpha;			// Valor de alfa que se asignará a la imagen de daño al ser atacado
	public float damageScreenSpeed;				// Velocidad a la que desaparecerá la imagen de daño
	public Text scoreText;						// Referencia al texto que muestra la puntuación actual
	public GameObject gameOverText;				// Referencia al texto que se mostraŕa cuando se acabe la partida
	public Tuto_MonsterSpawnerController spawners;	// Referencia al componente que genera monstruos en los puntos de generación

	private Color damageScreenColor;			// Referencia al objeto Color de la imagen de daño

	private bool damaged;						// Utilizado para indicar cuando se debe mostrar la imagen de daño	
	private int score = 0;						// Puntuación actual

	private bool dead = false;					// True si el jugador ha perdido

	// Llamada al crearse el GameObject (al comenzar el juego, en este caso)
	void Awake () {
		// Obtiene la referencia al objeto color de la imagen de daño. Es necesario para modificar el alfa (transparencia) del color,
		// ya que no es posible modificar un valor individual directamente
		damageScreenColor = damageScreen.color;
	}
		
	/// <summary>
	/// Función llamada en cada frame.
	/// </summary>
	void Update () {
		// Actualiza la transparencia de la imagen de daño (si se ha sido dañado recientemente)
		updateDamageScreen ();

		// Si la partida acabó y se pulsa el click izquierdo del ratón, se vuelve a cargar esta escena
		if (dead) {
			if (Input.GetMouseButtonDown (0))
				SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		}
	}

	/// <summary>
	/// Actualiza la transparencia de la imagen de daño (si se ha sido dañado recientemente).
	/// </summary>
	private void updateDamageScreen () {
		// Si el jugador acaba de ser dañado
		if(damaged) {
			damaged = false; // Se resetea la flag

			// Se hace aparecer la imagen dándole el valor dado por "damageScreenMaxAlpha" a su transparencia
			damageScreenColor.a = damageScreenMaxAlpha;
			damageScreen.color = damageScreenColor;	// Se actualiza la transparencia
		} else {
			// Se hace desaparecer la imagen según la velocidad dada en "damageScreenSpeed"
			damageScreenColor.a =  Mathf.Lerp (damageScreenColor.a, 0.0f, damageScreenSpeed * Time.deltaTime);
			damageScreen.color = damageScreenColor;	// Se actualiza la transparencia
		}
	}

	/// <summary>
	/// Llamado cuando el jugador es dañado.
	/// </summary>
	/// <returns><c>true</c>, si el jugador ha sido derrotado, <c>false</c> si no es así.</returns>
	/// <param name="damage">Daño recibido.</param>
	public bool damagePlayer (int damage) {
		healthBar.value -= damage; // Resta a la vida restante el daño causado
		healthText.text = healthBar.value+"/"+healthBar.maxValue; // Actualiza el texto que muestra la vida del jugador

		damaged = true; // Se mostrará la imagen de daño

		// Si la vida ha llegado a 0
		if (healthBar.value == 0) {
			dead = true; // El jugador ha sido derrotado

			spawners.turnOff (); // Desactiva los generadores de monstruos
			gameOverText.SetActive (true); // Muestra la interfaz de Game Over
		}

		return dead;
	}

	/// <summary>
	/// Añade a la puntuación el valor de "scorePoints".
	/// </summary>
	/// <param name="scorePoints">Puntuación a añadir.</param>
	public void addScore (int scorePoints) {
		score += scorePoints; // Suma la puntuación
		scoreText.text = "Puntuación: " + score; // Actualiza el texto
	}
}
