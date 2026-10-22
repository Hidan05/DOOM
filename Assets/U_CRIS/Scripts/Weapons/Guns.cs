using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevaArma", menuName = "Armas/Nueva Arma")]
public class Guns : ScriptableObject
{
    public string weaponName;
    public float range;
    public float horizontalRange;
    public float verticalRange;
    public float fireRate;
    public int damage;

    [Header("Munición")]
    public int magazineSize;          // Cuántas balas caben en el cargador
    public string ammoType;           // Ej: "Pistol" o "Shotgun"

    [Header("Sonidos")]
    public AudioClip sound;           // Sonido de disparo
    public AudioClip emptySound;      // Sonido cuando intentas disparar sin balas
}