using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

//namespace LAViD.LibrasKe {

	public static class GameData {

		public const string Music_OsGonzagas_AmorDaGota = "os_gonzagas_amor";
		public const string Music_SeuPereira_CarimboDaPenha = "seu_pereira_carimbo";
		public const string Music_BetoBrito_XoteMeiDeFeira = "beto_brito_xote";

		public static KinectSensor sensor = null;
		public static BodyFrameReader reader;

		public static string music { get; set; }

		public static float score = 0.7f * 0.8f;

		public static Dictionary<string, string> music_presentation_name = new Dictionary<string, string>()
		{
			{ Music_OsGonzagas_AmorDaGota, "Os Gonzagas - Amor da Gota" },
			{ Music_SeuPereira_CarimboDaPenha, "Seu Pereira & Coletivo 401 - Carimbó da Penha" },
			{ Music_BetoBrito_XoteMeiDeFeira, "Beto Brito - Xote Mei-de-Feira" }
		};

		public static string get_music_presentation_name()
		{
			Debug.Log(music);
			return music_presentation_name[music];
		}

	}

//}