using UnityEngine;
using System.Collections;

/// <summary>
/// Componente que permite a una cámara seguir al jugador en vista cenital.
/// </summary>
public class CameraFollow : MonoBehaviour {
	public GameObject target;		// Objeto al que la cámara ha de seguir (el jugador)
	public float smoothTime = 0.3f;	// Tiempo que tarda la cámara en centrarse en el objeto (se suaviza su movimiento)
	public float altitude = 3f;		// Altitud de la cámara
	public float zOffset = 3f;		// Desplazamiento de la cámara sobre el eje z para corregir su centro manualmente

	private Vector3 velocity = Vector3.zero; // Velocidad de la cámara, utilizada por el mecanismo de suavizado de movimiento

	// llamado cuando el objeto es creado (cuando comienza la partida en este caso)
	void Awake () {
		// Mueve la cámara a la posición objetivo, esté donde esté (se mantiene su rotación)
		transform.position = target.transform.position + new Vector3 (0, altitude, zOffset);
	}

	// Llamado en cada frame
	void Update () {
		// Si target tiene una referencia a un objeto
		if (target != null) {
			// Calcula la posición objetivo (se suma a la posición del objeto objetivo la altitud y el desplazamiento sobre el eje z dados)
			Vector3 goalPos = target.transform.position + new Vector3 (0, altitude, zOffset);
			// Se mueve la cámara hacia la posición objetivo de manera suavizada
			transform.position = Vector3.SmoothDamp (transform.position, goalPos, ref velocity, smoothTime);
		}
	}
}