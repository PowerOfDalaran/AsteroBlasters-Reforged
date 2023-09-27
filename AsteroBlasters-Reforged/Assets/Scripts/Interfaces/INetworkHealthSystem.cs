namespace PlayerFunctionality
{
    /// <summary>
    /// Interface forcing classes to implement health related mechanics (in network version of scripts)
    /// </summary>
    interface INetworkHealthSystem
    {
        /// <summary>
        /// Method dealing damage to the script owner
        /// </summary>
        /// <param name="damage">Amount of damage</param>
        /// <param name="damagingPlayer">Id of player, who activated this method (-1 if no other player did it)</param>
        public void TakeDamage(int damage, long damagingPlayer = -1);

        /// <summary>
        /// Method responsible for death of the script owner
        /// </summary>
        /// <param name="killerPlayerId">Id of player, who killed this player character (-1 if no player was killer)</param>
        public void Die(long killerPlayerId);
    }
}
