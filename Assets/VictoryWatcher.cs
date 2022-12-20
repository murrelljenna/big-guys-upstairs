using Fusion;
using game.assets.player;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets {

    public class VictoryWatcher : NetworkBehaviour
    {
        private List<GameObject> towns = new List<GameObject>();
        [Tooltip("Fireworks prefab to use when player wins")]
        public GameObject fireworksPrefab;
        private void Start()
        {
            Instantiation.ObjectSpawned.AddListener((netObj) =>
            {
                netObj.gameObject.IfIsA(
                    IsAUtils.IsATown,
                    (GameObject town) => {
                        towns.Add(town);
                        return town;
                    }
                );
            });

            Instantiation.OnBeforeObjectDespawned.AddListener((netObj) =>
            {
                netObj.gameObject.IfIsA(
                    IsAUtils.IsATown,
                    (GameObject town) => {
                        towns.Remove(town);
                        checkVictory();
                        return town;
                    }
                );
            });
        }

        private void checkVictory()
        {
            if (towns.Count == 0)
            {
                return;
            }

            if (towns.Count == 1 || towns.TrueForAll((GameObject go) => go.IsFriendOf(towns[0])))
            {
                playerWins(towns[0].GetComponent<Ownership>().owner);
            }

            return;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_FireworksAtCity(NetworkObject obj)
        {
            PlayAudio.PlayRandomSourceOnGameobject(obj.transform?.Find("Audio")?.Find("CheerSounds").gameObject);
            Instantiate(fireworksPrefab, obj.transform);
            Invoke("QuitToMainMenu", 10f);
        }

        protected void playerWins(Player player)
        {
            for (int i = 0; i < towns.Count; i++)
            {
                var city = towns[i];
                RPC_FireworksAtCity(city.GetComponent<NetworkObject>());
            }
        }

        private void QuitToMainMenu()
        {
            NetworkedGameManager.Get().QuitToMainMenu();
        }
    }
}
