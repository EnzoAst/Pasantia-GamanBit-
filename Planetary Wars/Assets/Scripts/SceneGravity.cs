public static class SceneGravity
{
    public static float playerSpeed = 4.5f;
    public static float jumpHeight = 2f;
    public static float jumpDuration = 0.5f;

    public static void SetGravityForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Mars":
                playerSpeed = 3f;
                jumpHeight = 3f;
                jumpDuration = 1.5f;
                break;

            case "Artic":
                playerSpeed = 3.5f;
                jumpHeight = 3f;
                jumpDuration = 1.2f;
                break;

            case "Ocean":
                playerSpeed = 3.8f;
                jumpHeight = 2f;
                jumpDuration = 1f;
                break;

            case "Moon":
                playerSpeed = 3f;
                jumpHeight = 3.3f;
                jumpDuration = 2f;
                break;

            case "Desert":
                playerSpeed = 4f;
                jumpHeight = 2f;
                jumpDuration = 0.5f;
                break;

            case "Terran":
                playerSpeed = 4f;
                jumpHeight = 1f;
                jumpDuration = 0.5f;
                break;

            case "Ruins":
                playerSpeed = 4.6f;
                jumpHeight = 1.5f;
                jumpDuration = 0.4f;
                break;

            case "Lava":
                playerSpeed = 2.5f;
                jumpHeight = 1.5f;
                jumpDuration = 0.4f;
                break;

            default:
                playerSpeed = 6f;
                jumpHeight = 2f;
                jumpDuration = 0.5f;
                break;
        }
    }
}
