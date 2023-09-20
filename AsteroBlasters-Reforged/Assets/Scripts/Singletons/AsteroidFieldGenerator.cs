using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameDevelopmentTools
{
    /// <summary>
    /// Class providing functionality, allowing to select specific area in editor, which will later be filled with asteroids on Start
    /// </summary>
    public class AsteroidFieldGenerator : MonoBehaviour
    {
        [SerializeField] GameObject asteroidPrefab;

        [SerializeField] Tilemap map;

        int spawningChance = 10;

        private void Start()
        {
            // Instantiating the temproary variables
            Vector3Int currentPosition;
            TileBase currentTile;

            // Adding one, to initial x and y value in order to avoid spawning asteroids next to end of the tile map
            // Subtracting one from maxium x and y value in order to avoid spawning asteroids next to end of the tile map
            // Adding 2 to x and y every iteration in order to place asteroid on every tile that doesn't touch other tiles with asteroids

            // Iterating through every row
            for (float x = map.LocalToWorld(map.localBounds.min).x + 1; x < map.LocalToWorld(map.localBounds.max).x - 1; x += 2)
            {
                // Iterating through every column
                for (float y = map.LocalToWorld(map.localBounds.min).y + 1; y < map.LocalToWorld(map.localBounds.max).y - 1; y += 2)
                {
                    // Getting the position, in which the tile should be created and accessing the tile on that position
                    currentPosition = new Vector3Int((int)x, (int)y, 0);
                    currentTile = map.GetTile(currentPosition);

                    // Checking if accessed tile isn't empty
                    if (currentTile != null)
                    {
                        float currentSpawningChance = Random.Range(0, 100);

                        if (currentSpawningChance >= spawningChance)
                        {
                            //Randomizing rotation
                            float randomZ = Random.Range(0f, 359f);

                            // Instantiating the asteroid in its position
                            GameObject createdAsteroid = Instantiate(asteroidPrefab, new Vector2(currentPosition.x + 0.5f, currentPosition.y + 0.5f), Quaternion.identity);
                            createdAsteroid.transform.Rotate(0, 0, randomZ);

                            // Setting up the spawning chance back to 10
                            spawningChance = 10;
                        }
                        else
                        {
                            // Increasing the spawning chance by 5
                            spawningChance += 5;
                        }
                    }
                }
            }
        }
    }
}