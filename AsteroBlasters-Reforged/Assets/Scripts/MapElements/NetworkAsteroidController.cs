using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameMapElements
{
    public class NetworkAsteroidController : NetworkBehaviour
    {
        // Arrays of possible sprites, which corresponds to different asteroid types
        [SerializeField] Sprite[] smallSprites;
        [SerializeField] Sprite[] mediumSprites;
        [SerializeField] Sprite[] bigSprites;

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

        public void TakeDamage(int damage)
        {
            TakeDamageServerRpc(damage);
        }

        [ServerRpc(RequireOwnership = false)]
        void TakeDamageServerRpc(int damage)
        {
            currentDurability -= damage;

            if (currentDurability <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            SplitAsteroidServerRpc();
            DespawnSelfServerRpc();
        }

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
        }

        [ServerRpc]
        void DespawnSelfServerRpc()
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }
}
