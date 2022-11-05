using game.assets.player;
using game.assets.utilities;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.utilities.resources;

namespace Tests.Serialization
{
    public class TestPlayerSerialization
    {
        [Test]
        public void TestPlayerSerializeResources()
        {
            var player = new Player();
            var playerResources = new ResourceSet(100, 100);
            player.resources = playerResources;
            Player roundTripSerializedPlayer = (Player)Player.Deserialize(
                Player.Serialize(player)
            );

            Assert.AreEqual(
                roundTripSerializedPlayer.resources,
                playerResources
            );
        }

        [Test]
        public void TestPlayerSerializeColour()
        {
            var player = new Player(GameUtils.PlayerColours.Pink);
            Player roundTripSerializedPlayer = (Player)Player.Deserialize(
                Player.Serialize(player)
            );

            Assert.AreEqual(
                roundTripSerializedPlayer.colour,
                player.colour
            );
        }
    }
}
