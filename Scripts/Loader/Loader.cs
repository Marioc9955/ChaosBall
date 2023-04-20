using UnityEngine.SceneManagement;

public static class Loader
{

    public enum Scene
    {
        MainMenuScene,
        LevelWaterfall,
        LevelDesert,
        LevelPastizal,
        LoadingScene
    }

    public enum AI
    {
        NoAI, AI1, AI2
    }


    private static Scene targetScene;

    public static AI targetAI { get; private set; }

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void Load(Scene targetScene, AI targetAI)
    {
        Loader.targetAI= targetAI;
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

}
