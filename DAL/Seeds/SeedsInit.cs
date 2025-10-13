namespace DAL.Seeds;

public class SeedsInit
{
    private static bool _isLoaded = false;

    public static void LoadLists()
    {
        if (_isLoaded) return;
        UserSeeds.LoadLists();
        VideoSeeds.LoadLists();
        PlaylistSeeds.LoadLists();
        CommentSeeds.LoadLists();
        OrderSeeds.LoadLists();
        SubscriptionSeeds.LoadLists();
        _isLoaded = true;
    }
}

