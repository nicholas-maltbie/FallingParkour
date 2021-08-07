using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace PropHunt.Environment.Checkpoint
{
    public static class SpawnPointUtilities
    {
        public static System.Random rand = new System.Random();

        public static IEnumerable<(Vector3, Quaternion)> GetRandomizedSpawns(this ISpawnPointCollection collection)
        {
            return Enumerable.Range(0, collection.Total())
                .OrderBy(x => rand.Next())
                .Select(i => collection.GetSpawnPoint(i));
        }

        public static (Vector3, Quaternion) GetRandomSpawn(this ISpawnPointCollection collection)
        {
            return collection.GetSpawnPoint(Mathf.Abs(rand.Next()) % collection.Total());
        }

        public static ISpawnPointCollection GetDefaultCheckpoint()
        {
            GameObject go = GameObject.FindGameObjectWithTag("DefaultCheckpoint");
            if (go != null)
            {
                return go.GetComponent<ISpawnPointCollection>();
            }
            return null;
        }
    }
}