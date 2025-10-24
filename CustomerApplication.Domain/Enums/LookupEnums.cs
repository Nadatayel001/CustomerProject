namespace CustomerApplication.CustomerApplication.Domain.Enums
{
    public class LookupEnums
    {
        // Categories
        public enum CategoryCode
        {
            District = 1,
            Governorate = 2,
            Gender = 3,
            Village = 4
        }

        // District enum
        public enum District    // filtered by catId=1
        {
            Dokki = 1,
            NasrCity = 2
        }

        // Governorate enum   // filtered by catId=2
        public enum Governorate
        {
            Cairo = 1,
            Giza = 2
        }

        // Gender enum
        public enum Gender  // filtered by catId=3
        {
            Female = 1,
            Male = 2
        }

        // Village enum    // filtered by catId=4
        public enum Village
        {
            VillageA = 1,
            VillageB = 2
        }

    }
}
