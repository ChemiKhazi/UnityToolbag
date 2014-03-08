using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameRandom {

	[System.Serializable]
	public class RandomChance
	{
		[System.Serializable]
		public class ChanceEntry
		{
			public string name;
			[Range(0f, 1f)]
			public float chance;
		}

		public ChanceEntry[] chances;

		public void SetAndNormalze(string chance, float newValue)
		{
		}
	}

	public bool IsInitialized
	{
		get;
		private set;
	}

	List<bool> deck = new List<bool>();
	int currentIndex = 0;

	public GameRandom(int chances, int chanceSpace)
	{
		deck = new List<bool>();
		deck.AddRange(Enumerable.Repeat(false, chanceSpace));
		for (int i = 0; i < chances; i++)
		{
			deck[i] = true;
		}
		Shuffle();
	}

	public void Shuffle()
	{
		for (int i = 0; i < deck.Count; i++)
		{
			bool tempVal = deck[i];
			int randIndex = Random.Range(i, deck.Count);
			deck[i] = deck[randIndex];
			deck[randIndex] = tempVal;
		}
	}

	public bool GetChance()
	{
		bool chance = deck[currentIndex];
		currentIndex++;
		if (currentIndex == deck.Count)
		{
			Shuffle();
			currentIndex = 0;
		}
		return chance;
	}
}
