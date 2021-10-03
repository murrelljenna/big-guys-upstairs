using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using static game.assets.utilities.GameUtils;

public class TestNormalizePrefabName
{
    [Test]
    public void TestTwoDigits()
    {
        string result = "Light Infantry(14)";

        Assert.AreEqual(
            "Light Infantry",
            normalizePrefabName(result)
        );
    }

    [Test]
    public void TestThreeDigits()
    {
        string result = "Light Infantry(144)";

        Assert.AreEqual(
            "Light Infantry",
            normalizePrefabName(result)
        );
    }

    [Test]
    public void TestClone()
    {
        string result = "Light Infantry(Clone)";

        Assert.AreEqual(
            "Light Infantry",
            normalizePrefabName(result)
        );
    }

}
