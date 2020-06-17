using System;

namespace FetchDataFromDynamics
{
    public class Contact
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Spark_ContactNumber { get; }
        public DateTime CreatedOn { get; }
        public char CardCreated { get; } = 'N';
        public int NumberOfCards { get; } = 0;
        public string ImagePath { get; } = "\\\\mls_storage\\member_photos\\{{}}.jpg";
       
        public Contact(string firstName, string lastName, string spark_ContactNumber, DateTime createdOn)
        {
            FirstName = firstName;
            LastName = lastName;
            Spark_ContactNumber = spark_ContactNumber;
            CreatedOn = createdOn;
            ImagePath = String.Format(ImagePath, Spark_ContactNumber);
        }
    }
}