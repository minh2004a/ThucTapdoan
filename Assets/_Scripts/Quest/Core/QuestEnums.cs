namespace QuestSystem
{
    public enum QuestType
    {
        Story,
        Side,
        Daily,
        Income,
        Combat,
        Delivery,
        Collection,
        Tutorial
    }

    public enum QuestCategory
    {
        MainStory,
        NPCPersonal,
        ResourceGather,
        MonsterHunt,
        ShopMission,
        Exploration,
        Seasonal
    }

    public enum QuestTier
    {
        Tutorial,
        Easy,
        Medium,
        Hard,
        Epic,
        Legendary
    }

    public enum QuestState
    {
        Locked,
        Available,
        Active,
        Completed,
        Failed,
        Archived
    }

    public enum ObjectiveType
    {
        Collect,
        Kill,
        TalkTo,
        Buy,
        Sell,
        Plant,
        Harvest,
        Explore,
        Deliver,
        Equip,
        EarnMoney,
        Mine,
        Custom
    }

    public enum ObjectiveOrder
    {
        Parallel,
        Sequential
    }

    public enum RewardType
    {
        Gold,
        Item,
        Experience,
        Reputation,
        QuestUnlock,
        ShopUnlock,
        AreaUnlock,
        Recipe,
        Custom
    }

    public enum ReputationLevel
    {
        Stranger = 0,
        Acquaintance = 100,
        Friend = 300,
        BestFriend = 600,
        Soulmate = 1000
    }

    public enum ValidationType
    {
        Valid,
        LevelTooLow,
        PrerequisiteNotMet,
        InsufficientReputation,
        InventoryFull,
        MaxActiveQuestsReached,
        OnCooldown,
        AlreadyCompleted,
        SeasonRestriction
    }

    public enum SlotState
    {
        Empty,
        Available,
        Active,
        Completed,
        Locked
    }

    public enum MoneyTrackingMode
    {
        Total,
        Earned
    }

    public enum IncomeType
    {
        Farming,
        ResourceGathering,
        Selling,
        Mixed
    }

    public enum VendorRewardType
    {
        Discount,
        UnlockItem,
        SpecialOffer,
        Reputation
    }

    public enum UnlockType
    {
        Quest,
        ShopItem,
        Area,
        Recipe,
        Feature
    }
}
