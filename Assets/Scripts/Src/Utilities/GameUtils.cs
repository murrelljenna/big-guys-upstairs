using game.assets.ai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace game.assets.utilities {
    public static class GameUtils
    {
        public struct PlayerColour
        {
            public string name;
            public string html;
            public Color color;

            public PlayerColour(string name, string html, Color color)
            {
                this.name = name;
                this.html = html;
                this.color = color;
            }
        }

        public static class LayerMask
        {
            public static int Terrain = (1 << 11);
            public static int Resource = (1 << 9);
            public static int Attackable = Resource | (1 << 10) | (1 << 12) | (1 << 14) | (1 << 16) | (1 << 18);
            public static int All = Terrain | Resource | (1 << 10) | (1 << 12) | (1 << 14) | (1 << 16) | (1 << 18);
        }

        public static class PlayerColours
        {
            public static PlayerColour Blue = new PlayerColour("blue", "#95B2FF", Color.blue);
            public static PlayerColour Red = new PlayerColour("red", "#FF89BB", Color.red);
            public static PlayerColour Green = new PlayerColour("green", "#89ffA0", Color.green);
            public static PlayerColour Yellow = new PlayerColour("yellow", "#FCFF89", Color.yellow);
            public static PlayerColour Pink = new PlayerColour("pink", "#FF80EB", Color.magenta);
            public static PlayerColour White = new PlayerColour("white", "#FFFFFF", Color.white);
            public static PlayerColour Black = new PlayerColour("black", "#9F9F9F", Color.black);
        }

        public static string normalizePrefabName(string name)
        {
            string rx = "\\(([0-9]*|Clone)\\)";

            return Regex.Replace(name, rx, "");
        }

        public static GameObject[] findGameObjectsInRange(Vector3 center, float range)
        {
            Collider[] hitColliders = Physics.OverlapSphere(center, range);
            GameObject[] units = new GameObject[hitColliders.Length];
            for (int i = 0; i < hitColliders.Length; i++) {
                units[i] = hitColliders[i].gameObject;
            }
            return units;
        }

        public static Health[] findEnemyUnitsInRange(Vector3 center, float range)
        {
            return Physics.OverlapSphere(center, range).GetComponents<Health>();
        }
        private static Vector2 randomPointOnUnitCircle(float radius) {
            float angle = Random.Range (0f, Mathf.PI * 2);
            float x = Mathf.Sin (angle) * radius;
            float y = Mathf.Cos (angle) * radius;

            return new Vector2(x, y);
        }

        public static Vector2 randomPointOnUnitCircle(Vector3 offset, float radius)
        {
            Vector2 randomInCircle = randomPointOnUnitCircle(radius);
            return new Vector3(randomInCircle.x + offset.x, 0, randomInCircle.y + offset.z);
        }


        public static class MagicWords
        {
            public static class GameObjectNames
            {
                public static string Player = "Player";
                public static string StartingCity = "Capitol";
                public static string GameManager = "GameManager";
                public static string ClientSingleton = "ClientSingleton";
                public static string FirstPersonCharacter = "FirstPersonCharacter";
                public static string BuildingMenu = "BuildingMenu";
                public static string CommandMenu = "CommandMenu";
            }
        }

        public static T[] GetComponents<T>(this Collider[] colliders) where T : MonoBehaviour {
            List<T> monos = new List<T>();
            for (int i = 0; i < colliders.Length; i++)
            {
                T mono = colliders[i].GetComponent<T>();
                if (mono != null) {
                    monos.Add(mono);
                }
            }

            return monos.ToArray();
        }

        public static bool isInRangeOf(this GameObject gameObject, GameObject otherGameObject, float rng) {
            Vector3 closestPoint;
            Collider collider = otherGameObject.GetComponent<Collider>();

            if (collider != null)
            {
                closestPoint = collider.ClosestPointOnBounds(otherGameObject.transform.position);
            }
            else
            {
                closestPoint = otherGameObject.transform.position;
            }


            return gameObject.isInRangeOf(closestPoint, rng);
        }

        public static bool isInRangeOf(this GameObject gameObject, Vector3 point, float rng) {
            float deltaX = gameObject.transform.position.x - point.x;
            float deltaZ = gameObject.transform.position.z - point.z;
            float distance = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);

            return (distance < rng);
        }

        public static T[] GetComponents<T>(this GameObject[] gameObjects) where T : MonoBehaviour {
            List<T> monos = new List<T>();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                T mono = gameObjects[i].GetComponent<T>();
                if (mono != null) {
                    monos.Add(mono);
                }
            }

            return monos.ToArray();
        }

        public static class MagicNumbers
        {
            public static float PlayerSpawnRadius = 3f;
        }
    }
}
