using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyShoot : MonoBehaviourPun
{
    // Start is called before the first frame update
    public GameObject projectilePrefab; // Prefab del proyectil
    public Transform firePoint; // Punto de origen del disparo
    public float fireRate = 1f; // Tiempo entre disparos (en segundos)
    public float detectionRange = 10f; // Rango de detecci�n del jugador
    public float rotationSpeed = 5f; // Velocidad de rotaci�n hacia el jugador

    private float nextFireTime = 0f; // Tiempo para el pr�ximo disparo
    private GameObject targetPlayer; // Jugador objetivo

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) // Solo el due�o del PhotonView puede disparar
            return;

        FindNearestPlayer(); // Buscar al jugador m�s cercano

        if (targetPlayer != null)
        {
            RotateTowardsPlayer(); // Girar hacia el jugador

            if (Time.time >= nextFireTime)
            {
                Shoot(); // Disparar hacia el jugador
                nextFireTime = Time.time + fireRate; // Actualizar el tiempo del pr�ximo disparo
            }
        }
    }
    // M�todo para encontrar al jugador m�s cercano
    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Buscar todos los jugadores
        float nearestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < nearestDistance && distance <= detectionRange)
            {
                nearestDistance = distance;
                nearestPlayer = player;
            }
        }

        targetPlayer = nearestPlayer; // Asignar al jugador m�s cercano como objetivo
    }

    // M�todo para girar hacia el jugador
    void RotateTowardsPlayer()
    {
        if (targetPlayer != null)
        {
            // Calcular la direcci�n hacia el jugador
            Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;

            // Calcular la rotaci�n necesaria para mirar hacia el jugador
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Suavizar la rotaci�n hacia el jugador
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Asegurarse de que el firePoint tambi�n apunte hacia el jugador
            if (firePoint != null)
            {
                firePoint.rotation = lookRotation;
            }
        }
    }

    // M�todo para disparar
    void Shoot()
    {
        if (targetPlayer != null && projectilePrefab != null && firePoint != null)
        {
            // Calcular la direcci�n hacia el jugador
            Vector3 direction = (targetPlayer.transform.position - firePoint.position).normalized;

            // Instanciar el proyectil en la red
            GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab.name, firePoint.position, firePoint.rotation);
            projectile.GetComponent<Projectile>().SetDirection(direction); // Establecer la direcci�n del proyectil
        }
    }
}
