using UnityEditor;
using UnityEngine;

public class PlayerPrefsEditor {

    [MenuItem("Tools/PlayerPrefs/DeleteAll")]
    static void DeleteAll(){
        PlayerPrefs.DeleteAll();
        Debug.Log("Delete All Data Of PlayerPrefs!!");
    }

    [MenuItem("Tools/PlayerPrefs/DeleteAddCostume")]
    static void DeleteAddCostume(){
        PlayerPrefs.DeleteKey("Set_black_locked");
        PlayerPrefs.DeleteKey("CostumeColor_default_locked");
        Debug.Log("Delete All Data Of AddCostume");
    }
}