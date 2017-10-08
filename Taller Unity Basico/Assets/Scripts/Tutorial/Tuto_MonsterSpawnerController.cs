using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla los generadores de monstruo.
/// </summary>
public class Tuto_MonsterSpawnerController : MonoBehaviour {
	public List<GameObject> monsters;	// Lista de prefabs de monstruos a instanciar
	public float timeToSpawn;			// Periodo, en segundos, entre generaciones de monstruo

	private float spawnTimer;			// Temporizador para la generación de monstruos
	private bool activated = true;		// Si es "true" se generarán monstruos

	// Llamado cuando el objeto es creado (al comenzar la partida en este caso)
	void Awake () {
		// Inicia el temporizador de generación de monstruos
		spawnTimer = timeToSpawn;
	}

	// Llamado en cada frame
	void Update () {
		// Si está activado (el jugador no ha sido derrotado)
		if (activated) {
			// Si el temporizador es mayor que 0, lo actualiza
			if (spawnTimer > 0) {
				spawnTimer -= Time.deltaTime;
			} else { // Si el temporizador ha llegado a 0
				// Para cada hijo de este objeto (generadores de monstruo)
				for (int c = 0; c < transform.childCount; c++) {
					// Instancia un monstruo al azar sobre este generador
					GameObject.Instantiate (monsters [Random.Range (0, monsters.Count)], transform.GetChild (c).position, 
						Quaternion.Euler (new Vector3 (0, Random.Range (0.0f, 360f), 0.0f)), null);
				}

				// No se generarán nuevos monstruos hasta transcurridos "timeToSpawn" segundos
				spawnTimer = timeToSpawn;
			}
		}
	}

	/// <summary>
	/// Desactiva la generación de monstruos (el jugador ha sido derrotado)
	/// </summary>
	public void turnOff () {
		activated = false;
	}
}
