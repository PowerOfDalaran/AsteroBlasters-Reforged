using PickableObjects;
using PlayerFunctionality;
using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameMapElements
{
    /// <summary>
    /// Class responsible for the behaviour of asteroids (network version)
    /// </summary>
    public class NetworkAsteroidController : NetworkBehaviour, INetworkHealthSystem
    {
        // Arrays of possible sprites, which corresponds to different asteroid types
        [SerializeField] Sprite[] smallSprites;
        [SerializeField] Sprite[] mediumSprites;
        [SerializeField] Sprite[] bigSprites;

        [SerializeField] GameObject[] possibleRewards;

        [SerializeField] GameObject asteroidPrefab;

        [NonSerialized] NetworkVariable<AsteroidTypes> asteroidType;

        public AsteroidTypes AsteroidType
        {
            get { return asteroidType.Value; }
            set { asteroidType.Value = value; }
        }

        [SerializeField] int maxDurability;
        [SerializeField] int currentDurability;

        SpriteRenderer mySpriteRenderer;

        private void Awake()
        {
            asteroidType = new NetworkVariable<AsteroidTypes>();
        }

        void Start()
        {
            mySpriteRenderer = GetComponent<SpriteRenderer>();

            // Setting up the properties, which differs between asteroid types
            if (AsteroidType == AsteroidTypes.SmallAsteroid)
            {
                maxDurability = 1;
                mySpriteRenderer.sprite = smallSprites[Random.Range(0, smallSprites.Length)];
            }
            else if (AsteroidType == AsteroidTypes.MediumAsteroid)
            {
                maxDurability = 2;
                mySpriteRenderer.sprite = mediumSprites[Random.Range(0, mediumSprites.Length)];
            }
            else if (AsteroidType == AsteroidTypes.BigAsteroid)
            {
                maxDurability = 3;
                mySpriteRenderer.sprite = bigSprites[Random.Range(0, bigSprites.Length)];
            }
            currentDurability = maxDurability;

            // Destroying Polygon Collider 2D if such collider already exist
            if (gameObject.GetComponent<PolygonCollider2D>() != null)
            {
                Destroy(gameObject.GetComponent<PolygonCollider2D>());
            }

            // Adding new one (which will automatically change its shape to match shape of the new sprite)
            gameObject.AddComponent<PolygonCollider2D>();
        }

        public void TakeDamage(int damage, long damagingPlayer = -1)
        {
            TakeDamageServerRpc(damage);
        }

        [ServerRpc(RequireOwnership = false)]
        void TakeDamageServerRpc(int damage)
        {
            currentDurability -= damage;

            if (currentDurability <= 0)
            {
                Die(-1);
            }
        }

        public void Die(long killerPlayerId)
        {
            SplitAsteroidServerRpc();
            DespawnSelfServerRpc();
        }

        /// <summary>
        /// Server rpc method, which creates two new asteroids in place of the old one.
        /// If the asteroid is the type of "small", creating the reward instead.
        /// </summary>
        [ServerRpc]
        void SplitAsteroidServerRpc()
        {
            if (AsteroidType != AsteroidTypes.SmallAsteroid)
            {
                // Generazing random offset for the first asteroid, and reversing it to create the offset for the second one
                float randomChangeX = Random.Range(0f, 1f);
                float randomChangeY = Random.Range(0f, 1f);

                // Creating new positions, modified by generated offset
                Vector2[] spawnPositions = new Vector2[2];
                spawnPositions[0] = new Vector2(transform.position.x + randomChangeX, transform.position.y + randomChangeY);
                spawnPositions[1] = new Vector2(transform.position.x - randomChangeX, transform.position.y - randomChangeY);

                // Generating random value for force in X direction for the first asteroid, and reversing it to create the force for the second one
                float[] randomForceX = new float[2];
                randomForceX[0] = Random.Range(0.5f, 2f);
                randomForceX[1] = -randomForceX[0];

                // Generating random value for force in Y direction for the first asteroid, and reversing it to create the force for the second one
                float[] randomForceY = new float[2];
                randomForceY[0] = Random.Range(0.5f, 2f);
                randomForceY[1] = -randomForceY[0];

                for (int i = 0; i < 2; i++)
                {
                    // Creating new asteroid and accessing its AsteroidController script
                    GameObject subAsteroid = Instantiate(asteroidPrefab, spawnPositions[i], Quaternion.identity);

                    NetworkAsteroidController createdAsteroidController = subAsteroid.GetComponent<NetworkAsteroidController>();

                    // Assigning the type of new asteroid
                    if (AsteroidType == AsteroidTypes.BigAsteroid)
                    {
                        createdAsteroidController.AsteroidType = AsteroidTypes.MediumAsteroid;
                    }
                    else
                    {
                        createdAsteroidController.AsteroidType = AsteroidTypes.SmallAsteroid;
                    }

                    subAsteroid.GetComponent<NetworkObject>().Spawn();

                    // Launching the new asteroid in randomized direction
                    subAsteroid.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomForceX[i], randomForceY[i]), ForceMode2D.Impulse);
                }
            }
            else
            {
                // Rhecking if the number should be spawned
                int random = Random.Range(1, 11);

                if (random >= 6)
                {
                    // Randomizing the reward and spawning it
                    int rewardIndex = Random.Range(0, possibleRewards.Length);

                    SpawnReward(rewardIndex);
                }
            }
        }

        /// <summary>
        /// Method spawning the reward with given index, checking if it is the weapon power up (if so, randomizing which weapon should be given) and spawning it
        /// </summary>
        /// <param name="rewardIndex">Index of reward, which should be spawned</param>
        void SpawnReward(int rewardIndex)
        {
            // Creating the reward locally
            GameObject createdReward = Instantiate(possibleRewards[rewardIndex], transform.position, Quaternion.identity);

            // Checking if created reward is weapon power up, if so randomizing the weapon it grants
            NetworkWeaponPowerUp createdWeaponPowerUp = createdReward.GetComponent<NetworkWeaponPowerUp>();

            if (createdWeaponPowerUp != null)
            {
                int randomWeapon = Random.Range(0, 3);

                switch (randomWeapon)
                {
                    case 0:
                        createdWeaponPowerUp.GrantedWeapon = WeaponSystem.WeaponClass.PlasmaCannon;
                        break;
                    case 1:
                        createdWeaponPowerUp.GrantedWeapon = WeaponSystem.WeaponClass.MissileLauncher;
                        break;
                    case 2:
                        createdWeaponPowerUp.GrantedWeapon = WeaponSystem.WeaponClass.LaserSniperGun;
                        break;
                    default:
                        Debug.Log("Unexpected number was randomized.");
                        break;
                }
            }

            // Spawning the reward on every connected client
            createdReward.GetComponent<NetworkObject>().Spawn();
        }

        /// <summary>
        /// Method despawning this game object
        /// </summary>
        [ServerRpc]
        void DespawnSelfServerRpc()
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }
}
