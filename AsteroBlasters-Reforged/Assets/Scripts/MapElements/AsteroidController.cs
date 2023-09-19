using PlayerFunctionality;
using UnityEngine;

namespace GameMapElements
{
    /// <summary>
    /// Enumerator representing the three possible sizes of asteroids.
    /// </summary>
    public enum AsteroidTypes
    {
        BigAsteroid,
        MediumAsteroid,
        SmallAsteroid,
    }

    /// <summary>
    /// Class responsible for the behaviour of asteroids
    /// </summary>
    public class AsteroidController : MonoBehaviour, IHealthSystem
    {
        // Arrays of possible sprites, which corresponds to different asteroid types
        [SerializeField] Sprite[] smallSprites;
        [SerializeField] Sprite[] mediumSprites;
        [SerializeField] Sprite[] bigSprites;

        [SerializeField] public AsteroidTypes asteroidType;

        [SerializeField] int maxDurability;
        [SerializeField] int currentDurability;

        SpriteRenderer mySpriteRenderer;

        void Start() 
        {
            mySpriteRenderer = GetComponent<SpriteRenderer>();

            // Setting up the properties, which differs between asteroid types
            if (asteroidType == AsteroidTypes.SmallAsteroid)
            {
                maxDurability = 1;
                mySpriteRenderer.sprite = smallSprites[Random.Range(0, smallSprites.Length)];
            }
            else if (asteroidType == AsteroidTypes.MediumAsteroid)
            {
                maxDurability = 2;
                mySpriteRenderer.sprite = mediumSprites[Random.Range(0, mediumSprites.Length)];
            }
            else if (asteroidType == AsteroidTypes.BigAsteroid)
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
            currentDurability -= damage;

            if (currentDurability <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            SplitAsteroid();
            Destroy(gameObject);
        }

        /// <summary>
        /// Method, which creates two new asteroids in place of the old one (if the asteroid size is bigger than "small" one)
        /// </summary>
        void SplitAsteroid()
        {
            if (asteroidType != AsteroidTypes.SmallAsteroid)
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
                    GameObject subAsteroid = Instantiate(gameObject, spawnPositions[i], Quaternion.identity);

                    AsteroidController createdAsteroidController = subAsteroid.GetComponent<AsteroidController>();

                    // Assigning the type of new asteroid
                    if (asteroidType == AsteroidTypes.BigAsteroid)
                    {
                        createdAsteroidController.asteroidType = AsteroidTypes.MediumAsteroid;
                    }
                    else
                    {
                        createdAsteroidController.asteroidType = AsteroidTypes.SmallAsteroid;
                    }

                    // Launching the new asteroid in randomized direction
                    subAsteroid.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomForceX[i], randomForceY[i]), ForceMode2D.Impulse);
                }
            }
        }
    }
}
