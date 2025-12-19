
using UnityEngine;
using SQLite4Unity3d;
using Assets;
using Assets.UI;
using System.Linq;




#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

public class DataService  {

	private SQLiteConnection _connection;

	public DataService(string DatabaseName){

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);     

	}

	
	public IEnumerable<Food> GetFoods(){

        var foodTable = _connection.Table<Food>().ToList();
        var foodEffectsTable = _connection.Table<FoodEffect>().ToList();

        foreach (var food in foodTable)
        {
            if (food.EffectId != null)
            {
                var description = string.Format(foodEffectsTable.First(x => x.Id == food.EffectId).Description, food.EffectAmount);
                food.Effect = new FoodEffect { Id = food.EffectId.Value, Description = description };
            }
        }

        return foodTable;
	}

    public IEnumerable<Level> GetLevels()
    {
        var levelTable = _connection.Table<Level>().ToList();
        var levelRewardTable = _connection.Table<LevelReward>().ToList();
        var unlockedLevels = _connection.Table<UnlockedLevels>().ToList();

        foreach(var level in levelTable)
        {
            foreach (var levelReward in levelRewardTable.Where(x => x.LevelId == level.Id))
            {
                level.RewardsList.Add(levelReward);
            }

            foreach(var reward in level.RewardsList)
            {
                level.Rewards.Add(reward.Grade, reward);
            }

            if(unlockedLevels.Any(x => x.LevelId == level.Id))
            {
                level.Unlocked = true;
            }
        }

        return levelTable;
    }

    public IEnumerable<Section> GetSections()
    {
        var sectionTable = _connection.Table<Section>().ToList();
        var unlockedSectionsTable = _connection.Table<UnlockedSections>().ToList();
        var foodToUnlockTable = _connection.Table<SectionFood>().ToList();

        foreach (var section in sectionTable)
        {
            if (unlockedSectionsTable.Any(x => x.SectionId == section.Id))
            {
                section.Unlocked = true;
            }

            section.FoodToUnlock = foodToUnlockTable.Where(x => x.SectionId == section.Id).ToList();
        }

        return sectionTable;
    }

    public void StoreUnlockedLevel(int id)
    {
        _connection.Insert(new UnlockedSections { SectionId = id});
    }

    public void StoreUnlockedSection(int id)
    {
        _connection.Insert(new UnlockedLevels { LevelId = id });
    }



    public IEnumerable<PlayerFood> GetPlayerFood()
    {
        return _connection.Table<PlayerFood>();
    }

    public void StorePlayerFood(List<PlayerFood> playerFood)
    {
        _connection.UpdateAll(playerFood);
    }

    public void AddPlayerFood(PlayerFood playerFood)
    {
        _connection.Insert(playerFood);
    }

}
