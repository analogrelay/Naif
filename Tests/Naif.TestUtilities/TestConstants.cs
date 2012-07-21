namespace Naif.TestUtilities
{
    public class TestConstants
    {
        public const string PETAPOCO_DatabaseName = "Test.sdf";
        public const string PETAPOCO_TableName = "Dogs";
        public const string PETAPOCO_CreateTableSql = "CREATE TABLE Dogs (ID int IDENTITY(1,1) NOT NULL, Name nvarchar(100) NOT NULL, Age int NULL)";
        public const string PETAPOCO_InsertRow = "INSERT INTO Dogs (Name, Age) VALUES ('{0}', {1})";
        public const string PETAPOCO_DogNames = "Spot,Buster,Buddy,Spot,Gizmo";
        public const string PETAPOCO_DogAges = "1,5,3,4,6";
        public const int PETAPOCO_RecordCount = 5;

        public const string PETAPOCO_InsertDogName = "Milo";
        public const int PETAPOCO_InsertDogAge = 3;

        public const int PETAPOCO_DeleteDogId = 2;
        public const string PETAPOCO_DeleteDogName = "Buster";
        public const int PETAPOCO_DeleteDogAge = 5;

        public const int PETAPOCO_ValidDogId = 3;
        public const string PETAPOCO_ValidDogName = "Buddy";
        public const int PETAPOCO_ValidDogAge = 3;

        public const int PETAPOCO_InvalidDogId = 999;

        public const string PETAPOCO_UpdateDogName = "Milo";
        public const int PETAPOCO_UpdateDogAge = 6;
        public const int PETAPOCO_UpdateDogId = 3;

        public const int PAGE_First = 0;
        public const int PAGE_Second = 1;
        public const int PAGE_Last = 4;
        public const int PAGE_RecordCount = 5;
        public const int PAGE_TotalCount = 22;

        public const int PAGE_NegativeIndex = -1;
        public const int PAGE_OutOfRange = 5;
    }
}