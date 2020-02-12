using System;

namespace MSAL
{
    public class Contact
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Spark_ContactNumber { get; }
        public DateTime CreatedOn { get; }
       
        public Contact(string firstName, string lastName, string spark_ContactNumber, DateTime createdOn)
        {
            FirstName = firstName;
            LastName = lastName;
            Spark_ContactNumber = spark_ContactNumber;
            CreatedOn = createdOn;
        }
    }
}