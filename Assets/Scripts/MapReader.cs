using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

public class MapReader : MonoBehaviour {
	[XmlRoot(ElementName="Vector3")]
	public class vector3Helper {
		[XmlAttribute(AttributeName="x")]
		public string x { get; set; }
		[XmlAttribute(AttributeName="y")]
		public string y { get; set; }
		[XmlAttribute(AttributeName="z")]
		public string z { get; set; }

		public Vector3 toVector3() {
			return new Vector3(float.Parse(this.x), float.Parse(this.y), float.Parse(this.z));
		}

		public string toCSV() {
			return this.x + ',' + this.y + ',' + this.z;
		}
	}

	[XmlRoot(ElementName="spawns")]
	public class Spawns {
		[XmlElement(ElementName="Vector3")]
		public List<vector3Helper> locations { get; set; }
	}

	[XmlRoot(ElementName="map")]
	public class Map {
		[XmlElement(ElementName="sceneName")]
		public string sceneName { get; set; }
		[XmlElement(ElementName="players")]
		public string players { get; set; }
		[XmlElement(ElementName="spawns")]
		public Spawns spawns { get; set; }
		[XmlAttribute(AttributeName="name")]
		public string name { get; set; }
	}

	[XmlRoot(ElementName="maps")]
	public class Maps {
		[XmlElement(ElementName="map")]
		public List<Map> maps { get; set; }
	}

	public static T Deserialize<T>(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		StreamReader reader = new StreamReader(path);
		T deserialized = (T)serializer.Deserialize(reader.BaseStream);
		reader.Close();
		return deserialized;
	}

	public Maps read() {
		return(Deserialize<Maps>(Path.Combine(Application.dataPath, "StreamingAssets/xml/maps.xml")));
	}

	public Map randomMap() {
		Maps maps = Deserialize<Maps>(Path.Combine(Application.dataPath, "StreamingAssets/xml/maps.xml"));
		//int index = UnityEngine.Random.Range(0, maps.maps.Count);
        int index = 0;
		    return maps.maps[index];
	  }
}
