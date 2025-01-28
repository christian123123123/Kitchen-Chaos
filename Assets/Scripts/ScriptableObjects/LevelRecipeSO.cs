using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelRecipeSO : ScriptableObject {
    [System.Serializable]
    public class LevelRecipe {
        public int levelIndex;
        public List<RecipeSO> recipes;
    }

    public List<LevelRecipe> levelRecipes;
}
