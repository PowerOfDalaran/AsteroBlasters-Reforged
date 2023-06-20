using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface forcing classes to implement health related mechanics
/// </summary>
interface IHealthSystem
{
    /// <summary>
    /// Method dealing damage to the script owner
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    public void TakeDamage(int damage);
    /// <summary>
    /// Method responsible for death of the script owner
    /// </summary>
    void Die();
}
